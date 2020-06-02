using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

using static BlockTypes;
using static AudioBlockWrapper;
using UnityEngine.Audio;
using UnityEditor;

public class AudioBlockHoa
{
    public static Dictionary<int, UnityHoaAudioClip> hoaAudioClips = new Dictionary<int, UnityHoaAudioClip>();
    public static List<int> activeHoaAudioClips = new List<int>();
    public static Dictionary<int, AudioSource> hoaAudioSources = new Dictionary<int, AudioSource>();

    public class UnityHoaChannelFormat
    {
        public string name;
        public int cfId;
        public int clipId;
        public List<UnityHoaAudioBlock> audioBlocks;
        public int currentAudioBlocksIndex;
        public int degree;
        public int acn;
        public int channelNum;
    }

    public class UnityHoaAudioClip
    {
        public int clipId;
        public int numberOfChannels;
        public Dictionary<int, UnityHoaChannelFormat> channelFormats;
        private int[] channelNums;
        private int position = 0;

        public AudioClip createAudioClip()
        {
            if (numberOfChannels == channelFormats.Count)
            {
                channelNums = new int[numberOfChannels];
                int samplerate = getSamplerate();
                int frameSize = getNumberOfFrames();
                channelNums = new int[numberOfChannels];

                foreach (var channelFormat in channelFormats.Values)
                {
                    channelNums[channelFormat.acn] = channelFormat.channelNum;
                }

                AudioClip clip = AudioClip.Create(clipId.ToString(), frameSize, numberOfChannels, samplerate, true, OnAudioRead, OnAudioSetPosition);
                return clip;
            }
            else
            {
                return null;
            }
        }

        unsafe void OnAudioRead(float[] data)
        {


            float* bufferCounter = getHoaAudioFrame(position, data.Length, channelNums, numberOfChannels);

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


    public static void updateAudioBlockHoa(GameObject hoaVisualisation)
    {
        List<int> clipIdsToProcess;

        lock (activeHoaAudioClips)
        {
            clipIdsToProcess = new List<int>(activeHoaAudioClips);
        }

        foreach (int clipId in clipIdsToProcess)
        {
            if (!hoaAudioSources.ContainsKey(clipId))
            {
                if (hoaAudioClips.ContainsKey(clipId))
                {
                    if (hoaAudioClips[clipId].numberOfChannels == hoaAudioClips[clipId].channelFormats.Count)
                    {
                        GameObject hoaAudioSourceInstance;
                        if (hoaVisualisation)
                        {
                            hoaAudioSourceInstance = UnityEngine.Object.Instantiate(hoaVisualisation) as GameObject;
                        }
                        else
                        {
                            hoaAudioSourceInstance = new GameObject();
                        }
                        hoaAudioSourceInstance.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
                        hoaAudioSourceInstance.AddComponent<AudioSource>();
                        AudioSource audioSource = hoaAudioSourceInstance.GetComponent<AudioSource>();
                        AudioMixer mixer = Resources.Load("ResonanceAudioMixer") as AudioMixer;
                        var test = mixer.FindMatchingGroups("Master")[0];
                        audioSource.outputAudioMixerGroup = test;
                        audioSource.dopplerLevel = 0;
                        AudioClip clip = hoaAudioClips[clipId].createAudioClip();
                        //clip.name = clipId.ToString();
                        audioSource.clip = clip;
                        audioSource.spatialBlend = 0.0f;
                        audioSource.loop = false;
                        audioSource.playOnAwake = true;
                        audioSource.Play();
                        hoaAudioSources.Add(hoaAudioClips[clipId].clipId, audioSource);
                        var clipTest = audioSource.GetComponent<AudioClip>();
                        //string path = AssetDatabase.GetAssetPath(clipTest);
                        //AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
                        //audioImporter.ambisonic = true;
                        //AssetDatabase.ImportAsset(path);
                    }
                }
            }
        }
    }

    public static void loadHoaAudioBlock(AdmHoaAudioBlock nextBlock)
    {
        if (nextBlock.newBlockFlag != 0)
        {
            // We assume the blocks will come through in order for optimisation purposes
            // (it's an ADM stipulation anyway, although might not be the case for S-ADM depending upon the transport mechanism used.)
            // We do simple checks to confirm this, and discard those blocks that don't continue on from existing blocks.

            UnityHoaAudioBlock newBlock = new UnityHoaAudioBlock
            {
                blockId = nextBlock.blockId,
                startTime = nextBlock.rTime,
                endTime = nextBlock.rTime + nextBlock.duration,
                duration = nextBlock.duration
            };

            if(!hoaAudioClips.ContainsKey(nextBlock.objId))
            {
                hoaAudioClips.Add(nextBlock.objId,
                        new UnityHoaAudioClip
                        {
                            clipId = nextBlock.objId,
                            numberOfChannels = nextBlock.numOfChannels,
                            channelFormats = new Dictionary<int, UnityHoaChannelFormat>()
                        }
                    );
            }

            lock (activeHoaAudioClips)
            {
                activeHoaAudioClips.Add(nextBlock.objId);
            }


            if (!hoaAudioClips[nextBlock.objId].channelFormats.ContainsKey(nextBlock.cfId))
            {
                lock (hoaAudioClips)
                {
                    hoaAudioClips[nextBlock.objId].channelFormats.Add(nextBlock.cfId,
                        new UnityHoaChannelFormat
                        {
                            name = Encoding.ASCII.GetString(nextBlock.name),
                            cfId = nextBlock.cfId,
                            audioBlocks = new List<UnityHoaAudioBlock>(),
                            currentAudioBlocksIndex = 0,
                            channelNum = nextBlock.channelNum,
                            acn = nextBlock.order * nextBlock.order + nextBlock.order + nextBlock.degree,
                            degree = nextBlock.degree,
                            clipId = nextBlock.objId
                        }
                    );
                }

                lock (activeHoaAudioClips)
                {
                    activeHoaAudioClips.Add(nextBlock.cfId);
                }
            }

            if (hoaAudioClips[nextBlock.objId].channelFormats[nextBlock.cfId].audioBlocks.Count > 0)
            {
                UnityHoaAudioBlock previousBlock = hoaAudioClips[nextBlock.objId].channelFormats[nextBlock.cfId].audioBlocks[hoaAudioClips[nextBlock.objId].channelFormats[nextBlock.cfId].audioBlocks.Count - 1];

                if (newBlock.blockId > previousBlock.blockId && newBlock.startTime >= previousBlock.endTime)
                {
                    //Initializing next block
                }

            }
        }
    }
}