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
    Thread getBlocksThread;
    Dictionary<int, GameObject> audioObjects = new Dictionary<int, GameObject>();
    string fileToRead = "/Users/edgarsg/Desktop/test3.wav";

    void Awake()
    {
        if (readFile(fileToRead))
        {
            getBlocksThread = new Thread(new ThreadStart(getBlocksLoop));
            getBlocksThread.Start();
        }
    }

    void Update()
    {
        float time = Time.fixedTime;
        List<int> cfIdsToProcess;

        lock (activeChannelFormats)
        {
            cfIdsToProcess = new List<int>(activeChannelFormats);
        }

        foreach (int cfId in cfIdsToProcess)
        {
            if (!audioObjects.ContainsKey(cfId))
            {
                GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
                audioObjectInstance.AddComponent<AudioSource>();
                AudioSource audioSource = audioObjectInstance.GetComponent<AudioSource>();
                audioSource.dopplerLevel = 0;
                audioSource.playOnAwake = true;
                audioSource.clip = channelFormats[cfId].createAudioClip();
                audioObjectInstance.name = channelFormats[cfId].name;
                audioObjects.Add(channelFormats[cfId].cfId, audioObjectInstance);
            }

            doPositionFor(cfId);
        }

    }

    void doPositionFor(int cfId)
    {
        float timeSnapshot = Time.fixedTime;

        while (channelFormats[cfId].currentAudioBlocksIndex < channelFormats[cfId].audioBlocks.Count)
        {
            //lock
            UnityAudioBlock audioBlock = channelFormats[cfId].audioBlocks[channelFormats[cfId].currentAudioBlocksIndex];
            if (audioBlock.startTime <= timeSnapshot)
            {
                // This block has started. Has it ended?
                if(audioBlock.endTime > timeSnapshot)
                {
                    float interpolant = (timeSnapshot - audioBlock.startTime) / audioBlock.duration;

                    // We are in the block - find the interpolant (progress in to interpolation ramp)
                    Vector3 newPos;
                    if (audioBlock.moveSpherically)
                    {
                        newPos = Vector3.Slerp(audioBlock.startPos, audioBlock.endPos, interpolant);
                    }
                    else
                    {
                        newPos = Vector3.Lerp(audioBlock.startPos, audioBlock.endPos, interpolant);
                    }
                    audioObjects[cfId].transform.position = newPos;
                    break;
                }
                else
                {
                    // It has ended. Make sure we set the position to it's final resting place.
                    audioObjects[cfId].transform.position = audioBlock.endPos;
                    // Now increment currentAudioBlocksIndex and re-evaluate the while - we might have already started the next block.
                    channelFormats[cfId].currentAudioBlocksIndex++;
                }

            }
            else
            {
                //The current block hasn't started yet. Do nothing.
                break;
            }

        }
    }

}
