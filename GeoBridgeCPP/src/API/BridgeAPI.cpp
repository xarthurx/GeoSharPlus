#include "GeoBridgeCPP/API/BridgeAPI.h"

#include "GeoBridgeCPP/Core/MathTypes.h"
#include "GeoBridgeCPP/Serialization/GeoSerializer.h"
#include "GeoBridgeFB/cpp/geometry_generated.h"

extern "C" {
GEOBRIDGE_API void* GEOBRIDGE_CALL create_point3d_buffer(const uint8_t* buffer,
                                                         size_t size) {
  // Verify the buffer is not null and has a reasonable size
  if (buffer == nullptr || size == 0) {
    return nullptr;
  }

  // Verify the buffer using FlatBuffers verification
  flatbuffers::Verifier verifier(buffer, size);
  if (!verifier.VerifyBuffer<GeoBridgeFB::Point3dData>(nullptr)) {
    // Buffer validation failed
    return nullptr;
  }

  // Get access to the Point3D data
  auto point3d = GeoBridgeFB::(buffer);
  if (!point3d) {
    return nullptr;
  }

  // Create a copy of the buffer to return
  auto result = new std::vector<uint8_t>(buffer, buffer + size);
  return result;
}

GEOBRIDGE_API void* GEOBRIDGE_CALL create_polyline_buffer(const uint8_t* buffer,
                                                          size_t size) {
  // Simply copy the buffer since it's already a valid FlatBuffer
  auto result = new std::vector<uint8_t>(buffer, buffer + size);
  return result;
}

// GEOBRIDGE_API void* GEOBRIDGE_CALL create_mesh_buffer(const double* vertices,
//                                                       size_t vertex_count,
//                                                       const int* faces,
//                                                       size_t face_count) {
//   GeoBridgeCPP::Mesh mesh;
//   mesh.V = Eigen::Map<const GeoBridgeCPP::MatrixX3d>(vertices, vertex_count,
//   3); mesh.F = Eigen::Map<const GeoBridgeCPP::MatrixX3i>(faces, face_count,
//   3);
//
//   auto buffer = new std::vector<uint8_t>(
//       GeoBridgeCPP::Serialization::serializeMesh(mesh));
//
//   return buffer;
// }

GEOBRIDGE_API int GEOBRIDGE_CALL get_buffer_size(void* buffer) {
  if (buffer == nullptr) {
    return 0;
  }
  auto vec = static_cast<std::vector<uint8_t>*>(buffer);
  return static_cast<int>(vec->size());
}

GEOBRIDGE_API void GEOBRIDGE_CALL free_buffer(void* buffer) {
  delete static_cast<std::vector<uint8_t>*>(buffer);
}
}
