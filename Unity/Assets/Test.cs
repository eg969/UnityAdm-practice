using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioBlockWrapper;
using System.Text;
using System;
using System.Threading;

public class Test : MonoBehaviour
{
    object lockObject = new object();
    public GameObject objectInstance;
    Thread newThread;
    Dictionary<int, GameObject> audioObjects = new Dictionary<int, GameObject>();
    void Start()
    {
        readFile();
        newThread = new Thread(new ThreadStart(getNewBlock));
        newThread.Start();
    }

    void addAnimations()
    {
        newThread.Suspend();
        foreach (UnityAudioChannelFormat channelFormat in channelFormats)
        {
            if (!audioObjects.ContainsKey(channelFormat.cfId))
            {
                GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
                audioObjects.Add(channelFormat.cfId, audioObjectInstance);
                audioObjects[channelFormat.cfId].AddComponent<Animation>();
                Animation objectAnimation = audioObjects[channelFormat.cfId].GetComponent<Animation>();
                AnimationClip clip = new AnimationClip();
                clip.name = channelFormat.cfId.ToString();
                clip.legacy = true;

                objectAnimation.AddClip(clip, clip.name);
                objectAnimation.Play(clip.name, PlayMode.StopSameLayer);
            }
            GameObject audioObject = audioObjects[channelFormat.cfId];
            Animation anim = audioObject.GetComponent<Animation>();
            AnimationClip animationClip = anim.GetClip(channelFormat.cfId.ToString());

            AnimationCurve xCurve = new AnimationCurve();
            AnimationCurve yCurve = new AnimationCurve();
            AnimationCurve zCurve = new AnimationCurve();

            foreach (UnityAudioBlock audioBlock in channelFormat.audioBlocks)
            {

                xCurve.AddKey(new Keyframe(audioBlock.rTime, audioBlock.x));
                yCurve.AddKey(new Keyframe(audioBlock.rTime, audioBlock.y));
                zCurve.AddKey(new Keyframe(audioBlock.rTime, audioBlock.z));

                animationClip.SetCurve("", typeof(Transform), "localPosition.x", xCurve);
                animationClip.SetCurve("", typeof(Transform), "localPosition.y", yCurve);
                animationClip.SetCurve("", typeof(Transform), "localPosition.z", zCurve);

                anim.PlayQueued(animationClip.name, QueueMode.PlayNow, PlayMode.StopAll);
                channelFormat.audioBlocks.Remove(audioBlock);
            }
            newThread.Resume();
        }


    }
    void Update()
    {
        addAnimations();
    }

}
