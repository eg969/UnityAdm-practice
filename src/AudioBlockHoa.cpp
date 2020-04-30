//
//  AudioBlockHoa.cpp
//  adm
//
//  Created by Edgars Grivcovs on 25/04/2020.
//

#include <stdio.h>
#include "AudioBlockHoa.h"

AdmAudioBlock getHoaBlock(adm::AudioBlockFormatHoa hoaBlock)
{
    AdmAudioBlock currentBlock;
    
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
    
    currentBlock.cfId = hoaBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
    currentBlock.blockId = hoaBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();
    
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
