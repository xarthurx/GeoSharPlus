#pragma once
#include <cstdint>

#include "GeoSharPlusCPP/Core/Macro.h"

extern "C" {
// Conduct a roundtrip serialization of a point3d
GEOBRIDGE_API bool GEOBRIDGE_CALL point3d_roundtrip(const uint8_t* InBuffer,
                                                    int inSize,
                                                    uint8_t** outBuffer,
                                                    int* outSize);
// Conduct a roundtrip serialization of a point array
GEOBRIDGE_API bool GEOBRIDGE_CALL point3d_array_roundtrip(
    const uint8_t* inBuffer, int inSize, uint8_t** outBuffer, int* outSize);
}