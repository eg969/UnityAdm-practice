﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

using static AudioBlockInterface;

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
    public static extern int readAdm(byte[] filePath);

    [DllImport(dll, CharSet = CharSet.Ansi)]
    public static extern CAudioBlock getNextBlock();

    [DllImport(dll, CharSet = CharSet.Ansi)]
    public static extern IntPtr getLatestException();

    [DllImport(dll)]
    public static extern unsafe float* getAudioFrame(int startFrame, int bufferSize, int channelNum);

    [DllImport(dll)]
    public static extern int getSamplerate();

    [DllImport(dll)]
    public static extern int getNumberOfFrames();

}