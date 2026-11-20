#pragma once

#ifdef _WIN32
  #ifdef GEOSHARPLUS_EXPORTS
    #define GSP_API __declspec(dllexport)
  #else
    #define GSP_API __declspec(dllimport)
  #endif
  // Must agree with CallingConvention.Cdecl in the C# [DllImport] declarations.
  #define GSP_CALL __cdecl
#else
  #define GSP_API __attribute__((visibility("default")))
  #define GSP_CALL
#endif
