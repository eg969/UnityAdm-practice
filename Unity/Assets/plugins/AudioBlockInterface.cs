using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

using static AudioBlockWrapper;
using static AudioBlockObjects;
using static AudioBlockHoa;
using static BlockTypes;

public class AudioBlockInterface
{
    private List<AdmObjectsAudioBlock> objectsBlocks = new List<AdmObjectsAudioBlock>();
    private List<AdmHoaAudioBlock> hoaBlocks = new List<AdmHoaAudioBlock>();
    private List<AdmSpeakerAudioBlock> speakerBlocks = new List<AdmSpeakerAudioBlock>();
    private List<AdmBinauralAudioBlock> binauralBlocks = new List<AdmBinauralAudioBlock>();

    public static bool readFileForUnityAudio(string filePath)
    {
        byte[] byteArray;
        byteArray = Encoding.ASCII.GetBytes(filePath + '\0');

        if (readAdm(byteArray) == 0)
        {
            Thread getBlocksThread = new Thread(new ThreadStart(getBlocksForUnityLoop));
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

    public static void getBlocksForUnityLoop()
    {
        //int i = 0;
        while (true)
        {
            AdmObjectsAudioBlock nextObjectBlock = getNextObjectBlock();
            AdmHoaAudioBlock nextHoaBlock = getNextHoaBlock();
            AdmSpeakerAudioBlock nextSpeakerBlock = getNextSpeakerBlock();
            AdmBinauralAudioBlock nextBinauralBlock = getNextBinauralBlock();


            loadObjectsAudioBlock(nextObjectBlock);
            loadHoaAudioBlock(nextHoaBlock);
            Thread.Sleep(20);
        }
    }

    public static bool readFileForBear(string filePath)
    {
        byte[] byteArray;
        byteArray = Encoding.ASCII.GetBytes(filePath + '\0');

        if (readAdm(byteArray) == 0)
        {
            Thread getBlocksThread = new Thread(new ThreadStart(getBlocksForBearLoop));
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

    public static void getBlocksForBearLoop()
    {
        while (true)
        {
            AdmObjectsAudioBlock nextObjectBlock = getNextObjectBlock();
            AdmHoaAudioBlock nextHoaBlock = getNextHoaBlock();
            AdmSpeakerAudioBlock nextSpeakerBlock = getNextSpeakerBlock();
            AdmBinauralAudioBlock nextBinauralBlock = getNextBinauralBlock();
            bool callProcess = true;


            while (callProcess)
            {
                if (addBearObjectBlock(nextObjectBlock) == 0 && addBearHoaBlock(nextHoaBlock) == 0 && addBearSpeakerBlock(nextSpeakerBlock) == 0 && addBearBinauralBlock(nextBinauralBlock) == 0)
                {
                    process();
                    callProcess = false;
                }
                else
                {
                    callProcess = true;
                }
            }

            Thread.Sleep(20);
        }
    }
}
