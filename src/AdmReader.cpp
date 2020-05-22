//
//  AdmReader.cpp
//  adm
//
//  Created by Edgars Grivcovs on 25/04/2020.
//


#include <stdio.h>

#include "AdmReader.h"




AdmReader::AdmReader()
{
    
}

AdmReader::~AdmReader()
{
    
}


bool AdmReader::isCommonDefinition(adm::AudioChannelFormat* channelFormat)
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
std::optional<Value> AdmReader::getFromMap(std::map<Key, Value> &targetMap, Key key){
    auto it = targetMap.find(key);
    if(it == targetMap.end()) return std::optional<Value>();
    return std::optional<Value>(it->second);
}
template<typename Key, typename Value>
void AdmReader::setInMap(std::map<Key, Value> &targetMap, Key key, Value value){
    auto it = targetMap.find(key);
    if(it == targetMap.end()){
        targetMap.insert(std::make_pair(key, value));
    } else {
        it->second = value;
    }
}

//AdmReader
void AdmReader::readAvalibelBlocks()
{
    
    auto trackFormats = parsedDocument->getElements<adm::AudioTrackFormat>();
    for(auto trackFormat : trackFormats)
    {
        //auto trackFormat = trackUid->getReference<adm::AudioTrackFormat>();
        
        auto tfIdStr = adm::formatId(trackFormat->get<adm::AudioTrackFormatId>());
        
        for(auto& audioId : audioIds)
        {
            std::string tfIdString = audioId.trackRef();
            if(tfIdStr == tfIdString)
            {
                std::shared_ptr<adm::AudioStreamFormat> streamFormat = trackFormat->getReference<adm::AudioStreamFormat>();

                if(streamFormat)
                {
                    std::shared_ptr<adm::AudioChannelFormat> channelFormat = streamFormat->getReference<adm::AudioChannelFormat>();

                    if(channelFormat)
                    {
                        auto cfIdStr = adm::formatId(channelFormat->get<adm::AudioChannelFormatId>());
                        int cfId = std::stoi(cfIdStr.substr(3,8), nullptr, 16);
                        auto audioObjects = parsedDocument->getElements<adm::AudioObject>();
                        std::shared_ptr<adm::AudioPackFormat> packFormat = streamFormat->getReference<adm::AudioPackFormat>();
                        for(auto audioObject : audioObjects)
                        {
                            auto packRef = audioObject->getReferences<adm::AudioPackFormat>()[0];
                            if(packFormat != nullptr && packFormat == packRef)
                            {
                                auto objIdStr = adm::formatId(audioObject->get<adm::AudioObjectId>());
                                int objId = std::stoi(objIdStr.substr(3,8), nullptr, 16);
                                int numOfChannels = 0;
                                auto cfRefs = packFormat->getReferences<adm::AudioChannelFormat>();
                                for(auto cfRef : cfRefs)
                                {
                                    int order = 0;
                                    auto blocks = cfRef->getElements<adm::AudioBlockFormatHoa>();
                                    if(blocks.size() != 0)
                                    {
                                        auto block = blocks[0];
                                        if(block.has<adm::Order>())order = block.get<adm::Order>().get();
                                        int newNumOfChnannels = (order + 1) * (order + 1);
                                        if(newNumOfChnannels > numOfChannels ) numOfChannels = newNumOfChnannels;
                                    }
                                }
                                setInMap(objectIds, cfId, objId);
                                setInMap(hoaChannels, objId, numOfChannels);
                            }
                        }

                        setInMap(channelNums, cfId, (int)audioId.trackIndex() - 1);

                        int typeDef = channelFormat->get<adm::TypeDescriptor>().get();
                        setInMap(typeDefs, cfId, typeDef);

                        for(auto newBlock : channelFormat->getElements<adm::AudioBlockFormatObjects>())
                        {
                            int blockId  = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();

                            if(!getFromMap(knownBlocks, cfId).has_value() || *(getFromMap(knownBlocks, cfId)) < blockId)
                            {
                                objectBlocks->push_back(newBlock);
                                setInMap(knownBlocks, cfId, blockId);
                            }
                        }

                        for(auto newBlock : channelFormat->getElements<adm::AudioBlockFormatDirectSpeakers>())
                        {
                            int blockId  = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();

                            if(!getFromMap(knownBlocks, cfId).has_value() || *(getFromMap(knownBlocks, cfId)) < blockId)
                            {
                                speakerBlocks->push_back(newBlock);
                                setInMap(knownBlocks, cfId, blockId);
                            }
                        }

                        for(auto newBlock : channelFormat->getElements<adm::AudioBlockFormatHoa>())
                        {
                            int blockId  = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();

                            if(!getFromMap(knownBlocks, cfId).has_value() || *(getFromMap(knownBlocks, cfId)) < blockId)
                            {
                                hoaBlocks->push_back(newBlock);
                                setInMap(knownBlocks, cfId, blockId);
                            }
                        }

                        for(auto newBlock : channelFormat->getElements<adm::AudioBlockFormatBinaural>())
                        {
                            int blockId  = newBlock.get<adm::AudioBlockFormatId>().get<adm::AudioBlockFormatIdCounter>().get();

                            if(!getFromMap(knownBlocks, cfId).has_value() || *(getFromMap(knownBlocks, cfId)) < blockId)
                            {
                                binauralBlocks->push_back(newBlock);
                                setInMap(knownBlocks, cfId, blockId);
                            }
                        }
                        
                        //notCommonDefs.push_back(channelFormat);
                    }
                }
            }
        }
    }
}

//AdmReader
int AdmReader::readAdm(char filePath[2048])
{
    objectBlocks->clear();
    speakerBlocks->clear();
    hoaBlocks->clear();
    binauralBlocks->clear();
    knownBlocks.clear();
    channelNums.clear();
    previousParameters.gain = 1.0;
    previousParameters.distance = 1.0;
    reader = nullptr;
    parsedDocument = nullptr;
    //notCommonDefs.clear();

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

const char* AdmReader::getLatestException()
{
    return latestExceptionMsg.c_str();
}

AdmReaderSingleton::AdmReaderSingleton() //: singleton { getInstance() }
{
    
}
AdmReaderSingleton::~AdmReaderSingleton()
{
    
}

std::shared_ptr<AdmReader> AdmReaderSingleton::getInstance()
{
    
    if(std::shared_ptr<AdmReader> singletonInst = singletonStatic.lock())
    {
        return singletonInst;
    }
    else
    {
        singletonInst = std::make_shared<AdmReader>();
        singletonStatic = singletonInst;
        return singletonInst;
    }
    
}
