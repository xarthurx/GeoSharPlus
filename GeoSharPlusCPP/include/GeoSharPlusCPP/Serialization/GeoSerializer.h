#pragma once
#include <cstdint>
#include <vector>

#include "GeoSharPlusCPP/Core/Geometry.h"

namespace GeoSharPlusCPP::Serialization {
// Point serialization
bool serializePoint(const Vector3d& point, uint8_t*& resBuffer, int& resSize);
bool deserializePoint(const uint8_t* buffer, int size, Vector3d& point);

// Point array serialization
bool serializePointArray(const std::vector<Vector3d>& points,
                         uint8_t*& resBuffer, int& resSize);
bool deserializePointArray(const uint8_t* data, int size,
                           std::vector<Vector3d>& pointArray);


}  // namespace GeoSharPlusCPP::Serialization
