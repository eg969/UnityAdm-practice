﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

using static AudioBlockWrapper;
using static AudioBlockObjects;
using static BlockTypes;

public class AudioBlockInterface
{
    //Thread getBlocksThread;

    public static bool readFile(string filePath)
    {
        byte[] byteArray;
        byteArray = Encoding.ASCII.GetBytes(filePath + '\0');

        if (readAdm(byteArray) == 0)
        {
            Thread getBlocksThread = new Thread(new ThreadStart(getBlocksLoop));
            getBlocksThread.Start();
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
        //int i = 0;
        while (true)
        {

            AdmObjectsAudioBlock nextBlock = getNextObjectBlock();

            loadObjectsAudioBlock(nextBlock);
            Thread.Sleep(20);

            /*if (i < 5)
            {
                Thread.Sleep(1000);
            }
            else
            {
                Thread.Sleep(20);
            }
            i++;*/
        }
    }
}
