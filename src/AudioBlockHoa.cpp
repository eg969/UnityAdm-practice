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
    currentBlock.cfId = 0;
    currentBlock.blockId = 0;
    currentBlock.typeDef = -1;
    currentBlock.rTime = 0.0;
    currentBlock.duration = 0.0;
    currentBlock.channelNum = -1;
    ////////////////////////////////
    currentBlock.order = -1;
    currentBlock.degree = -1;
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
    
    currentBlock.cfId = hoaBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
    currentBlock.blockId = hoaBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();
    
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
