using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        private static extern bool queryNextBlock(int cfIndex, int blockIndex);

        [DllImport(dll, CharSet = CharSet.Ansi)]
        private static extern UnityAudioBlock getNextBlock(int cfIndex, int blockIndex);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct UnityAudioBlock
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public byte[] name;
            public float rTime;
            public float x;
            public float y;
            public float z;
        };

        public static UnityAudioBlock getBlock(int cfIndex, int blockIndex)
        {
            return getNextBlock(cfIndex, blockIndex);
        }

        public static bool queryBlock(int cfIndex, int blockIndex)
        {
            return queryNextBlock(cfIndex, blockIndex);
        }
    }