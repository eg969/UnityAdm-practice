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
            [DllImport(dll, CharSet = CharSet.Ansi)]
            private static extern UnityAudioBlock getNextBlock(int blockIndex);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public struct UnityAudioBlock
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
                public byte[] name;
                public int azimuth;
                public int elevation;
                public int distance;
            };

            public static UnityAudioBlock getBlock(int blockIndex)
            {
                return getNextBlock(blockIndex);
            }
}