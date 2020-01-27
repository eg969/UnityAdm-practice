using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioBlockWrapper;
using System.Text;
using System;

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

    void Start()
    {
        int j = 0;
        while (queryBlock(j, 0))
        {
            List<UnityAudioBlock> blocks = new List<UnityAudioBlock>();
            int k = 0;
            GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
            audioObjects.Add(audioObjectInstance);

            while (queryBlock(j, k))
            {
                UnityAudioBlock nextBlock = getBlock(j, k);
                blocks.Add(nextBlock);
                GameObject objectPosition = Instantiate(objectPositionInstance) as GameObject;

                objectPosition.transform.position = new Vector3(nextBlock.x, nextBlock.y, nextBlock.z);
                k++;
            }

            channelFormats.Add(blocks);
            j++;
        }





        for (int y = 0; y <channelFormats.Count; y++)
        {
            GameObject audioObject = audioObjects[y];
            audioObject.AddComponent<Animation>();

            Animation objectAnimation = audioObject.GetComponent<Animation>();
            AnimationClip clip = new AnimationClip();
            clip.legacy = true;

            AnimationCurve curve;
            Keyframe[] xKeys = new Keyframe[channelFormats[y].Count];
            Keyframe[] yKeys = new Keyframe[channelFormats[y].Count];
            Keyframe[] zKeys = new Keyframe[channelFormats[y].Count];

            for (int i = 0; i < channelFormats[y].Count; i++)
            {
                xKeys[i] = new Keyframe(channelFormats[y][i].rTime, channelFormats[y][i].x);
                yKeys[i] = new Keyframe(channelFormats[y][i].rTime, channelFormats[y][i].y);
                zKeys[i] = new Keyframe(channelFormats[y][i].rTime, channelFormats[y][i].z);
            }

            curve = new AnimationCurve(xKeys);
            clip.SetCurve("", typeof(Transform), "localPosition.x", curve);

            curve = new AnimationCurve(yKeys);
            clip.SetCurve("", typeof(Transform), "localPosition.y", curve);

            curve = new AnimationCurve(zKeys);
            clip.SetCurve("", typeof(Transform), "localPosition.z", curve);

            objectAnimation.AddClip(clip, clip.name);
            objectAnimation.Play(clip.name);

        }
    }


    void Update()
    {
        
    }

}
