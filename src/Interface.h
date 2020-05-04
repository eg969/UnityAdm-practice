//Interface.h


#include "AdmReader.h"
#include "AudioBlockBinaural.h"
#include "AudioBlockDirectSpeakers.h"
#include "AudioBlockHoa.h"
#include "AudioBlockObjects.h"

#pragma once

namespace Dll
{
    AudioObjectBlock getNextObjectBlock();
    AudioHoaBlock getNextHoaBlock();
    AudioSpeakerBlock getNextSpeakerBlock();
    AudioBinauralBlock getNextBinauralBlock();
    
    //float* audioBuffer = nullptr;

    float* getAudioFrame(int startFrame, int bufferSize, int channelNum);
    int getSamplerate();
    int getNumberOfFrames();

}


