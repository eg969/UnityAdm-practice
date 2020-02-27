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
                    GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
                    audioObjectInstance.name = channelFormat.Value.name;
                    audioObjects.Add(channelFormat.Value.cfId, audioObjectInstance);
                }

                doPositionFor(channelFormat.Value.cfId);
            }
        }
    }

    void doPositionFor(int cfId)
    {
        float timeSnapshot = Time.fixedTime;
        int blockIndex = 0;

        while (blockIndex < channelFormats[cfId].audioBlocks.Count)
        {
            UnityAudioBlock audioBlock = channelFormats[cfId].audioBlocks[blockIndex];
            if (audioBlock.startTime <= timeSnapshot)
            {
                // This block has started. Has it ended?
                if(audioBlock.endTime > timeSnapshot)
                {
                    float interpolant = (timeSnapshot - audioBlock.startTime) / audioBlock.duration;
                    
                    // We are in the block - find the interpolant (progress in to interpolation ramp)
                    Vector3 newPos = Vector3.Lerp(audioBlock.startPos, audioBlock.endPos, interpolant);
                    /*if (cfId == 4097)
                    {
                        Debug.Log("Time: " + timeSnapshot + " CF: " + cfId + "  in block " + blockIndex + " Interp: " + interpolant +
                            "\n         x: " + newPos.x + " y: " + newPos.y + " z: " + newPos.z +
                        "\n         x: " + audioBlock.startPos.x + " y: " + audioBlock.startPos.y + " z: " + audioBlock.startPos.z +
                        "\n         x: " + audioBlock.endPos.x + " y: " + audioBlock.endPos.y + " z: " + audioBlock.endPos.z);
                        
                    }*/
                    audioObjects[cfId].transform.position = newPos;
                    break;
                }
                else
                {
                    // It has ended. Make sure we set the position to it's final resting place.
                    audioObjects[cfId].transform.position = audioBlock.endPos;
                    // Now increment currentAudioBlocksIndex and re-evaluate the while - we might have already started the next block.
                    blockIndex++;
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
