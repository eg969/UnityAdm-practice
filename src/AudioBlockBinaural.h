//
//  AudioBlockBinaural.h

#include "AdmReader.h"
#pragma once

struct AudioBinauralBlock
{
    bool newBlockFlag;
    char name[100];
    int cfId;
    int blockId;
    int typeDef;
    float rTime;
    float duration;
    int channelNum;
};

AudioBinauralBlock loadBinauralBlock(adm::AudioBlockFormatBinaural binauralBlock);
