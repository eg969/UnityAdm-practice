#include "Interface.h"

std::shared_ptr<AdmReader> admReader = AdmReaderSingleton::getInstance();
#ifdef WIN32
#define DLLEXPORT __declspec(dllexport)
#else
#define DLLEXPORT
#endif


