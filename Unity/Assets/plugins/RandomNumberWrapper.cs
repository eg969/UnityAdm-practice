using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class RandomNumberWrapper
{
#if UNITY_STANDALONE_OSX
    const string dll = "RandomNumberBUNDLE";
#elif UNITY_STANDALONE_WIN
    const string dll = " ";
#else
    const string dll = " ";
#endif
    [DllImport(dll)]
    private static extern int getRandom();

    public static int GetRandNum()
    { 
       return getRandom();
    }

}