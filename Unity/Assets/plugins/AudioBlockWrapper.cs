using System;
using System.Runtime.InteropServices;


using static BlockTypes;

public class AudioBlockWrapper
{
    const string dll = "bw64_and_adm";

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