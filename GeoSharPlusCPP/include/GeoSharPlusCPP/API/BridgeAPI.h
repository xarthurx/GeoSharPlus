#pragma once
#include <cstdint>

#include "GeoSharPlusCPP/Core/Macro.h"

extern "C" {
// New point3d functions
GEOBRIDGE_API bool GEOBRIDGE_CALL point3d_roundtrip(const uint8_t* InBuffer,
                                                    int inSize,
                                                    uint8_t** outBuffer,
                                                    int* outSize);
}