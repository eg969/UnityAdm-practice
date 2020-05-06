
//  AudioBlockObjects.h

#pragma once
#include "AdmReader.h"
struct AudioObjectBlock
{
    bool newBlockFlag;
    char name[100];
    int cfId;
    int blockId;
    int typeDef;
    float rTime;
    float duration;
    float interpolationLength;
    float x;
    float y;
    float z;
    int importance;
    float width;
    float height;
    float depth;
    float diffuse;
    float divergence;
    float maxDistance;
    float positionRange;
    float azimuthRange;
    int channelLock;
    float gain;
    int jumpPosition;
    int moveSpherically;
    int channelNum;
};

AudioObjectBlock loadObjectBlock(adm::AudioBlockFormatObjects objectBlock);
