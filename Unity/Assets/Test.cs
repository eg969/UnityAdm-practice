using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioBlockWrapper;
using System.Text;
using System;
using System.Threading;
using UnityEditor;

public class Test : MonoBehaviour
{
    public GameObject objectInstance;
    Thread newThread;
    Dictionary<int, GameObject> audioObjects = new Dictionary<int, GameObject>();

    void Start()
    {
        newThread = new Thread(new ThreadStart(getNewBlock));
        newThread.Start();
    }

    void addAnimations()
    {
        for(int i = 0; i < channelFormats.Count; i++)
        {
            if (!audioObjects.ContainsKey(channelFormats[i].cfId))
            {
                GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
                audioObjects.Add(channelFormats[i].cfId, audioObjectInstance);
                audioObjects[channelFormats[i].cfId].AddComponent<Animation>();
                Animation objectAnimation = audioObjects[channelFormats[i].cfId].GetComponent<Animation>();
                AnimationClip clip = new AnimationClip();
                clip.name = channelFormats[i].cfId.ToString();
                clip.legacy = true;

                objectAnimation.AddClip(clip, clip.name);
                objectAnimation.Play(clip.name, PlayMode.StopSameLayer);
            }
            if (channelFormats[i].audioBlocks.Count != 0)
            {
                GameObject audioObject = audioObjects[channelFormats[i].cfId];
                Animation anim = audioObject.GetComponent<Animation>();
                AnimationClip animationClip = anim.GetClip(channelFormats[i].cfId.ToString());
                AnimationCurve xCurve = new AnimationCurve();
                AnimationCurve yCurve = new AnimationCurve();
                AnimationCurve zCurve = new AnimationCurve();

                

                while (channelFormats[i].audioBlocks.Count != 0)
                {

                    Keyframe xKey = new Keyframe(channelFormats[i].audioBlocks[0].rTime, channelFormats[i].audioBlocks[0].x);
                    Keyframe yKey = new Keyframe(channelFormats[i].audioBlocks[0].rTime, channelFormats[i].audioBlocks[0].y);
                    Keyframe zKey = new Keyframe(channelFormats[i].audioBlocks[0].rTime, channelFormats[i].audioBlocks[0].z);


                    xCurve.AddKey(xKey);
                    yCurve.AddKey(yKey);
                    zCurve.AddKey(zKey);
                    /*
                    AnimationUtility.SetKeyLeftTangentMode(xCurve, xCurve.keys.Length - 1, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyLeftTangentMode(yCurve, yCurve.keys.Length - 1, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyLeftTangentMode(zCurve, zCurve.keys.Length - 1, AnimationUtility.TangentMode.Constant);

                    AnimationUtility.SetKeyRightTangentMode(xCurve, xCurve.keys.Length - 1, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyRightTangentMode(yCurve, yCurve.keys.Length - 1, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyRightTangentMode(zCurve, zCurve.keys.Length - 1, AnimationUtility.TangentMode.Constant);
                    */

                    //Debug.Log("Clip len: " + animationClip.length);
                    

                    channelFormats[i].audioBlocks.RemoveAt(0);

                }

                animationClip.SetCurve("", typeof(Transform), "localPosition.x", xCurve);
                animationClip.SetCurve("", typeof(Transform), "localPosition.y", yCurve);
                animationClip.SetCurve("", typeof(Transform), "localPosition.z", zCurve);

                anim.PlayQueued(animationClip.name, QueueMode.PlayNow, PlayMode.StopAll);


            }

        }
    }
    void Update()
    {
        lock (lockObject)
        {
            addAnimations();
        }
    }

}
