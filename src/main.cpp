
#include "main.h"
#include "Interface.h"



extern "C"
{

    int readAdm(char filePath[2048])
    {
        return Dll::readAdm(filePath);
    }

    const char* getLatestException()
    {
        return Dll::getLatestException();
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


