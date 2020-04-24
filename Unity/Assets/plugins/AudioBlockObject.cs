using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static AudioBlockInterface;
using static AudioBlockWrapper;

public class AudioBlockObject
{

    public static Dictionary<int, UnityAudioChannelFormat> channelFormats = new Dictionary<int, UnityAudioChannelFormat>();
    public static List<int> activeChannelFormats = new List<int>();
    public static Dictionary<int, GameObject> audioObjects = new Dictionary<int, GameObject>();
    

    public class UnityAudioChannelFormat
    {
        public string name;
        public int cfId;
        public List<UnityAudioBlock> audioBlocks;
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

        while (channelFormats[cfId].currentAudioBlocksIndex < channelFormats[cfId].audioBlocks.Count)
        {
            //lock
            UnityAudioBlock audioBlock = channelFormats[cfId].audioBlocks[channelFormats[cfId].currentAudioBlocksIndex];
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
                    audioObjects[cfId].transform.position = audioBlock.endPos;
                    if (audioObjects[cfId].GetComponent<AudioSource>()) audioObjects[cfId].GetComponent<AudioSource>().volume = audioBlock.endGain;
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

    public static void updateAudioBlockObjects(GameObject objectInstance)
    {
        List<int> cfIdsToProcess;

        lock (activeChannelFormats)
        {
            cfIdsToProcess = new List<int>(activeChannelFormats);
        }

        foreach (int cfId in cfIdsToProcess)
        {
            if (!audioObjects.ContainsKey(cfId))
            {
                GameObject audioObjectInstance = UnityEngine.Object.Instantiate(objectInstance) as GameObject;
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

    public static void getAudioBlockObjects(CAudioBlock nextBlock)
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


            UnityAudioBlock newBlock = new UnityAudioBlock
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

            if (!channelFormats.ContainsKey(nextBlock.cfId))
            {
                lock (channelFormats)
                {
                    channelFormats.Add(nextBlock.cfId,
                        new UnityAudioChannelFormat
                        {
                            name = Encoding.ASCII.GetString(nextBlock.name),
                            cfId = nextBlock.cfId,
                            audioBlocks = new List<UnityAudioBlock>(),
                            currentAudioBlocksIndex = 0,
                            channelNum = nextBlock.channelNum
                        }
                    );
                }

                lock (activeChannelFormats)
                {
                    activeChannelFormats.Add(nextBlock.cfId);
                }
            }

            if (channelFormats[nextBlock.cfId].audioBlocks.Count > 0)
            {
                UnityAudioBlock previousBlock = channelFormats[nextBlock.cfId].audioBlocks[channelFormats[nextBlock.cfId].audioBlocks.Count - 1];

                if (newBlock.blockId > previousBlock.blockId && newBlock.startTime >= previousBlock.endTime)
                {
                    newBlock.startPos.Set(previousBlock.endPos.x, previousBlock.endPos.y, previousBlock.endPos.z);
                    newBlock.startGain = previousBlock.endGain;
                }

            }

            lock (channelFormats)
            {
                channelFormats[nextBlock.cfId].audioBlocks.Add(newBlock);
            }
        }
    }
}