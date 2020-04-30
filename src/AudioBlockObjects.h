
//  AudioBlockObjects.h

#include "AdmReader.h"
#pragma once

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
    float gain;
    int jumpPosition;
    int moveSpherically;
    int channelNum;
};

AudioObjectBlock loadObjectBlock(adm::AudioBlockFormatObjects objectBlock);
