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
    private static extern void readAdm();

    [DllImport(dll, CharSet = CharSet.Ansi)]
    private static extern CAudioBlock getNextBlock();

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CAudioBlock
    {
        public byte newBlockFlag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] name;
        public int cfId;
        public int blockId;
        public float rTime;
        public float duration; // TODO: Implement in C++
        public float x;
        public float y;
        public float z;
    };

    public class UnityAudioBlock
    {
        public int blockId;
        public float startTime;
        public float endTime;
        public float duration;
        public Vector3 startPos;
        public Vector3 endPos;
    };

    public class UnityAudioChannelFormat
    {
        public string name;
        public int cfId;
        public List<UnityAudioBlock> audioBlocks;
        public int currentAudioBlocksIndex;
    };

    public static Dictionary<int, UnityAudioChannelFormat> channelFormats = new Dictionary<int, UnityAudioChannelFormat>();

    public static readonly object lockObject = new object();

    public static void readFile()
    {
        readAdm();
    }

    public static void getBlocksLoop()
    {
        readFile();
        while (true)
        {
            CAudioBlock nextBlock = getNextBlock();

            if (nextBlock.newBlockFlag != 0)
            {
                // We assume the blocks will come through in order for optimisation purposes
                // (it's an ADM stipulation anyway, although might not be the case for S-ADM depending upon the transport mechanism used.)
                // We do simple checks to confirm this, and discard those blocks that don't continue on from existing blocks.

                UnityAudioBlock newBlock = new UnityAudioBlock {
                    blockId = nextBlock.blockId,
                    startTime = nextBlock.rTime,
                    endTime = nextBlock.rTime + nextBlock.duration,
                    duration = nextBlock.duration,
                    startPos = new Vector3(nextBlock.x, nextBlock.y, nextBlock.z), // Will be overwritten if we find a previous block, otherwise (if first block) this is correct
                    endPos = new Vector3(nextBlock.x, nextBlock.y, nextBlock.z)
                };

                if (!channelFormats.ContainsKey(nextBlock.cfId))
                {
                    lock (lockObject)
                    {

                        channelFormats.Add(nextBlock.cfId,
                            new UnityAudioChannelFormat {
                                name = Encoding.ASCII.GetString(nextBlock.name),
                                cfId = nextBlock.cfId,
                                audioBlocks = new List<UnityAudioBlock>(),
                                currentAudioBlocksIndex = 0
                            }
                        );
                    }
                }

                if (channelFormats[nextBlock.cfId].audioBlocks.Count > 0) {
                    UnityAudioBlock previousBlock = channelFormats[nextBlock.cfId].audioBlocks[channelFormats[nextBlock.cfId].audioBlocks.Count - 1];
                    lock (lockObject)
                    {
                        if (newBlock.blockId > previousBlock.blockId && newBlock.startTime >= previousBlock.endTime)
                        {
                            newBlock.startPos.Set(previousBlock.endPos.x, previousBlock.endPos.y, previousBlock.endPos.z);
                            channelFormats[nextBlock.cfId].audioBlocks.Add(newBlock);
                        }
                    }
                }
                else
                {
                    lock (lockObject)
                    {
                        channelFormats[nextBlock.cfId].audioBlocks.Add(newBlock);
                    }
                }
            }

            Thread.Sleep(20);

        }

    }

}
