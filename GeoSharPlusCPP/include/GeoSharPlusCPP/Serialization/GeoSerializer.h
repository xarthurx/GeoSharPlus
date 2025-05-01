#pragma once
#include <cstdint>
#include <vector>

#include "GeoSharPlusCPP/Core/Geometry.h"

namespace GeoSharPlusCPP::Serialization {
// Point serialization
bool serializePoint(const Vector3d& point, uint8_t*& resBuffer, int& size);
bool deserializePoint(const uint8_t* buffer, int size, Vector3d& point);

// Point array serialization
bool serializePointArray(const std::vector<Vector3d>& points);
bool deserializePointArray(const uint8_t* data, size_t size);

// Polyline serialization (stub declaration)
// std::vector<uint8_t> serializePolyline(const Polyline& polyline);
// Polyline deserializePolyline(const uint8_t* data, size_t size);

// Mesh serialization
// std::vector<uint8_t> serializeMesh(const Mesh& mesh);
// Mesh deserializeMesh(const uint8_t* data, size_t size);

}  // namespace GeoSharPlusCPP::Serialization
