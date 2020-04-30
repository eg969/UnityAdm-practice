//
//  AdmReader.cpp
//  adm
//
//  Created by Edgars Grivcovs on 25/04/2020.
//


#include <stdio.h>

#include "AdmReader.h"


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

//AdmReader
void readAvalibelBlocks()
{
    auto trackFormats = parsedDocument->getElements<adm::AudioTrackFormat>();
    int tfIdVal;
    for(auto trackFormat : trackFormats)
    {
        tfIdVal = trackFormat->get<adm::AudioTrackFormatId>().get<adm::AudioTrackFormatIdValue>().get();
        for(auto& audioId : audioIds)
        {
            std::string tfIdString = audioId.trackRef().substr(7,4);
            int tfIdInt = std::stoi(tfIdString, nullptr, 16);

            if(tfIdVal == tfIdInt)
            {
                std::shared_ptr<adm::AudioStreamFormat> streamFormat = trackFormat->getReference<adm::AudioStreamFormat>();

                if(streamFormat)
                {
                    std::shared_ptr<adm::AudioChannelFormat> channelFormat = streamFormat->getReference<adm::AudioChannelFormat>();

                    if(channelFormat)
                    {
                        int cfId = channelFormat->get<adm::AudioChannelFormatId>().get<adm::AudioChannelFormatIdValue>().get();
                        setInMap(channelNums, cfId, (int)audioId.trackIndex() - 1);

                        int typeDef = channelFormat->get<adm::TypeDescriptor>().get();
                        setInMap(typeDefs, cfId, typeDef);

                        for(auto newBlock : channelFormat->getElements<adm::AudioBlockFormatObjects>())
                        {
                            int cfId = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
                            int blockId  = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();

                            if(!getFromMap(knownBlocks, cfId).has_value() || *(getFromMap(knownBlocks, cfId)) < blockId)
                            {
                                objectBlocks.push_back(newBlock);
                                setInMap(knownBlocks, cfId, blockId);
                            }
                        }

                        for(auto newBlock : channelFormat->getElements<adm::AudioBlockFormatDirectSpeakers>())
                        {
                            int cfId = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
                            int blockId  = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();

                            if(!getFromMap(knownBlocks, cfId).has_value() || *(getFromMap(knownBlocks, cfId)) < blockId)
                            {
                                speakerBlocks.push_back(newBlock);
                                setInMap(knownBlocks, cfId, blockId);
                            }
                        }

                        for(auto newBlock : channelFormat->getElements<adm::AudioBlockFormatHoa>())
                        {
                            int cfId = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
                            int blockId  = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();

                            if(!getFromMap(knownBlocks, cfId).has_value() || *(getFromMap(knownBlocks, cfId)) < blockId)
                            {
                                hoaBlocks.push_back(newBlock);
                                setInMap(knownBlocks, cfId, blockId);
                            }
                        }

                        for(auto newBlock : channelFormat->getElements<adm::AudioBlockFormatBinaural>())
                        {
                            int cfId = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdValue>().get();
                            int blockId  = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();

                            if(!getFromMap(knownBlocks, cfId).has_value() || *(getFromMap(knownBlocks, cfId)) < blockId)
                            {
                                binauralBlocks.push_back(newBlock);
                                setInMap(knownBlocks, cfId, blockId);
                            }
                        }
                        
                        notCommonDefs.push_back(channelFormat);
                    }
                }
            }
        }
    }
}
//AdmReader
int readAdm(char filePath[2048])
{
    objectBlocks.clear();
    speakerBlocks.clear();
    hoaBlocks.clear();
    binauralBlocks.clear();
    knownBlocks.clear();
    channelNums.clear();
    previousParameters.gain = 1.0;
    previousParameters.distance = 1.0;
    reader = nullptr;
    parsedDocument = nullptr;
    notCommonDefs.clear();

    try
    {
        reader = bw64::readFile(filePath);
        auto aXml = reader->axmlChunk();
        auto chnaChunk = reader->chnaChunk();
        audioIds = chnaChunk->audioIds();

        std::stringstream stream;
        aXml->write(stream);
        parsedDocument = adm::parseXml(stream);
    }
    catch (std::exception &e)
    {
        latestExceptionMsg = e.what();
        return 1;
    }
        return 0;
}

