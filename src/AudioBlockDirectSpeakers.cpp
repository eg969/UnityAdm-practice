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
    currentBlock.objId = 0;
    currentBlock.cfId = 0;
    currentBlock.blockId = 0;
    currentBlock.typeDef = -1;
    currentBlock.rTime = 0.0;
    currentBlock.duration = 0.0;
    currentBlock.channelNum = -1;
    currentBlock.x = 0.0;
    currentBlock.y = 0.0;
    currentBlock.z = 0.0;
    currentBlock.azimuth = 0.0;
    currentBlock.elevation = 0.0;
    currentBlock.distance = 0.0;
    currentBlock.azimuthMax= 0.0;
    currentBlock.elevationMax = 0.0;
    currentBlock.distanceMax = 0.0;
    currentBlock.azimuthMin = 0.0;
    currentBlock.elevationMin = 0.0;
    currentBlock.distanceMin= 0.0;
    strcpy(currentBlock.verticalEdge, std::string("").c_str());
    strcpy(currentBlock.horizontalEdge, std::string("").c_str());

    
    if(speakerBlock.has<adm::Rtime>())currentBlock.rTime = speakerBlock.get<adm::Rtime>().get().count()/1000000000.0;
    if(speakerBlock.has<adm::Duration>())currentBlock.duration = speakerBlock.get<adm::Duration>().get().count()/1000000000.0;
    
    if(speakerBlock.has<adm::SpeakerPosition>())
    {
        auto position = speakerBlock.get<adm::SpeakerPosition>();
        float azimuth = 0.0;
        float elevation = 0.0;
        float distance = 0.0;
        
        if(position.has<adm::Azimuth>())azimuth = position.get<adm::Azimuth>().get();
        if(position.has<adm::Elevation>())elevation = position.get<adm::Elevation>().get();
        if(position.has<adm::Distance>())distance = position.get<adm::Distance>().get();
        
        currentBlock.azimuth =  azimuth;
        currentBlock.elevation =  elevation;
        currentBlock.distance = distance;
        
        if(position.has<adm::Azimuth>() && position.has<adm::Elevation>())
        {
            float x = distance * sin(-TO_RAD * azimuth) * cos(TO_RAD * elevation);
            currentBlock.x = x;
        }
        if(position.has<adm::Elevation>() && position.has<adm::Azimuth>())
        {
            float y = distance * cos(TO_RAD * elevation) * cos(TO_RAD * azimuth);
            currentBlock.y = y;
        }
        if(position.has<adm::Elevation>() && position.has<adm::Distance>())
        {
            float z = distance * sin(TO_RAD * elevation);
            currentBlock.z = z;
        }
        
        if(position.has<adm::AzimuthMax>())currentBlock.azimuthMax = position.get<adm::AzimuthMax>().get();
        if(position.has<adm::ElevationMax>())currentBlock.elevationMax = position.get<adm::ElevationMax>().get();
        if(position.has<adm::DistanceMax>())currentBlock.distanceMax = position.get<adm::DistanceMax>().get();
        if(position.has<adm::AzimuthMin>())currentBlock.azimuthMin = position.get<adm::AzimuthMin>().get();
        if(position.has<adm::ElevationMax>())currentBlock.elevationMin = position.get<adm::ElevationMin>().get();
        if(position.has<adm::DistanceMax>())currentBlock.distanceMin = position.get<adm::DistanceMin>().get();
        
        if(position.has<adm::ScreenEdgeLock>())
        {
            auto edgeLock = position.get<adm::ScreenEdgeLock>();
            
            
            if(edgeLock.has<adm::VerticalEdge>()) strcpy(currentBlock.verticalEdge, edgeLock.get<adm::VerticalEdge>().get().c_str());
            if(edgeLock.has<adm::HorizontalEdge>()) strcpy(currentBlock.horizontalEdge, edgeLock.get<adm::HorizontalEdge>().get().c_str());
        }
        
    }
    
    currentBlock.newBlockFlag = true;
    currentBlock.cfId = std::stoi(adm::formatId(speakerBlock.get<adm::AudioBlockFormatId>()).substr(3,8), nullptr, 16);
    currentBlock.blockId = speakerBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();
    
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
