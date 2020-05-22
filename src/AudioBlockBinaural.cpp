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
    currentBlock.objId = 0;
    currentBlock.cfId = 0;
    currentBlock.blockId = 0;
    currentBlock.typeDef = -1;
    currentBlock.rTime = 0.0;
    currentBlock.duration = 0.0;
    currentBlock.channelNum = -1;
    
    currentBlock.newBlockFlag = true;
    currentBlock.cfId = std::stoi(adm::formatId(binauralBlock.get<adm::AudioBlockFormatId>()).substr(3,8), nullptr, 16);
    currentBlock.blockId = binauralBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();
    
    
    if(binauralBlock.has<adm::Rtime>())currentBlock.rTime = binauralBlock.get<adm::Rtime>().get().count()/1000000000.0;
    if(binauralBlock.has<adm::Duration>())currentBlock.duration = binauralBlock.get<adm::Duration>().get().count()/1000000000.0;
    
    auto channelNums = AdmReaderSingleton::getInstance()->channelNums;
    auto typeDefs = AdmReaderSingleton::getInstance()->typeDefs;
    auto objectIds = AdmReaderSingleton::getInstance()->objectIds;
    
    if(AdmReaderSingleton::getInstance()->getFromMap(objectIds, currentBlock.cfId).has_value())
    {
        currentBlock.objId = AdmReaderSingleton::getInstance()->getFromMap(objectIds, currentBlock.cfId).value();
    }
    
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
