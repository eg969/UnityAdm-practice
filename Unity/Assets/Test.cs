using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioBlockWrapper;
using System.Text;
using System;
using System.Threading;

public class Test : MonoBehaviour
{
    public GameObject objectInstance;
    public GameObject objectPositionInstance;
    private List<GameObject> audioObjects = new List<GameObject>();

    UnityAudioBlock previousUnityBlock;
    UnityAudioBlock nextUnityBlock;
    List<List<UnityAudioBlock>> channelFormats = new List<List<UnityAudioBlock>>();

    Vector3 start;
    Vector3 end;

    Thread newThread;




    void Start()
    {
        newThread = new Thread(new ThreadStart(getBlocksThread));
        newThread.Start();
    }

    void getBlocksThread()
    {
        int j = 0;
        while (queryBlock(j, 0))
        {
            List<UnityAudioBlock> blocks = new List<UnityAudioBlock>();
            int k = 0;
            //GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
            //audioObjects.Add(audioObjectInstance);

            while (queryBlock(j, k))
            {
                UnityAudioBlock nextBlock = getBlock(j, k);
                blocks.Add(nextBlock);
                //GameObject objectPosition = Instantiate(objectPositionInstance) as GameObject;

                //objectPosition.transform.position = new Vector3(nextBlock.x, nextBlock.y, nextBlock.z);
                k++;
            }

            channelFormats.Add(blocks);
            j++;
        }
    }

    void addAnimations()
    {
        initializeObjects();
        for (int y = 0; y < channelFormats.Count; y++)
        {
            GameObject audioObject = audioObjects[y];
            Animation objectAnimation = audioObject.GetComponent<Animation>();
            AnimationClip clip = objectAnimation.GetClip(Encoding.ASCII.GetString(channelFormats[y][0].name));

            AnimationCurve xCurve = new AnimationCurve();
            AnimationCurve yCurve = new AnimationCurve();
            AnimationCurve zCurve = new AnimationCurve();

            newThread.Abort();

            for (int i = 0; i < channelFormats[y].Count; i++)
            {
                xCurve.AddKey(new Keyframe(channelFormats[y][i].rTime, channelFormats[y][i].x));
                yCurve.AddKey(new Keyframe(channelFormats[y][i].rTime, channelFormats[y][i].y));
                zCurve.AddKey(new Keyframe(channelFormats[y][i].rTime, channelFormats[y][i].z));

                clip.SetCurve("", typeof(Transform), "localPosition.x", xCurve);
                clip.SetCurve("", typeof(Transform), "localPosition.y", yCurve);
                clip.SetCurve("", typeof(Transform), "localPosition.z", zCurve);

                objectAnimation.PlayQueued(clip.name, QueueMode.PlayNow, PlayMode.StopAll);
            }
            /////
            newThread.Start();
        }   
    }


    void initializeObjects()
    {
        for (int y = 0; y < channelFormats.Count; y++)
        {
            GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
            audioObjects.Add(audioObjectInstance);

            GameObject audioObject = audioObjects[y];
            audioObject.AddComponent<Animation>();

            Animation objectAnimation = audioObject.GetComponent<Animation>();
            AnimationClip clip = new AnimationClip();
            clip.name = Encoding.ASCII.GetString(channelFormats[y][0].name);
            clip.legacy = true;

            objectAnimation.AddClip(clip, clip.name);
            objectAnimation.Play(clip.name, PlayMode.StopSameLayer);
        }
    }

    void Update()
    {
        addAnimations();
    }

}
