using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

using static AudioBlockWrapper;
using static AudioBlockObject;

public class AudioBlockInterface
{
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
        while (true)
        {
            CAudioBlock nextBlock = getNextBlock();

            getAudioBlockObjects(nextBlock);
            Thread.Sleep(20);
        }
    }
}
