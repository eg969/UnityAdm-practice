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
    Thread getBlocksThread;
    Dictionary<int, GameObject> audioObjects = new Dictionary<int, GameObject>();

    void Start()
    {
        getBlocksThread = new Thread(new ThreadStart(getBlocksLoop));
        getBlocksThread.Start();
    }

    void Update()
    {
        lock (lockObject)
        {
            foreach (var channelFormat in channelFormats)
            {
                if (!audioObjects.ContainsKey(channelFormat.Value.cfId))
                {
                    audioObjects.Add(channelFormat.Value.cfId, new GameObject(channelFormat.Value.name));
                }

                doPositionFor(channelFormat.Value.cfId);
            }
        }
    }

    void doPositionFor(int cfId)
    {
        float timeSnapshot = Time.fixedTime;

        while (channelFormats[cfId].currentAudioBlocksIndex < channelFormats[cfId].audioBlocks.Count)
        {
            UnityAudioBlock audioBlock = channelFormats[cfId].audioBlocks[channelFormats[cfId].currentAudioBlocksIndex];
            if (audioBlock.startTime <= timeSnapshot)
            {
                // This block has started. Has it ended?
                if(audioBlock.endTime > timeSnapshot)
                {
                    // We are in the block - find the interpolant (progress in to interpolation ramp)
                    float interpolant = (timeSnapshot - audioBlock.startTime) / audioBlock.duration;
                    audioObjects[cfId].transform.position = Vector3.Lerp(audioBlock.startPos, audioBlock.endPos, interpolant);
                    break;
                }
                else
                {
                    Debug.Log(cfId + " " + channelFormats[cfId].currentAudioBlocksIndex);
                    // It has ended. Make sure we set the position to it's final resting place.
                    audioObjects[cfId].transform.position = audioBlock.endPos;
                    // Now increment currentAudioBlocksIndex and re-evaluate the while - we might have already started the next block.
                    // TODO - can not modify from Dict - channelFormats[cfId].currentAudioBlocksIndex++;
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
