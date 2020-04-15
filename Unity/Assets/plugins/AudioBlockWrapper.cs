using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;


public class AudioBlockWrapper
{
#if UNITY_STANDALONE_OSX
    const string dll = "bw64_and_adm";
#elif UNITY_STANDALONE_WIN
        const string dll = " ";
#else
        const string dll = " ";
#endif

    [DllImport(dll)]
    private static extern int readAdm(byte[] filePath);

    [DllImport(dll, CharSet = CharSet.Ansi)]
    private static extern CAudioBlock getNextBlock();

    [DllImport(dll, CharSet = CharSet.Ansi)]
    private static extern IntPtr getLatestException();

    [DllImport(dll)]
    private static extern unsafe float* getAudioFrame(int startFrame, int bufferSize, int channelNum);

    [DllImport(dll)]
    private static extern int getSamplerate();

    [DllImport(dll)]
    private static extern int getNumberOfFrames();

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CAudioBlock
    {
        public byte newBlockFlag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] name;
        public int cfId;
        public int blockId;
        public int typeDef;
        public float rTime;
        public float duration;
        public float interpolationLength;
        public float x;
        public float y;
        public float z;
        public float gain;
        public int jumpPosition;
        public int moveSpherically;
        public int channelNum;
    };

    public class UnityAudioBlock
    {
        public int blockId;
        public float startTime;
        public float endTime;
        public float duration;
        public float interpolationLength;
        public Vector3 startPos;
        public Vector3 endPos;
        public float startGain;
        public float endGain;
        public bool jumpPosition;
        public bool moveSpherically;

    };

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

    public static Dictionary<int, UnityAudioChannelFormat> channelFormats = new Dictionary<int, UnityAudioChannelFormat>();
    public static List<int> activeChannelFormats = new List<int>();

    public static bool readFile(string filePath)
    {
        byte[] byteArray;
        byteArray = Encoding.ASCII.GetBytes(filePath + '\0');

        if (readAdm(byteArray) == 0)
        {
            return true;
        }
        else
        {
            string ansi = Marshal.PtrToStringAnsi(getLatestException());
            UnityEngine.Debug.Log(ansi);
            return false;
        }
    }

    public static void getBlocksLoop()
    {
        while (true)
        {
            CAudioBlock nextBlock = getNextBlock();

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

            Thread.Sleep(20);
        }
    }
}