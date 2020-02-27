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
            for(int cfIndex = 0; cfIndex < channelFormats.Count; cfIndex++)
            {
                if (!audioObjects.ContainsKey(channelFormats[cfIndex].cfId))
                {
                    GameObject audioObjectInstance = Instantiate(objectInstance) as GameObject;
                    audioObjectInstance.name = channelFormats[cfIndex].name;
                    audioObjects.Add(channelFormats[cfIndex].cfId, audioObjectInstance);
                }

                doPositionFor(cfIndex);
            }
        }
    }

    void doPositionFor(int cfIndex)
    {
        float timeSnapshot = Time.fixedTime;

        while (channelFormats[cfIndex].currentAudioBlocksIndex < channelFormats[cfIndex].audioBlocks.Count)
        {
            UnityAudioBlock audioBlock = channelFormats[cfIndex].audioBlocks[channelFormats[cfIndex].currentAudioBlocksIndex];
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
                    audioObjects[channelFormats[cfIndex].cfId].transform.position = newPos;
                    break;
                }
                else
                {
                    // It has ended. Make sure we set the position to it's final resting place.
                    audioObjects[channelFormats[cfIndex].cfId].transform.position = audioBlock.endPos;
                    // Now increment currentAudioBlocksIndex and re-evaluate the while - we might have already started the next block.
                    channelFormats[cfIndex].currentAudioBlocksIndex++;
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
