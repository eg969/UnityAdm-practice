//
//  AudioBlockHoa.h

#pragma once
#include "AdmReader.h"
struct AudioHoaBlock
{
    bool newBlockFlag;
    char name[100];
    int objId;
    int cfId;
    int blockId;
    int typeDef;
    float rTime;
    float duration;
    int channelNum;
    int order;
    int degree;
    float nfcRefDist;
    int screenRef;
    char normalization[100];
    char equation[100];
};

AudioHoaBlock loadHoaBlock(adm::AudioBlockFormatHoa hoaBlock);
