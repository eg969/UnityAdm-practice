
#include "main.h"

extern "C"
{

    DLLEXPORT int readAdm(char filePath[2048])
    {
        return AdmReaderSingleton::getInstance()->readAdm(filePath);
    }

    DLLEXPORT const char* getLatestException()
    {
        return AdmReaderSingleton::getInstance()->getLatestException();
    }

    DLLEXPORT AudioObjectBlock getNextObjectBlock()
    {
        return Dll::getNextObjectBlock();
    }

    DLLEXPORT AudioHoaBlock getNextHoaBlock()
    {
        return Dll::getNextHoaBlock();
    }

    DLLEXPORT AudioSpeakerBlock getNextSpeakerBlock()
    {
        return Dll::getNextSpeakerBlock();
    }

    DLLEXPORT AudioBinauralBlock getNextBinauralBlock()
    {
        return Dll::getNextBinauralBlock();
    }

    DLLEXPORT float* getAudioFrame(int startFrame, int bufferSize, int channelNum)
    {
        return Dll::getAudioFrame(startFrame, bufferSize, channelNum);
    }

    DLLEXPORT float* getHoaAudioFrame(int startFrame, int bufferSize, int channelNums[], int amountOfChannels)
    {
        return Dll::getHoaAudioFrame(startFrame, bufferSize, channelNums, amountOfChannels);
    }

    DLLEXPORT int getSamplerate()
    {
        return Dll::getSamplerate();
    }

    DLLEXPORT int getNumberOfFrames()
    {
        return Dll::getNumberOfFrames();
    }
}


