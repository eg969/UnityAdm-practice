//  AdmReader.h



#include <adm/adm.hpp>
#include <bw64/bw64.hpp>
#include <adm/parse.hpp>
#include <vector>
#include <memory>
#include "string.h"
#include <map>
#include <optional>
#include <math.h>

#pragma once
#define TO_RAD 3.14/180.0

class AdmReader
{
public:
    AdmReader();
    ~AdmReader();
    
    bool isCommonDefinition(adm::AudioChannelFormat* channelFormat);
    template<typename Key, typename Value>
    std::optional<Value> getFromMap(std::map<Key, Value> &targetMap, Key key);
    template<typename Key, typename Value>
    void setInMap(std::map<Key, Value> &targetMap, Key key, Value value);
    void readAvalibelBlocks();

    //namespace Dll
    //{
    const char* getLatestException();
    int readAdm(char filePath[2048]);
    //}

    std::shared_ptr<adm::Document> parsedDocument;
    std::string latestExceptionMsg{""};
    std::unique_ptr<bw64::Bw64Reader> reader;
    std::vector<bw64::AudioId> audioIds;

    std::shared_ptr<std::vector<adm::AudioBlockFormatObjects>> objectBlocks = std::make_shared<std::vector<adm::AudioBlockFormatObjects>>();
    std::shared_ptr<std::vector<adm::AudioBlockFormatDirectSpeakers>> speakerBlocks = std::make_shared<std::vector<adm::AudioBlockFormatDirectSpeakers>>();
    std::shared_ptr<std::vector<adm::AudioBlockFormatHoa>> hoaBlocks = std::make_shared<std::vector<adm::AudioBlockFormatHoa>>();
    std::shared_ptr<std::vector<adm::AudioBlockFormatBinaural>> binauralBlocks =  std::make_shared<std::vector<adm::AudioBlockFormatBinaural>>();

    using ChannelFormatId = int;
    using BlockIndex = int;
    using ChannelNum = int;
    using TypeDef = int;
    using ObjectId = int;
    using NumberOfChannels = int;

    std::map<ChannelFormatId,BlockIndex> knownBlocks;
    std::map<ChannelFormatId,ObjectId> objectIds;
    std::map<ObjectId,NumberOfChannels> hoaChannels;
    std::map<ChannelFormatId,ChannelNum> channelNums;
    std::map<ChannelFormatId,TypeDef> typeDefs;

    struct holdParameters
    {
        float gain;
        float distance;
    };

    holdParameters previousParameters;
    float* audioObjectBuffer = nullptr;
    float* audioHoaBuffer = nullptr;
    //std::vector<std::shared_ptr<adm::AudioChannelFormat>> notCommonDefs;
};

class AdmReaderSingleton {
public:
    AdmReaderSingleton();
    ~AdmReaderSingleton();
     static std::shared_ptr<AdmReader> getInstance();
    
private:
    inline static std::weak_ptr<AdmReader> singletonStatic;
    //std::shared_ptr<AdmReader> singleton;
};

