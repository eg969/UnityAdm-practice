//
//  Interface.cpp
//  adm
//
//  Created by Edgars Grivcovs on 25/04/2020.
//

#include <stdio.h>

#include "Interface.h"


//Interface
AdmAudioBlock getNextBlock()
{
    AdmAudioBlock currentBlock;
    
    if(blocks.size() ==  0)
    {
        readAvalibelBlocks();
    }
    
    if(blocks.size() !=  0)
    {
        std::visit(overload
        {
            [&currentBlock](adm::AudioBlockFormatObjects audioBlock){currentBlock = getObjectBlock(audioBlock);},
            [&currentBlock](adm::AudioBlockFormatDirectSpeakers audioBlock){currentBlock = getSpeakerBlock(audioBlock);},
            [&currentBlock](adm::AudioBlockFormatHoa audioBlock){currentBlock = getHoaBlock(audioBlock);},
            [&currentBlock](adm::AudioBlockFormatBinaural audioBlock){currentBlock = getBinauralBlock(audioBlock);},
            [](auto){}
        },blocks[0]);

        blocks.erase(blocks.begin());
    }
    else
    {
        currentBlock.newBlockFlag = false;
    }
    
    return currentBlock;
}

//Interface

float* getAudioFrame(int startFrame, int bufferSize, int channelNum)
{
    if(audioBuffer == nullptr)audioBuffer = new float[bufferSize];
    float* bufferCounter = audioBuffer;
    reader->seek(startFrame);
    std::vector<float> block(bufferSize * reader->channels(), 0.0);
    reader->read(block.data(), bufferSize);

    for(int sampleNum = 0; sampleNum < bufferSize; sampleNum++)
    {
        int blockPos = channelNum + (sampleNum * reader->channels());
        *bufferCounter = block[blockPos];
        bufferCounter++;
    }

    return audioBuffer;
}
//Interface
int getSamplerate()
{
    return reader->sampleRate();
}
//Interface
int getNumberOfFrames()
{
    return reader->numberOfFrames();
}
