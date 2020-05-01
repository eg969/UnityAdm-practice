//
//  AudioBlockDirectSpeakers.cpp
//  adm
//
//  Created by Edgars Grivcovs on 25/04/2020.
//

#include <stdio.h>
#include "AudioBlockDirectSpeakers.h"

AudioSpeakerBlock loadSpeakerBlock(adm::AudioBlockFormatDirectSpeakers speakerBlock)
{
    AudioSpeakerBlock currentBlock;
    
    currentBlock.newBlockFlag = false;
    strcpy(currentBlock.name, std::string("").c_str());
    currentBlock.cfId = 0;
    currentBlock.blockId = 0;
    currentBlock.typeDef = -1;
    currentBlock.rTime = 0.0;
    currentBlock.duration = 0.0;
    currentBlock.channelNum = -1;
    
    
    currentBlock.cfId = speakerBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
    currentBlock.blockId = speakerBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();
    
    if(getFromMap(channelNums, currentBlock.cfId).has_value())
    {
        currentBlock.channelNum = getFromMap(channelNums, currentBlock.cfId).value();
    }
    
    if(getFromMap(typeDefs, currentBlock.cfId).has_value())
    {
        currentBlock.typeDef = getFromMap(typeDefs, currentBlock.cfId).value();
    }
    
    return currentBlock;
}
