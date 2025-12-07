#include "GeoSharPlusCPP/Extensions/ExampleExtensions.h"

#include <iostream>
#include <vector>

#include "GSP_FB/cpp/mesh_generated.h"
#include "GSP_FB/cpp/pointArray_generated.h"
#include "GSP_FB/cpp/point_generated.h"
#include "GeoSharPlusCPP/Core/MathTypes.h"
#include "GeoSharPlusCPP/Serialization/Serializer.h"

namespace GS = GeoSharPlusCPP::Serialization;

// ============================================
// GeoSharPlus Example Extensions Implementation
// ============================================
// These example functions demonstrate data I/O between C# and C++.
// 
// Pattern:
// 1. Deserialize input buffer to native C++ types
// 2. Process data (optional - these examples just pass through)
// 3. Serialize result back to output buffer
//
// You can modify these or create your own functions following this pattern.
// ============================================

extern "C" {

GSP_API bool GSP_CALL example_point3d_roundtrip(const uint8_t* inBuffer,
                                                 int inSize,
                                                 uint8_t** outBuffer,
                                                 int* outSize) {
  // Initialize output
  *outBuffer = nullptr;
  *outSize = 0;

  // Step 1: Deserialize the input point
  GeoSharPlusCPP::Vector3d pt;
  if (!GS::deserializePoint(inBuffer, inSize, pt)) {
    return false;
  }

  // Step 2: Process the point (example: you could transform it here)
  // For this example, we just pass it through unchanged.
  // Example processing:
  //   pt.x() += 1.0;  // Translate X by 1
  //   pt = pt * 2.0;  // Scale by 2

  // Step 3: Serialize the result
  if (!GS::serializePoint(pt, *outBuffer, *outSize)) {
    if (*outBuffer) delete[] *outBuffer;
    *outBuffer = nullptr;
    *outSize = 0;
    return false;
  }

  return true;
}

GSP_API bool GSP_CALL example_point3d_array_roundtrip(const uint8_t* inBuffer,
                                                       int inSize,
                                                       uint8_t** outBuffer,
                                                       int* outSize) {
  // Initialize output
  *outBuffer = nullptr;
  *outSize = 0;

  // Step 1: Deserialize the input point array
  std::vector<GeoSharPlusCPP::Vector3d> points;
  if (!GS::deserializePointArray(inBuffer, inSize, points)) {
    return false;
  }

  // Step 2: Process the points (example: you could transform them here)
  // For this example, we just pass them through unchanged.
  // Example processing:
  //   for (auto& pt : points) {
  //     pt.z() += 10.0;  // Move all points up by 10
  //   }

  // Step 3: Serialize the result
  if (!GS::serializePointArray(points, *outBuffer, *outSize)) {
    if (*outBuffer) delete[] *outBuffer;
    *outBuffer = nullptr;
    *outSize = 0;
    return false;
  }

  return true;
}

GSP_API bool GSP_CALL example_mesh_roundtrip(const uint8_t* inBuffer,
                                              int inSize,
                                              uint8_t** outBuffer,
                                              int* outSize) {
  // Initialize output
  *outBuffer = nullptr;
  *outSize = 0;

  // Step 1: Deserialize the input mesh
  GeoSharPlusCPP::Mesh mesh;
  if (!GS::deserializeMesh(inBuffer, inSize, mesh)) {
    return false;
  }

  // Step 2: Process the mesh (example: you could modify it here)
  // For this example, we just pass it through unchanged.
  // 
  // The mesh structure:
  //   mesh.V - Eigen::MatrixXd (N x 3) - vertex positions
  //   mesh.F - Eigen::MatrixXi (M x 3 or M x 4) - face indices (tri or quad)
  //
  // Example processing:
  //   mesh.V.col(2) *= 2.0;  // Scale Z coordinates by 2
  //   mesh.V.rowwise() += Eigen::RowVector3d(1, 0, 0);  // Translate

  // Step 3: Serialize the result
  if (!GS::serializeMesh(mesh, *outBuffer, *outSize)) {
    if (*outBuffer) delete[] *outBuffer;
    *outBuffer = nullptr;
    *outSize = 0;
    return false;
  }

  return true;
}

}  // extern "C"
