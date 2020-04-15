#include "main.h"
#include <adm/adm.hpp>
#include <bw64/bw64.hpp>
#include <adm/parse.hpp>
#include <vector>
#include <memory>
#include "string.h"
#include <map>
#include <optional>
#include <math.h>

#define TO_RAD 3.14/180

template<class... Ts> struct overload : Ts... { using Ts::operator()...; };
template<class... Ts> overload(Ts...) -> overload<Ts...>;

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
    std::string latestExceptionMsg{""};
    std::unique_ptr<bw64::Bw64Reader> reader;
    std::vector<bw64::AudioId> audioIds;
    const char* getLatestException(){
        return latestExceptionMsg.c_str();
    }

    //float* audioBuffer;
    using AdmBlockVariant = std::variant<
        adm::AudioBlockFormatObjects,
        adm::AudioBlockFormatDirectSpeakers,
        adm::AudioBlockFormatHoa,
        adm::AudioBlockFormatBinaural
    >;

    std::vector<AdmBlockVariant> blocks;
    using ChannelFormatId = int;
    using BlockIndex = int;
    using ChannelNum = int;
    using TypeDef = int;

    std::map<ChannelFormatId,BlockIndex> knownBlocks;
    std::map<ChannelFormatId,ChannelNum> channelNums;
    std::map<ChannelFormatId,TypeDef> typeDefs;


    struct holdParameters
    {
        float gain;
        float distance;
    };
    
    holdParameters previousParameters;

    
    struct AdmAudioBlock
    {
        bool newBlockFlag;
        char name[100];
        int cfId;
        int blockId;
        int typeDef;
        float rTime;
        float duration;
        float interpolationLength;
        float x;
        float y;
        float z;
        float gain;
        int jumpPosition;
        int moveSpherically;
        int channelNum;
    };
    

    std::vector<std::shared_ptr<adm::AudioChannelFormat>> notCommonDefs;

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
                            
                             notCommonDefs.push_back(channelFormat);
                        }
                    }
                }
            }
        }
        
    }

    int readAdm(char filePath[2048])
    {
        blocks.clear();
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

    float* audioBuffer = nullptr;

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
    
    int getSamplerate()
    {
        return reader->sampleRate();
    }

    int getNumberOfFrames()
    {
        return reader->numberOfFrames();
    }

    AdmAudioBlock getObjectBlock(adm::AudioBlockFormatObjects objectBlock)
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
        
       
        std::string name;

        if(objectBlock.has<adm::Rtime>())currentBlock.rTime =objectBlock.get<adm::Rtime>().get().count()/1000000000.0;
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
    
    AdmAudioBlock getSpeakerBlock(adm::AudioBlockFormatDirectSpeakers speakerBlock)
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
        
        return currentBlock;
    }
    
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
        
        return currentBlock;
    }
    
    AdmAudioBlock getBinauralBlock(adm::AudioBlockFormatBinaural binauralBlock)
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
        
        return currentBlock;
    }
    
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
}


