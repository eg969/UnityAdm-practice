//Interface.h


#include "AdmReader.h"
#include "AudioBlockBinaural.h"
#include "AudioBlockDirectSpeakers.h"
#include "AudioBlockHoa.h"
#include "AudioBlockObjects.h"

#pragma once
AdmAudioBlock getNextBlock();
float* getAudioFrame(int startFrame, int bufferSize, int channelNum);
int getSamplerate();
int getNumberOfFrames();

float* audioBuffer = nullptr;
