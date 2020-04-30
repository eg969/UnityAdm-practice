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
#define TO_RAD 3.14/180

template<class... Ts> struct overload : Ts... { using Ts::operator()...; };
template<class... Ts> overload(Ts...) -> overload<Ts...>;

bool isCommonDefinition(adm::AudioChannelFormat* channelFormat);
template<typename Key, typename Value>
std::optional<Value> getFromMap(std::map<Key, Value> &targetMap, Key key);
template<typename Key, typename Value>
void setInMap(std::map<Key, Value> &targetMap, Key key, Value value);
const char* getLatestException();
void readAvalibelBlocks();
int readAdm(char filePath[2048]);

std::shared_ptr<adm::Document> parsedDocument;
std::string latestExceptionMsg{""};
std::unique_ptr<bw64::Bw64Reader> reader;
std::vector<bw64::AudioId> audioIds;

std::vector<adm::AudioBlockFormatObjects> objectBlocks;
std::vector<adm::AudioBlockFormatDirectSpeakers> speakerBlocks;
std::vector<adm::AudioBlockFormatHoa> hoaBlocks;
std::vector<adm::AudioBlockFormatBinaural> binauralBlocks;

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

/*struct AdmAudioBlock
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
};*/

std::vector<std::shared_ptr<adm::AudioChannelFormat>> notCommonDefs;
