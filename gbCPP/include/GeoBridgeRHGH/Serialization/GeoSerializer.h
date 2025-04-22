#pragma once
#include "../Core/Geometry.h"
#include <vector>
#include <cstdint>

namespace GeomBridgeRHGH::Serialization {
    // Mesh serialization
    std::vector<uint8_t> serializeMesh(const Mesh& mesh);
    Mesh deserializeMesh(const uint8_t* data, size_t size);

    // Polyline serialization (stub declaration)
    std::vector<uint8_t> serializePolyline(const Polyline& polyline);
    Polyline deserializePolyline(const uint8_t* data, size_t size);
    
    // Generic geometry interface
    std::vector<uint8_t> serializeGeometry(const Geometry& geom);
    Geometry deserializeGeometry(const uint8_t* data, size_t size);
}
