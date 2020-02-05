using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
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
            public bool newBlockFlag;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] name;
            public int cfId;
            public int blockId;
            public float rTime;
            public float x;
            public float y;
            public float z;
        };


    public struct UnityAudioBlock
    {
        public string name;
        public int cfId;
        public int blockId;
        public float rTime;
        public float x;
        public float y;
        public float z;
    };

    public struct UnityAudioChannelFormat
    {
        public int cfId;
        public List<UnityAudioBlock> audioBlocks;
    };

    public static List<UnityAudioChannelFormat> channelFormats = new List<UnityAudioChannelFormat>();

    public static void readFile()
    {
        readAdm();
    }

    public static void getNewBlock()
    {
        while (true)
        {
            CAudioBlock nextBlock = getNextBlock();
            if (nextBlock.newBlockFlag)
            {
                UnityAudioBlock newBlock;
                newBlock.name = Encoding.ASCII.GetString(nextBlock.name);
                newBlock.cfId = nextBlock.cfId;
                newBlock.blockId = nextBlock.blockId;
                newBlock.rTime = nextBlock.rTime;
                newBlock.x = nextBlock.x;
                newBlock.y = nextBlock.y;
                newBlock.z = nextBlock.z;

                foreach (UnityAudioChannelFormat channelFormat in channelFormats)
                {
                    if (channelFormat.cfId == newBlock.cfId)
                    {
                        channelFormat.audioBlocks.Add(newBlock);
                        return;
                    }
                }
                UnityAudioChannelFormat newChannelFormat;
                newChannelFormat.cfId = newBlock.cfId;
                newChannelFormat.audioBlocks = new List<UnityAudioBlock>();
                newChannelFormat.audioBlocks.Add(newBlock);
                channelFormats.Add(newChannelFormat);
            }
        }
    }

    public static long nanoTime()
    {
        long nano = 10000L * Stopwatch.GetTimestamp();
        nano /= TimeSpan.TicksPerMillisecond;
        nano *= 100L;
        return nano;
    }
}