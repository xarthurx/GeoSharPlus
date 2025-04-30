#include "GeoSharPlusCPP/API/BridgeAPI.h"

#include "GeoSharPlusCPP/Core/MathTypes.h"
#include "GeoSharPlusCPP/Serialization/GeoSerializer.h"
#include "GSP.FB/cpp/geometry_generated.h"

#include <iostream>
extern "C" {

GEOBRIDGE_API void* GEOBRIDGE_CALL create_point3d_buffer(const uint8_t* buffer,
                                                         size_t size) {

	flatbuffers::Verifier verifier(buffer, size);
  if (!verifier.VerifyBuffer<GeoBridgeFB::GeoBridgeWrapper>()) {
    // Buffer validation failed
    return nullptr;
  }

  // Verify the buffer is not null and has a reasonable size
  if (buffer == nullptr || size == 0) {
    return nullptr;
  }

  auto wrapper = GeoBridgeFB::GetGeoBridgeWrapper(buffer);
  if (!wrapper) {
    return nullptr;
  }

  // Get access to the Point3D data
  auto pt = wrapper->data_as_PointData(); 
  auto vec = pt->vec();

  // Create a copy of the buffer to return
  auto result = new std::vector<uint8_t>(buffer, buffer + size);

  // Verify the data in the result buffer
  const uint8_t* result_data = result->data();
  auto result_wrapper = GeoBridgeFB::GetGeoBridgeWrapper(result_data);
  if (!result_wrapper) {
    std::cerr << "Error: Could not get wrapper from result buffer" << std::endl;
    delete result;
    return nullptr;
  }
  
  auto result_pt = result_wrapper->data_as_PointData();
  if (!result_pt) {
    std::cerr << "Error: Could not get PointData from result buffer" << std::endl;
    delete result;
    return nullptr;
  }
  
  auto result_vec = result_pt->vec();
  if (!result_vec) {
    std::cerr << "Error: Could not get vector from result buffer" << std::endl;
    delete result;
    return nullptr;
  }
  
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
//   GeoSharPlusCPP::Mesh mesh;
//   mesh.V = Eigen::Map<const GeoSharPlusCPP::MatrixX3d>(vertices, vertex_count,
//   3); mesh.F = Eigen::Map<const GeoSharPlusCPP::MatrixX3i>(faces, face_count,
//   3);
//
//   auto buffer = new std::vector<uint8_t>(
//       GeoSharPlusCPP::Serialization::serializeMesh(mesh));
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
