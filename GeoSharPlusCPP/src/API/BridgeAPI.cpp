#include "GeoSharPlusCPP/API/BridgeAPI.h"

#include <iostream>
#include <memory>

#include "GSP_FB/cpp/geometry_generated.h"
#include "GeoSharPlusCPP/Core/MathTypes.h"
#include "GeoSharPlusCPP/Serialization/GeoSerializer.h"

namespace GS = GeoSharPlusCPP::Serialization;

extern "C" {

GEOBRIDGE_API bool GEOBRIDGE_CALL point3d_roundtrip(const uint8_t* inBuffer,
                                                    int inSize,
                                                    uint8_t** outBuffer,
                                                    int* outSize) {
  *outBuffer = nullptr;
  *outSize = 0;

  GeoSharPlusCPP::Vector3d pt;
  if (!GS::deserializePoint(inBuffer, inSize, pt)) {
    return false;
  }

  // Serialize the point into the allocated buffer
  if (!GS::serializePoint(pt, *outBuffer, *outSize)) {
    if (*outBuffer) delete[] *outBuffer;  // Cleanup
    *outBuffer = nullptr;
    *outSize = 0;

    return false;
  }

  return true;
}
}
