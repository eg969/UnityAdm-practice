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

    std::shared_ptr<adm::Document> parsedDocument;

    void readAdm()
    {
        auto reader = bw64::readFile("/Users/edgarsg/Desktop/test.wav");
        
        auto aXml = reader->axmlChunk();
        
        std::stringstream stream;
        aXml->write(stream);
        parsedDocument = adm::parseXml(stream);
        
    }

    struct AdmAudioBlock
    {
        bool newBlockFlag;
        char name[100];
        int cfId;
        int blockId;
        float rTime;
        float x;
        float y;
        float z;
    };

    std::vector<adm::AudioBlockFormatObjects> blocks;

    void readAvalibelBlocks()
    {
        auto allChannelFormats = parsedDocument->getElements<adm::AudioChannelFormat>();
        std::vector<std::shared_ptr<adm::AudioChannelFormat>> notCommonDefs;
        
        for (auto channelFormat: allChannelFormats)
        {
            if(!isCommonDefinition(channelFormat.get()))
            {
                notCommonDefs.push_back(channelFormat);
            }
        }
            
        for (auto channelFormat : notCommonDefs)
        {
            auto newBlocks = channelFormat->getElements<adm::AudioBlockFormatObjects>();
            
            for(auto newBlock : newBlocks)
            {
                blocks.push_back(newBlock);
            }
        }
    }

    AdmAudioBlock getNextBlock()
    {
        AdmAudioBlock currentBlock;
        if(blocks.size() !=  0)
        {
            std::string name;
            auto rTime = blocks[0].get<adm::Rtime>().get().count();
            auto position = blocks[0].get<adm::CartesianPosition>();
            int blockId = blocks[0].get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
            int cfId = 0;
            
            currentBlock.newBlockFlag = true;
            strcpy(currentBlock.name, name.c_str());
            currentBlock.cfId = cfId;
            currentBlock.blockId = blockId;
            currentBlock.rTime = rTime/100000000.0;
            
            currentBlock.x = position.get<adm::X>().get();
            currentBlock.y = position.get<adm::Y>().get();
            currentBlock.z = position.get<adm::Z>().get();
            
            blocks.erase(blocks.begin());
        }
        else
        {
            currentBlock.newBlockFlag = false;
            readAvalibelBlocks();
        }
        return currentBlock;
    }
    
    bool queryNextBlock(int cfIndex, int blockIndex)
    {
        return true;
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
    

