#pragma once

#ifdef _WIN32
    #ifdef GEOBRIDGE_EXPORTS
        #define GEOBRIDGE_API __declspec(dllexport)
    #else
        #define GEOBRIDGE_API __declspec(dllimport)
    #endif
    #define GEOBRIDGE_CALL __stdcall
#else
    #define GEOBRIDGE_API __attribute__((visibility("default")))
    #define GEOBRIDGE_CALL
#endif
