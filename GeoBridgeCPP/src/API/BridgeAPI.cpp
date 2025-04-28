#include "GeoBridgeCPP/API/BridgeAPI.h"

#include "GeoBridgeCPP/Core/MathTypes.h"
#include "GeoBridgeCPP/Serialization/GeoSerializer.h"

extern "C" {

GEOBRIDGE_API void* GEOBRIDGE_CALL create_polyline_buffer(
    const double* vertices, size_t vertex_count, bool isClosed) {
  GeoBridgeCPP::Polyline polyline;
  polyline.vertices =
      Eigen::Map<const GeoBridgeCPP::MatrixX3d>(vertices, vertex_count, 3);
  polyline.isClosed = isClosed;

  auto buffer = new std::vector<uint8_t>(
      GeoBridgeCPP::Serialization::serializePolyline(polyline));

  return buffer;
}

GEOBRIDGE_API void* GEOBRIDGE_CALL create_mesh_buffer(const double* vertices,
                                                      size_t vertex_count,
                                                      const int* faces,
                                                      size_t face_count) {
  GeoBridgeCPP::Mesh mesh;
  mesh.V = Eigen::Map<const GeoBridgeCPP::MatrixX3d>(vertices, vertex_count, 3);
  mesh.F = Eigen::Map<const GeoBridgeCPP::MatrixX3i>(faces, face_count / 3, 3);

  auto buffer = new std::vector<uint8_t>(
      GeoBridgeCPP::Serialization::serializeMesh(mesh));

  return buffer;
}

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
