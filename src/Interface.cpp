//
//  Interface.cpp
//  adm
//
//  Created by Edgars Grivcovs on 25/04/2020.
//

#include <stdio.h>
#include <iterator>
#include <vector>
#include <iostream>

#include "Interface.h"


//Interface
AudioObjectBlock Dll::getNextObjectBlock()
{
    AudioObjectBlock currentBlock;
    auto& objectBlocks = AdmReaderSingleton::getInstance()->objectBlocks;
    
    if(objectBlocks->size() ==  0)
    {
         AdmReaderSingleton::getInstance()->readAvalibelBlocks();
    }
    
    if(objectBlocks->size() !=  0)
    {
        auto objectBlocksToProcess = *(objectBlocks.get());
        currentBlock = loadObjectBlock(objectBlocksToProcess[0]);
        objectBlocks->erase(objectBlocks->begin());
    }
    else
    {
        currentBlock.newBlockFlag = false;
    }
    
    return currentBlock;
}

AudioHoaBlock Dll::getNextHoaBlock()
{
    AudioHoaBlock currentBlock;
    auto& hoaBlocks = AdmReaderSingleton::getInstance()->hoaBlocks;
    if(hoaBlocks->size() ==  0)
    {
         AdmReaderSingleton::getInstance()->readAvalibelBlocks();
    }
    
    if(hoaBlocks->size() !=  0)
    {
        auto hoaBlocksToProcess = *(hoaBlocks.get());
        currentBlock = loadHoaBlock(hoaBlocksToProcess[0]);
        hoaBlocks->erase(hoaBlocks->begin());
    }
    else
    {
        currentBlock.newBlockFlag = false;
    }
    
    return currentBlock;
}

AudioSpeakerBlock Dll::getNextSpeakerBlock()
{
    AudioSpeakerBlock currentBlock;
    auto& speakerBlocks = AdmReaderSingleton::getInstance()->speakerBlocks;
    
    if(speakerBlocks->size() ==  0)
    {
        AdmReaderSingleton::getInstance()->readAvalibelBlocks();
    }
    
    if(speakerBlocks->size() !=  0)
    {
        auto speakerBlocksToProcess = *(speakerBlocks.get());
        currentBlock = loadSpeakerBlock(speakerBlocksToProcess[0]);
        speakerBlocks->erase(speakerBlocks->begin());
    }
    else
    {
        currentBlock.newBlockFlag = false;
    }
    
    return currentBlock;
}

AudioBinauralBlock Dll::getNextBinauralBlock()
{
    AudioBinauralBlock currentBlock;
    auto& binauralBlocks = AdmReaderSingleton::getInstance()->binauralBlocks;
    
    if(binauralBlocks->size() ==  0)
    {
        
        AdmReaderSingleton::getInstance()->readAvalibelBlocks();
    }
    
    if(binauralBlocks->size() !=  0)
    {
        auto binauralBlocksToProcess = *(binauralBlocks.get());
        currentBlock = loadBinauralBlock(binauralBlocksToProcess[0]);
        binauralBlocks->erase(binauralBlocks->begin());
    }
    else
    {
        currentBlock.newBlockFlag = false;
    }
    
    return currentBlock;
}

//Interface
float* Dll::getAudioFrame(int startFrame, int bufferSize, int channelNum)
{
    //float* audioBuffer = nullptr;
    if(AdmReaderSingleton::getInstance()->audioBuffer == nullptr)AdmReaderSingleton::getInstance()->audioBuffer  = new float[bufferSize];
    float* bufferCounter = AdmReaderSingleton::getInstance()->audioBuffer ;
    auto& reader = AdmReaderSingleton::getInstance()->reader;
    
    reader->seek(startFrame);
    std::vector<float> block(bufferSize * reader->channels(), 0.0);
    reader->read(block.data(), bufferSize);

    for(int sampleNum = 0; sampleNum < bufferSize; sampleNum++)
    {
        int blockPos = channelNum + (sampleNum * reader->channels());
        *bufferCounter = block[blockPos];
        bufferCounter++;
    }

    return AdmReaderSingleton::getInstance()->audioBuffer;
}

float* Dll::getHoaAudioFrame(int startFrame, int bufferSize, int channelNums[], int amountOfChannels)
{
    //float* audioBuffer = nullptr;
    if(AdmReaderSingleton::getInstance()->audioBuffer == nullptr)AdmReaderSingleton::getInstance()->audioBuffer  = new float[bufferSize];
    float* bufferCounter = AdmReaderSingleton::getInstance()->audioBuffer ;
    auto& reader = AdmReaderSingleton::getInstance()->reader;
    
    reader->seek(startFrame);
    std::vector<float> block(bufferSize * reader->channels(), 0.0);
    reader->read(block.data(), bufferSize);

    for(int sampleNum = 0; sampleNum < bufferSize; sampleNum++)
    {
        for(int channelIndex = 0 ; channelIndex < amountOfChannels; channelIndex++)
        {
            int blockPos = channelNums[channelIndex] + (sampleNum * reader->channels());
            *bufferCounter = block[blockPos];
            bufferCounter++;
        }
    }

    return AdmReaderSingleton::getInstance()->audioBuffer;
}
//Interface
int Dll::getSamplerate()
{
    auto& reader = AdmReaderSingleton::getInstance()->reader;
    return reader->sampleRate();
}

//Interface
int Dll::getNumberOfFrames()
{
    auto& reader = AdmReaderSingleton::getInstance()->reader;
    return reader->numberOfFrames();
}
