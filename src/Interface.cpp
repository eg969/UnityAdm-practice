//
//  Interface.cpp
//  adm
//
//  Created by Edgars Grivcovs on 25/04/2020.
//

#include <stdio.h>

#include "Interface.h"


//Interface
/*AdmAudioBlock getNextBlock()
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
}*/


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

AudioHoaBlock getNextHoaBlock()
{
    AudioHoaBlock currentBlock;
    
    if(hoaBlocks.size() ==  0)
    {
        readAvalibelBlocks();
    }
    
    if(hoaBlocks.size() !=  0)
    {
        currentBlock = loadHoaBlock(hoaBlocks[0]);
        hoaBlocks.erase(hoaBlocks.begin());
    }
    else
    {
        currentBlock.newBlockFlag = false;
    }
    
    return currentBlock;
}

AudioSpeakerBlock getNextSpeakerBlock()
{
    AudioSpeakerBlock currentBlock;
    
    if(speakerBlocks.size() ==  0)
    {
        readAvalibelBlocks();
    }
    
    if(hoaBlocks.size() !=  0)
    {
        currentBlock = loadSpeakerBlock(speakerBlocks[0]);
        speakerBlocks.erase(speakerBlocks.begin());
    }
    else
    {
        currentBlock.newBlockFlag = false;
    }
    
    return currentBlock;
}

AudioBinauralBlock getNextBinauralBlock()
{
    AudioBinauralBlock currentBlock;
    
    if(binauralBlocks.size() ==  0)
    {
        readAvalibelBlocks();
    }
    
    if(binauralBlocks.size() !=  0)
    {
        currentBlock = loadBinauralBlock(binauralBlocks[0]);
        binauralBlocks.erase(binauralBlocks.begin());
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
