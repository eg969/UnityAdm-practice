//
//  AudioBlockHoa.h

#include "AdmReader.h"
#pragma once

struct AudioHoaBlock
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

AudioHoaBlock loadHoaBlock(adm::AudioBlockFormatHoa hoaBlock);
