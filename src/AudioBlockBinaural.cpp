//
//  AudioBlockBinaural.cpp
//  adm
//
//  Created by Edgars Grivcovs on 25/04/2020.
//

#include <stdio.h>

#include "AudioBlockBinaural.h"

AudioBinauralBlock loadBinauralBlock(adm::AudioBlockFormatBinaural binauralBlock)
{
    AudioBinauralBlock currentBlock;
    
    currentBlock.newBlockFlag = false;
    strcpy(currentBlock.name, std::string("").c_str());
    currentBlock.cfId = 0;
    currentBlock.blockId = 0;
    currentBlock.typeDef = -1;
    currentBlock.rTime = 0.0;
    currentBlock.duration = 0.0;
    currentBlock.channelNum = -1;
    
    currentBlock.cfId = binauralBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
    currentBlock.blockId = binauralBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();
    
    auto channelNums = AdmReaderSingleton::getInstance()->channelNums;
    auto typeDefs = AdmReaderSingleton::getInstance()->typeDefs;
    
    if(AdmReaderSingleton::getInstance()->getFromMap(channelNums, currentBlock.cfId).has_value())
    {
        currentBlock.channelNum = AdmReaderSingleton::getInstance()->getFromMap(channelNums, currentBlock.cfId).value();
    }
    
    if(AdmReaderSingleton::getInstance()->getFromMap(typeDefs, currentBlock.cfId).has_value())
    {
        currentBlock.typeDef = AdmReaderSingleton::getInstance()->getFromMap(typeDefs, currentBlock.cfId).value();
    }
    
    return currentBlock;
}
