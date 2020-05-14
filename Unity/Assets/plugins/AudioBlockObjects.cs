﻿using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static AudioBlockInterface;
using static AudioBlockWrapper;
using static BlockTypes;

public class AudioBlockObjects
{

    public static Dictionary<int, UnityObjectsChannelFormat> objectChannelFormats = new Dictionary<int, UnityObjectsChannelFormat>();
    public static List<int> activeObjectChannelFormats = new List<int>();
    public static Dictionary<int, GameObject> audioObjects = new Dictionary<int, GameObject>();
    

    public class UnityObjectsChannelFormat
    {
        public string name;
        public int cfId;
        public List<UnityObjectsAudioBlock> audioBlocks;
        public int currentAudioBlocksIndex;
        public int channelNum;
        private int position = 0;

        public AudioClip createAudioClip()
        {
            int samplerate = getSamplerate();
            int frameSize = getNumberOfFrames();

            AudioClip clip = AudioClip.Create(name, frameSize, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
            return clip;
        }

        unsafe void OnAudioRead(float[] data)
        {
            float* bufferCounter = getAudioFrame(position, data.Length, channelNum);

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = *bufferCounter;

                bufferCounter++;
                position++;
            }

        }

        void OnAudioSetPosition(int newPosition)
        {
            position = newPosition;

        }
    }

    public static void doPositionFor(int cfId)
    {
        float timeSnapshot = Time.fixedTime;

        while (objectChannelFormats[cfId].currentAudioBlocksIndex < objectChannelFormats[cfId].audioBlocks.Count)
        {
            //lock
            UnityObjectsAudioBlock audioBlock = objectChannelFormats[cfId].audioBlocks[objectChannelFormats[cfId].currentAudioBlocksIndex];
            if (audioBlock.startTime <= timeSnapshot)
            {
                // This block has started. Has it ended?
                if (audioBlock.endTime > timeSnapshot)
                {
                    float interpolant = 0;

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
                    UnityObjectsAudioBlock nextAudioBlock = new UnityObjectsAudioBlock();
                    if (objectChannelFormats[cfId].audioBlocks.Count > objectChannelFormats[cfId].currentAudioBlocksIndex + 1)
                    {
                        nextAudioBlock = objectChannelFormats[cfId].audioBlocks[objectChannelFormats[cfId].currentAudioBlocksIndex + 1];
                    }
                    if (nextAudioBlock.endTime >= timeSnapshot)
                    {
                        audioObjects[cfId].transform.position = audioBlock.endPos;
                        if (audioObjects[cfId].GetComponent<AudioSource>()) audioObjects[cfId].GetComponent<AudioSource>().volume = audioBlock.endGain;
                    }

                    // Now increment currentAudioBlocksIndex and re-evaluate the while - we might have already started the next block.
                    objectChannelFormats[cfId].currentAudioBlocksIndex++;
                }

            }
            else
            {
                //The current block hasn't started yet. Do nothing.
                break;
            }

        }
    }

    public static void updateAudioBlockObjects(GameObject objectInstance)
    {
        List<int> cfIdsToProcess;

        lock (activeObjectChannelFormats)
        {
            cfIdsToProcess = new List<int>(activeObjectChannelFormats);
        }

        foreach (int cfId in cfIdsToProcess)
        {
            if (!audioObjects.ContainsKey(cfId))
            {
                GameObject audioObjectInstance = UnityEngine.Object.Instantiate(objectInstance) as GameObject;
                if (objectChannelFormats[cfId].channelNum >= 0)
                {
                    audioObjectInstance.AddComponent<AudioSource>();
                    AudioSource audioSource = audioObjectInstance.GetComponent<AudioSource>();
                    audioSource.dopplerLevel = 0;
                    audioSource.clip = objectChannelFormats[cfId].createAudioClip();
                    audioSource.spatialBlend = 1.0f;
                    audioSource.loop = false;
                    audioSource.playOnAwake = true;
                    audioSource.Play();

                }
                audioObjectInstance.name = objectChannelFormats[cfId].name;
                audioObjects.Add(objectChannelFormats[cfId].cfId, audioObjectInstance);
            }

            doPositionFor(cfId);
        }
    }

    public static void loadObjectsAudioBlock(AdmObjectsAudioBlock nextBlock)
    {
        if (nextBlock.newBlockFlag != 0)
        {
            // We assume the blocks will come through in order for optimisation purposes
            // (it's an ADM stipulation anyway, although might not be the case for S-ADM depending upon the transport mechanism used.)
            // We do simple checks to confirm this, and discard those blocks that don't continue on from existing blocks.

            bool jumpPos = false;
            bool moveSpher = false;

            if (nextBlock.jumpPosition != 0)
            {
                jumpPos = true;
            }
            else
            {
                jumpPos = false;
            }
            if (nextBlock.moveSpherically != 0)
            {
                moveSpher = true;
            }
            else
            {
                moveSpher = false;
            }

            UnityObjectsAudioBlock newBlock = new UnityObjectsAudioBlock
            {
                blockId = nextBlock.blockId,
                startTime = nextBlock.rTime,
                endTime = nextBlock.rTime + nextBlock.duration,
                duration = nextBlock.duration,
                interpolationLength = nextBlock.interpolationLength,
                // TODO: explain why x<-->z 
                startPos = new Vector3(nextBlock.x, nextBlock.z, nextBlock.y), // Will be overwritten if we find a previous block, otherwise (if first block) this is correct
                endPos = new Vector3(nextBlock.x, nextBlock.z, nextBlock.y),
                startGain = nextBlock.gain,
                endGain = nextBlock.gain,
                jumpPosition = jumpPos,
                moveSpherically = moveSpher
            };

            if (!objectChannelFormats.ContainsKey(nextBlock.cfId))
            {
                lock (objectChannelFormats)
                {
                    objectChannelFormats.Add(nextBlock.cfId,
                        new UnityObjectsChannelFormat
                        {
                            name = Encoding.ASCII.GetString(nextBlock.name),
                            cfId = nextBlock.cfId,
                            audioBlocks = new List<UnityObjectsAudioBlock>(),
                            currentAudioBlocksIndex = 0,
                            channelNum = nextBlock.channelNum
                        }
                    );
                }

                lock (activeObjectChannelFormats)
                {
                    activeObjectChannelFormats.Add(nextBlock.cfId);
                }
            }

            if (objectChannelFormats[nextBlock.cfId].audioBlocks.Count > 0)
            {
                UnityObjectsAudioBlock previousBlock = objectChannelFormats[nextBlock.cfId].audioBlocks[objectChannelFormats[nextBlock.cfId].audioBlocks.Count - 1];

                if (newBlock.blockId > previousBlock.blockId && newBlock.startTime >= previousBlock.endTime)
                {
                    newBlock.startPos.Set(previousBlock.endPos.x, previousBlock.endPos.y, previousBlock.endPos.z);
                    newBlock.startGain = previousBlock.endGain;
                }

            }

            lock (objectChannelFormats)
            {
                objectChannelFormats[nextBlock.cfId].audioBlocks.Add(newBlock);
            }
        }
    }
}