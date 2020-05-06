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
    float rTime;
    float duration;
    float x;
    float y;
    float z;
    float azimuth;
    float elevation;
    float distance;
    float azimuthMax;
    float elevationMax;
    float distanceMax;
    float azimuthMin;
    float elevationMin;
    float distanceMin;
    char verticalEdge[10];
    char horizontalEdge[10];
    int typeDef;
    int channelNum;
};

AudioSpeakerBlock loadSpeakerBlock(adm::AudioBlockFormatDirectSpeakers speakerBlock);
