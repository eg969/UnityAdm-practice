//
//  AudioBlockHoa.cpp
//  adm
//
//  Created by Edgars Grivcovs on 25/04/2020.
//

#include <stdio.h>
#include "AudioBlockHoa.h"

AudioHoaBlock loadHoaBlock(adm::AudioBlockFormatHoa hoaBlock)
{
    AudioHoaBlock currentBlock;
    
    //Initial values
    currentBlock.newBlockFlag = false;
    strcpy(currentBlock.name, std::string("").c_str());
    currentBlock.objId = 0;
    currentBlock.cfId = 0;
    currentBlock.blockId = 0;
    currentBlock.typeDef = -1;
    currentBlock.rTime = 0.0;
    currentBlock.duration = 0.0;
    currentBlock.channelNum = -1;
    ////////////////////////////////
    currentBlock.order = -1;
    currentBlock.degree = -1;
    currentBlock.numOfChannels = 0;
    currentBlock.nfcRefDist = 0.0;
    currentBlock.screenRef = 0;
    strcpy(currentBlock.normalization, std::string("").c_str());
    strcpy(currentBlock.equation, std::string("").c_str());
    
    
    if(hoaBlock.has<adm::Rtime>())currentBlock.rTime = hoaBlock.get<adm::Rtime>().get().count()/1000000000.0;
    if(hoaBlock.has<adm::Duration>())currentBlock.duration = hoaBlock.get<adm::Duration>().get().count()/1000000000.0;
    if(hoaBlock.has<adm::Order>())currentBlock.order = hoaBlock.get<adm::Order>().get();
    if(hoaBlock.has<adm::Degree>())currentBlock.degree = hoaBlock.get<adm::Degree>().get();
    
    if(hoaBlock.has<adm::NfcRefDist>())currentBlock.nfcRefDist = hoaBlock.get<adm::NfcRefDist>().get();
    if(hoaBlock.has<adm::ScreenRef>())
    {
        if(hoaBlock.get<adm::ScreenRef>().get())
        {
            currentBlock.screenRef = 1;
        }
        else
        {
            currentBlock.screenRef = 0;
        }
    }
    if(hoaBlock.has<adm::Normalization>())strcpy(currentBlock.name, hoaBlock.get<adm::Normalization>().get().c_str());;
    if(hoaBlock.has<adm::Equation>())strcpy(currentBlock.name, hoaBlock.get<adm::Equation>().get().c_str());;
    
    currentBlock.newBlockFlag = true;
    currentBlock.cfId = std::stoi(adm::formatId(hoaBlock.get<adm::AudioBlockFormatId>()).substr(3,8), nullptr, 16);
    currentBlock.blockId = hoaBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();
    
    auto channelNums = getAdmReaderSingleton()->channelNums;
    auto typeDefs = getAdmReaderSingleton()->typeDefs;
    auto objectIds = getAdmReaderSingleton()->objectIds;
    auto hoaChannels = getAdmReaderSingleton()->hoaChannels;
    
    if(getAdmReaderSingleton()->getFromMap(objectIds, currentBlock.cfId).has_value())
    {
        currentBlock.objId = getAdmReaderSingleton()->getFromMap(objectIds, currentBlock.cfId).value();
    }
    
    if(getAdmReaderSingleton()->getFromMap(hoaChannels, currentBlock.objId).has_value())
    {
        currentBlock.numOfChannels = getAdmReaderSingleton()->getFromMap(hoaChannels, currentBlock.objId).value();
    }
    
    if(getAdmReaderSingleton()->getFromMap(channelNums, currentBlock.cfId).has_value())
    {
        currentBlock.channelNum = getAdmReaderSingleton()->getFromMap(channelNums, currentBlock.cfId).value();
    }
    
    if(getAdmReaderSingleton()->getFromMap(typeDefs, currentBlock.cfId).has_value())
    {
        currentBlock.typeDef = getAdmReaderSingleton()->getFromMap(typeDefs, currentBlock.cfId).value();
    }
    
    
    return currentBlock;
}
