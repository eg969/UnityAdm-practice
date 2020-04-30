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
       currentBlock.cfId = 0;
       currentBlock.blockId = 0;
       currentBlock.typeDef = -1;
       currentBlock.rTime = 0.0;
       currentBlock.duration = 0.0;
       currentBlock.interpolationLength = 0.0;
       currentBlock.x = 0.0;
       currentBlock.y = 0.0;
       currentBlock.z = 0.0;
       currentBlock.gain = 1.0;
       currentBlock.jumpPosition = 0;
       currentBlock.moveSpherically = 0;
       currentBlock.channelNum = -1;
       
      
       std::string name;

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
           auto distance = position.get<adm::Distance>().get();
           if(position.has<adm::Azimuth>())
           {
               float x = distance * sin(-TO_RAD * position.get<adm::Azimuth>().get()) * cos(TO_RAD * position.get<adm::Elevation>().get());

               currentBlock.x = x;
           }
           if(position.has<adm::Elevation>())
           {
               
               float y = distance * cos(TO_RAD * position.get<adm::Elevation>().get()) * cos(TO_RAD * position.get<adm::Azimuth>().get());

               currentBlock.y = y;
           }
           if(position.has<adm::Distance>())
           {
               float z = distance * sin(TO_RAD * position.get<adm::Elevation>().get());
               
               currentBlock.z = z;
           }
       }
       
       currentBlock.newBlockFlag = true;
       strcpy(currentBlock.name, name.c_str());
       currentBlock.cfId = objectBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
       currentBlock.blockId = objectBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();
       
       
       
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


AudioObjectBlock getNextObjectBlock()
{
    AudioObjectBlock currentBlock;
    
    if(objectBlocks.size() ==  0)
    {
        readAvalibelBlocks();
    }
    
    if(objectBlocks.size() !=  0)
    {

        currentBlock = loadObjectBlock(objectBlocks[0]);


        objectBlocks.erase(objectBlocks.begin());
    }
    else
    {
        currentBlock.newBlockFlag = false;
    }
    
    return currentBlock;
}
