//
//  AudioBlockObjects.cpp
//  adm
//
//  Created by Edgars Grivcovs on 25/04/2020.
//

#include <stdio.h>
#include "AudioBlockObjects.h"


//  AudioBlockObjects.h


AudioObjectBlock loadObjectBlock(adm::AudioBlockFormatObjects objectBlock)
{
    AudioObjectBlock currentBlock;

    currentBlock.newBlockFlag = false;
    strcpy(currentBlock.name, std::string("").c_str());
    currentBlock.objId = 0;
    currentBlock.cfId = 0;
    currentBlock.blockId = 0;
    currentBlock.typeDef = -1;
    currentBlock.rTime = 0.0;
    currentBlock.duration = 0.0;
    currentBlock.interpolationLength = 0.0;
    currentBlock.x = 0.0;
    currentBlock.y = 0.0;
    currentBlock.z = 0.0;
    currentBlock.importance = 10;
    currentBlock.width = 0.0;
    currentBlock.height = 0.0;
    currentBlock.depth = 0.0;
    currentBlock.diffuse = 0.0;
    currentBlock.divergence = 0.0;
    currentBlock.maxDistance = 0.0;
    currentBlock.azimuthRange = 0.0;
    currentBlock.positionRange = 0.0;
    currentBlock.channelLock = 0;
    currentBlock.gain = 1.0;
    currentBlock.jumpPosition = 0;
    currentBlock.moveSpherically = 0;
    currentBlock.channelNum = -1;
   
    //if(objectBlock.has<adm::>())strcpy(currentBlock.name, objectBlock.get<adm::>().get().c_str());
    if(objectBlock.has<adm::Rtime>())currentBlock.rTime = objectBlock.get<adm::Rtime>().get().count()/1000000000.0;
    if(objectBlock.has<adm::Duration>())currentBlock.duration = objectBlock.get<adm::Duration>().get().count()/1000000000.0;
   
    if(objectBlock.has<adm::JumpPosition>())
    {
        if(objectBlock.get<adm::JumpPosition>().has<adm::JumpPositionFlag>())
        {
            if(objectBlock.get<adm::JumpPosition>().get<adm::JumpPositionFlag>().get())
            {
                currentBlock.jumpPosition = 1;
            }
            else
            {
                currentBlock.jumpPosition = 0;
            }
        }
       
       if((objectBlock.get<adm::JumpPosition>().has<adm::InterpolationLength>()))currentBlock.interpolationLength = objectBlock.get<adm::JumpPosition>().get<adm::InterpolationLength>().get().count()/1000000000.0;
   }
   
   if(objectBlock.has<adm::Gain>())
   {
       currentBlock.gain = objectBlock.get<adm::Gain>().get();
   }

   if(objectBlock.has<adm::CartesianPosition>())
   {
       currentBlock.moveSpherically = 0;
       auto position = objectBlock.get<adm::CartesianPosition>();
       if(position.has<adm::X>())currentBlock.x = position.get<adm::X>().get();;
       if(position.has<adm::Y>())currentBlock.y = position.get<adm::Y>().get();;
       if(position.has<adm::Z>())currentBlock.z = position.get<adm::Z>().get();;
   }
   else if(objectBlock.has<adm::SphericalPosition>())
   {
       currentBlock.moveSpherically = 1;
       auto position = objectBlock.get<adm::SphericalPosition>();
       
       float azimuth = 0.0;
       float elevation = 0.0;
       float distance = 0.0;
       
       if(position.has<adm::Azimuth>())azimuth = position.get<adm::Azimuth>().get();
       if(position.has<adm::Elevation>())elevation = position.get<adm::Elevation>().get();
       if(position.has<adm::Distance>())distance = position.get<adm::Distance>().get();
       
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
       if(position.has<adm::Distance>() && position.has<adm::Elevation>())
       {
           float z = distance * sin(TO_RAD * elevation);
           
           currentBlock.z = z;
       }
   }
   
    
    if(objectBlock.has<adm::Importance>())currentBlock.importance = objectBlock.get<adm::Importance>().get();
    if(objectBlock.has<adm::Width>())currentBlock.importance = objectBlock.get<adm::Width>().get();
    if(objectBlock.has<adm::Height>())currentBlock.height = objectBlock.get<adm::Height>().get();
    if(objectBlock.has<adm::Depth>())currentBlock.depth = objectBlock.get<adm::Depth>().get();
    if(objectBlock.has<adm::Diffuse>())currentBlock.diffuse = objectBlock.get<adm::Diffuse>().get();
    
    if(objectBlock.has<adm::ObjectDivergence>())
    {
        auto objectDivergence = objectBlock.get<adm::ObjectDivergence>();
        if(objectDivergence.has<adm::Divergence>())currentBlock.divergence = objectDivergence.get<adm::Divergence>().get();
        if(objectDivergence.has<adm::AzimuthRange>())currentBlock.azimuthRange = objectDivergence.get<adm::AzimuthRange>().get();
        if(objectDivergence.has<adm::PositionRange>())currentBlock.positionRange = objectDivergence.get<adm::PositionRange>().get();
    }
    
    if(objectBlock.has<adm::ChannelLock>())
    {
        auto channelLock = objectBlock.get<adm::ChannelLock>();
        if(channelLock.has<adm::MaxDistance>())currentBlock.maxDistance = channelLock.get<adm::MaxDistance>().get();
        if(channelLock.has<adm::ChannelLockFlag>())
        {
            if(channelLock.get<adm::ChannelLockFlag>().get())
            {
                currentBlock.channelLock = 1;
            }
            else
            {
                currentBlock.channelLock = 0;
            }
        }
    }
    
    
    currentBlock.newBlockFlag = true;
    currentBlock.cfId = std::stoi(adm::formatId(objectBlock.get<adm::AudioBlockFormatId>()).substr(3,8), nullptr, 16);
    currentBlock.blockId = objectBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();

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

