//
//  AudioBlockDirectSpeakers.h

#pragma once
#include "AdmReader.h"
struct AudioSpeakerBlock
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

AudioSpeakerBlock loadSpeakerBlock(adm::AudioBlockFormatDirectSpeakers speakerBlock);
