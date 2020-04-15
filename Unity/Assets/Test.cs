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

    void Awake()
    {
        if (readFile("/Users/edgarsg/Desktop/panned_noise_adm.wav"))
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
                if (channelFormats[cfId].channelNum >= 0)
                {
                    audioObjectInstance.AddComponent<AudioSource>();
                    AudioSource audioSource = audioObjectInstance.GetComponent<AudioSource>();
                    audioSource.dopplerLevel = 0;
                    audioSource.clip = channelFormats[cfId].createAudioClip();
                    audioSource.spatialBlend = 1.0f;
                    audioSource.loop = false;
                    audioSource.playOnAwake = true;
                    audioSource.Play();
                }
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
                    float interpolant = 0 ;

                    if (audioBlock.jumpPosition)
                    {
                        if (audioBlock.startTime + audioBlock.interpolationLength > timeSnapshot)
                        {
                            interpolant = (timeSnapshot - audioBlock.startTime) / audioBlock.interpolationLength;
                        }
                        else
                        {
                            interpolant = 1f;
                        }
                    }
                    else
                    {
                        interpolant = (timeSnapshot - audioBlock.startTime) / audioBlock.duration;
                    }

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

                    if (audioObjects[cfId].GetComponent<AudioSource>())
                    {
                        float gainDiff = audioBlock.endGain - audioBlock.startGain;
                        float currenGain = audioBlock.startGain;
                        float newGain = currenGain + interpolant * gainDiff;
                        audioObjects[cfId].GetComponent<AudioSource>().volume = newGain;
                    }
                    break;
                }
                else
                {
                    // It has ended. Make sure we set the position to it's final resting place.
                    audioObjects[cfId].transform.position = audioBlock.endPos;
                    if (audioObjects[cfId].GetComponent<AudioSource>())audioObjects[cfId].GetComponent<AudioSource>().volume = audioBlock.endGain;
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
