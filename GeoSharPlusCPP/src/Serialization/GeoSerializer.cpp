#include "GeoSharPlusCPP/Serialization/GeoSerializer.h"

#include <combaseapi.h>  // Add this include for CoTaskMemAlloc

#include "GSP_FB/cpp/mesh_generated.h"
#include "GSP_FB/cpp/pointArray_generated.h"
#include "GSP_FB/cpp/point_generated.h"
#include "GeoSharPlusCPP/Core/MathTypes.h"
#include "flatbuffers/flatbuffers.h"

namespace GeoSharPlusCPP::Serialization {
bool serializePoint(const Vector3d& point, uint8_t*& resBuffer, int& resSize) {
  flatbuffers::FlatBufferBuilder builder;

  auto vec = GSP::FB::Vec3(point[0], point[1], point[2]);
  auto ptOffset = GSP::FB::CreatePointData(builder, &vec);
  builder.Finish(ptOffset);

  // Copy the serialized data to the provided buffer
  resSize = builder.GetSize();
  resBuffer = static_cast<uint8_t*>(CoTaskMemAlloc(resSize));
  if (!resBuffer) {
    return false;  // Handle allocation failure
  }
  std::memcpy(resBuffer, builder.GetBufferPointer(), resSize);

  return true;
}

bool deserializePoint(const uint8_t* buffer, int size, Vector3d& point) {
  flatbuffers::Verifier verifier(buffer, size);
  if (!verifier.VerifyBuffer<GSP::FB::PointData>()) {
    return false;
  }

  auto ptData = GSP::FB::GetPointData(buffer);
  if (!ptData) {
    return false;
  }

  point = Vector3d(ptData->point()->x(), ptData->point()->y(),
                   ptData->point()->z());
  return true;
}

bool serializePointArray(const std::vector<Vector3d>& points,
                         uint8_t*& resBuffer, int& resSize) {
  flatbuffers::FlatBufferBuilder builder;

  // Convert Eigen data to FlatBuffer vectors
  std::vector<GSP::FB::Vec3> pointVector;
  pointVector.reserve(points.size());
  for (const auto& p : points) {
    pointVector.emplace_back(p.x(), p.y(), p.z());
  }

  auto vecVector = builder.CreateVectorOfStructs(pointVector);
  auto ptArray = GSP::FB::CreatePointArrayData(builder, vecVector);
  builder.Finish(ptArray);

  // Copy the serialized data to the provided buffer
  resSize = builder.GetSize();
  resBuffer = static_cast<uint8_t*>(CoTaskMemAlloc(resSize));
  if (!resBuffer) {
    return false;  // Handle allocation failure
  }
  std::memcpy(resBuffer, builder.GetBufferPointer(), resSize);

  return true;
}

bool deserializePointArray(const uint8_t* data, int size,
                           std::vector<Vector3d>& pointArray) {
  // Verify the buffer integrity
  flatbuffers::Verifier verifier(data, size);
  if (!verifier.VerifyBuffer<GSP::FB::PointArrayData>()) {
    return false;
  }

  // Get the vector from the buffer
  auto ptArrayData = GSP::FB::GetPointArrayData(data);
  if (!ptArrayData) {
    return false;
  }

  auto points = ptArrayData->points();
  // Clear the output vector and reserve space
  pointArray.clear();

  // Convert each FlatBuffers Vec3 to an Eigen Vector3d
  for (size_t i = 0; i < points->size(); i++) {
    auto point = points->Get(i);
    pointArray.emplace_back(point->x(), point->y(), point->z());
  }

  return true;
}
}  // namespace GeoSharPlusCPP::Serialization
