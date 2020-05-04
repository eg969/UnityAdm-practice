
#include "main.h"

extern "C"
{

    int readAdm(char filePath[2048])
    {
        return AdmReaderSingleton::getInstance()->readAdm(filePath);
    }

    const char* getLatestException()
    {
        return AdmReaderSingleton::getInstance()->getLatestException();
    }

    AudioObjectBlock getNextObjectBlock()
    {
        return Dll::getNextObjectBlock();
    }

    AudioHoaBlock getNextHoaBlock()
    {
        return Dll::getNextHoaBlock();
    }

    AudioSpeakerBlock getNextSpeakerBlock()
    {
        return Dll::getNextSpeakerBlock();
    }

    AudioBinauralBlock getNextBinauralBlock()
    {
        return Dll::getNextBinauralBlock();
    }

    float* getAudioFrame(int startFrame, int bufferSize, int channelNum)
    {
        return Dll::getAudioFrame(startFrame, bufferSize, channelNum);
    }

    int getSamplerate()
    {
        return Dll::getSamplerate();
    }

    int getNumberOfFrames()
    {
        return Dll::getNumberOfFrames();
    }
}


