using UnityEngine;
using System.Runtime.InteropServices;


public static class BlockTypes
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct AdmObjectsAudioBlock
    {
        public byte newBlockFlag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] name;
        public int objId;
        public int cfId;
        public int blockId;
        public int typeDef;
        public float rTime;
        public float duration;
        public float interpolationLength;
        public float x;
        public float y;
        public float z;
        public int importance;
        public float width;
        public float height;
        public float depth;
        public float diffuse;
        public float divergence;
        public float maxDistance;
        public float positionRange;
        public float azimuthRange;
        public int channelLock;
        public float gain;
        public int jumpPosition;
        public int moveSpherically;
        public int channelNum;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct AdmHoaAudioBlock
    {
        public byte newBlockFlag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] name;
        public int objId;
        public int cfId;
        public int blockId;
        public int typeDef;
        public float rTime;
        public float duration;
        public int channelNum;
        public int order;
        public int degree;
        public int numOfChannels;
        public float nfcRefDist;
        public int screenRef;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] normalization;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] equation;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct AdmSpeakerAudioBlock
    {
        public byte newBlockFlag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] name;
        public int objId;
        public int cfId;
        public int blockId;
        public float rTime;
        public float duration;
        public float x;
        public float y;
        public float z;
        public float azimuth;
        public float elevation;
        public float distance;
        public float azimuthMax;
        public float elevationMax;
        public float distanceMax;
        public float azimuthMin;
        public float elevationMin;
        public float distanceMin;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] verticalEdge;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] horizontalEdge;
        public int typeDef;
        public int channelNum;

    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct AdmBinauralAudioBlock
    {
        public byte newBlockFlag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public byte[] name;
        public int objId;
        public int cfId;
        public int blockId;
        public int typeDef;
        public float rTime;
        public float duration;
        public int channelNum;

    };

    public class UnityObjectsAudioBlock
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

    public class UnityHoaAudioBlock
    {
        public int blockId;
        public float startTime;
        public float endTime;
        public float duration;
    };

    public struct UnitySpeakerAudioBlock
    {
        public int blockId;
        public float startTime;
        public float endTime;
        public float duration;
        public Vector3 startPos;
        public Vector3 endPos;
    };

    public struct UnityBinauralAudioBlock
    {
        public int blockId;
        public float startTime;
        public float endTime;
        public float duration;
    };
}
