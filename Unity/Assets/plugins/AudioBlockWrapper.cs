using System;
using System.Runtime.InteropServices;


using static BlockTypes;

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
    public static extern AdmObjectsAudioBlock getNextObjectBlock();

    [DllImport(dll, CharSet = CharSet.Ansi)]
    public static extern AdmHoaAudioBlock getNextHoaBlock();

    [DllImport(dll, CharSet = CharSet.Ansi)]
    public static extern AdmSpeakerAudioBlock getNextSpeakerBlock();

    [DllImport(dll, CharSet = CharSet.Ansi)]
    public static extern AdmBinauralAudioBlock getNextBinauralBlock();

    [DllImport(dll, CharSet = CharSet.Ansi)]
    public static extern int addBearObjectBlock(AdmObjectsAudioBlock audioBlock);

    [DllImport(dll, CharSet = CharSet.Ansi)]
    public static extern int addBearHoaBlock(AdmHoaAudioBlock audioBlock);

    [DllImport(dll, CharSet = CharSet.Ansi)]
    public static extern int addBearSpeakerBlock(AdmSpeakerAudioBlock audioBlock);

    [DllImport(dll, CharSet = CharSet.Ansi)]
    public static extern int addBearBinauralBlock(AdmBinauralAudioBlock audioBlock);

    [DllImport(dll)]
    public static extern void process();

    [DllImport(dll, CharSet = CharSet.Ansi)]
    public static extern IntPtr getLatestException();

    [DllImport(dll)]
    public static extern unsafe float* getAudioFrame(int startFrame, int bufferSize, int channelNum);

    [DllImport(dll)]
    public static extern unsafe float* getHoaAudioFrame(int startFrame, int bufferSize, int[] channelNums, int numberOfChannels);

    [DllImport(dll)]
    public static extern int getSamplerate();

    [DllImport(dll)]
    public static extern int getNumberOfFrames();

}