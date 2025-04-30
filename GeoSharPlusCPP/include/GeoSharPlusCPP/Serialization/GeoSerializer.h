#pragma once
#include <cstdint>
#include <vector>

#include "../Core/Geometry.h"

namespace GeoSharPlusCPP::Serialization {
// Point array serialization
std::vector<uint8_t> serializePointArray(const std::vector<Vector3d>& points);
std::vector<Vector3d> deserializePointArray(const uint8_t* data, size_t size);

// Polyline serialization (stub declaration)
std::vector<uint8_t> serializePolyline(const Polyline& polyline);
Polyline deserializePolyline(const uint8_t* data, size_t size);

// Mesh serialization
//std::vector<uint8_t> serializeMesh(const Mesh& mesh);
//Mesh deserializeMesh(const uint8_t* data, size_t size);

}  // namespace GeoSharPlusCPP::Serialization
