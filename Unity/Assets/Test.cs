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
                if (channelFormat.gameObject == null)
                {
                    channelFormat.gameObject = new GameObject(channelFormat.name);
                }

                doPositionFor(channelFormat);
            }
        }
    }

    Vector3 doPositionFor(UnityAudioChannelFormat* channelFormat)
    {
        float timeSnapshot = Time.fixedTime;

        while (channelFormat->currentAudioBlocksIndex < channelFormat->audioBlocks.Count)
        {
            UnityAudioBlock* audioBlock = channelFormat->audioBlocks[channelFormat->currentAudioBlocksIndex];
            if (audioBlock->startTime <= timeSnapshot)
            {
                // This block has started. Has it ended?
                if(audioBlock->endTime > timeSnapshot)
                {
                    // We are in the block - find the interpolant (progress in to interpolation ramp)
                    float interpolant = (timeSnapshot - audioBlock->startTime) / audioBlock->duration;
                    channelFormat->gameObject.transform.position = Vector3.Lerp(audioBlock->startPos, audioBlock->endPos, interpolant);
                    break;
                }
                else
                {
                    // It has ended. Make sure we set the position to it's final resting place.
                    channelFormat->gameObject.transform.position = audioBlock->endPosition;
                    // Now increment currentAudioBlocksIndex and re-evaluate the while - we might have already started the next block.
                    channelFormat->currentAudioBlocksIndex++;
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
