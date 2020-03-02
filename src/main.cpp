#include "main.h"
#include <adm/adm.hpp>
#include <bw64/bw64.hpp>
#include <adm/parse.hpp>
#include <vector>
#include <memory>
#include "string.h"
#include <map>
#include <optional>

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

template<typename Key, typename Value>
std::optional<Value> getFromMap(std::map<Key, Value> &targetMap, Key key){
    auto it = targetMap.find(key);
    if(it == targetMap.end()) return std::optional<Value>();
    return std::optional<Value>(it->second);
}
template<typename Key, typename Value>
void setInMap(std::map<Key, Value> &targetMap, Key key, Value value){
    auto it = targetMap.find(key);
    if(it == targetMap.end()){
        targetMap.insert(std::make_pair(key, value));
    } else {
        it->second = value;
    }
}

extern "C"
{

    std::shared_ptr<adm::Document> parsedDocument;

    std::vector<adm::AudioBlockFormatObjects> blocks;
    using ChannelFormatId = int;
    using BlockIndex = int;

    std::map<ChannelFormatId,BlockIndex> knownBlocks;

    struct AdmAudioBlock
    {
        bool newBlockFlag;
        char name[100];
        int cfId;
        int blockId;
        float rTime;
        float duration;
        float x;
        float y;
        float z;
    };

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
                int cfId = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
                int blockId  = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();

                if(!getFromMap(knownBlocks, cfId).has_value() || *(getFromMap(knownBlocks, cfId)) < blockId)
                {
                    blocks.push_back(newBlock);
                    setInMap(knownBlocks, cfId, blockId);
                }
            }
        }
    }

    void readAdm()
    {
        blocks.clear();
        knownBlocks.clear();
        auto reader = bw64::readFile("/Users/edgarsg/Desktop/test.wav");

        auto aXml = reader->axmlChunk();

        std::stringstream stream;
        aXml->write(stream);
        parsedDocument = adm::parseXml(stream);
    }

    AdmAudioBlock getNextBlock()
    {
        AdmAudioBlock currentBlock;
        if(blocks.size() !=  0)
        {
            std::string name;
            auto rTime = blocks[0].get<adm::Rtime>().get().count();
            float duration = 0.0;
            if(blocks[0].has<adm::Duration>()) {
                duration = blocks[0].get<adm::Duration>().get().count();
            }
            auto position = blocks[0].get<adm::CartesianPosition>();
            int blockId = blocks[0].get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();
            int cfId = blocks[0].get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();

            currentBlock.newBlockFlag = true;
            strcpy(currentBlock.name, name.c_str());
            currentBlock.cfId = cfId;
            currentBlock.blockId = blockId;
            currentBlock.rTime = rTime/1000000000.0;
            currentBlock.duration = duration/1000000000.0;
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


