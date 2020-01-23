#include "main.h"
#include <adm/adm.hpp>
#include <bw64/bw64.hpp>
#include <adm/parse.hpp>
#include <vector>
#include <memory>
#include "string.h"

bool isCommonDefinition(adm::AudioChannelFormat* channelFormat)
{
    auto idString = adm::formatId(channelFormat->get<adm::AudioChannelFormatId>());
    bool isCommon{false};
    if(idString.size() == 11) {
        auto identityDigits = idString.substr(7, 4);
        try {
            auto identityValue = std::stoi(identityDigits, 0, 16);
            isCommon = (identityValue < 0x1000);
        }
        catch(std::exception e) {/* invalid ID, so not a common definition */ };
    }
    return isCommon;
}

extern "C"
{
    struct AdmAudioBlock
    {
        char name[100];
        float rTime;
        float x;
        float y;
        float z;
    };
    

    AdmAudioBlock getNextBlock(int cfIndex, int blockIndex)
    {
        AdmAudioBlock currentBlock;
        std::string name;
        
        auto reader = bw64::readFile("/Users/edgarsg/Desktop/test.wav");
        
        auto aXml = reader->axmlChunk();
        
        std::stringstream stream;
        aXml->write(stream);
        auto parsedDocument = adm::parseXml(stream);
        auto allChannelFormats = parsedDocument->getElements<adm::AudioChannelFormat>();
        std::vector<std::shared_ptr<adm::AudioChannelFormat>> notCommonDefs;
        
        for (auto channelFormat: allChannelFormats)
        {
            if(!isCommonDefinition(channelFormat.get()))
            {
                notCommonDefs.push_back(channelFormat);
            }
        }
    
        name = notCommonDefs[cfIndex]->get<adm::AudioChannelFormatName>().get();
        
        auto blocks = notCommonDefs[cfIndex]->getElements<adm::AudioBlockFormatObjects>();
        
        auto num = blocks[blockIndex].get<adm::Rtime>().get().count();
        auto pos = blocks[blockIndex].get<adm::CartesianPosition>();

        strcpy(currentBlock.name, name.c_str());
        currentBlock.rTime = num/100000000.0;
        currentBlock.x = pos.get<adm::X>().get();
        currentBlock.y = pos.get<adm::Y>().get();
        currentBlock.z = pos.get<adm::Z>().get();

        return currentBlock;
    }
    
    bool queryNextBlock(int cfIndex, int blockIndex)
    {
        int numOfBlocks;
        
        std::string name;
        
        auto reader = bw64::readFile("/Users/edgarsg/Desktop/test.wav");
        
        auto aXml = reader->axmlChunk();
        
        std::stringstream stream;
        aXml->write(stream);
        auto parsedDocument = adm::parseXml(stream);
        auto allChannelFormats = parsedDocument->getElements<adm::AudioChannelFormat>();
        std::vector<std::shared_ptr<adm::AudioChannelFormat>> notCommonDefs;
        
        for (auto channelFormat: allChannelFormats)
        {
            if(!isCommonDefinition(channelFormat.get()))
            {
                notCommonDefs.push_back(channelFormat);
            }
        }
        
        if(cfIndex < notCommonDefs.size())
        {
            name = notCommonDefs[cfIndex]->get<adm::AudioChannelFormatName>().get();
            
            auto blocks = notCommonDefs[cfIndex]->getElements<adm::AudioBlockFormatObjects>();
            
            numOfBlocks = blocks.size();
            
            if(blockIndex < numOfBlocks)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
            
    }
}
    

