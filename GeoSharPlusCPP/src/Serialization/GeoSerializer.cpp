#include "GeoSharPlusCPP/Serialization/GeoSerializer.h"

#include <combaseapi.h>  // Add this include for CoTaskMemAlloc

#include "GSP_FB/cpp/geometry_generated.h"
#include "GeoSharPlusCPP/Core/MathTypes.h"
#include "flatbuffers/flatbuffers.h"

namespace GeoSharPlusCPP::Serialization {
bool serializePoint(const Vector3d& point, uint8_t*& resBuffer, int& resSize) {
  flatbuffers::FlatBufferBuilder builder;

  auto vec = GSP::FB::Vec3(point[0], point[1], point[2]);
  auto ptOffset = GSP::FB::CreatePointData(builder, &vec);

  auto wrapper = GSP::FB::CreateGSPWrapper(builder, GSP::FB::Data::PointData,
                                           ptOffset.Union());
  builder.Finish(wrapper);

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
  if (!verifier.VerifyBuffer<GSP::FB::GSPWrapper>()) {
    return false;
  }

  auto wrapper = GSP::FB::GetGSPWrapper(buffer);
  if (!wrapper) {
    return false;
  }

  // Get access to the Point3D data
  auto pointData = wrapper->data_as_PointData();
  if (!pointData) {
    return false;
  }

  auto fbPt = pointData->point();
  point = Vector3d(fbPt->x(), fbPt->y(), fbPt->z());
  return true;
}

bool serializePointArray(const std::vector<Vector3d>& points) {
  flatbuffers::FlatBufferBuilder builder(1024);
  // Convert Eigen data to FlatBuffer vectors
  std::vector<GSP::FB::Vec3> pointVector;
  pointVector.reserve(points.size());
  for (const auto& p : points) {
    pointVector.emplace_back(p.x(), p.y(), p.z());
  }
  auto pointArray = builder.CreateVectorOfStructs(pointVector);
  builder.Finish(pointArray);

  return true;
}

bool deserializePointArray(const uint8_t* data, size_t size) {
  auto pointArray =
      flatbuffers::GetRoot<flatbuffers::Vector<GSP::FB::Vec3>>(data);
  std::vector<Vector3d> points;
  points.reserve(pointArray->size());
  for (size_t i = 0; i < pointArray->size(); ++i) {
    const GSP::FB::Vec3& p =
        pointArray->Get(i);  // Fix: Use reference instead of pointer
    points.emplace_back(p.x(), p.y(), p.z());
  }
  return true;
}

// std::vector<uint8_t> serializeMesh(const Mesh& mesh) {
//   flatbuffers::FlatBufferBuilder builder(1024);
//
//   // Convert Eigen data to FlatBuffer vectors
//   std::vector<GSP::FB::Vec3> vertexVector;
//   vertexVector.reserve(mesh.V.rows());
//   for (int i = 0; i < mesh.V.rows(); ++i) {
//     vertexVector.emplace_back(mesh.V(i, 0), mesh.V(i, 1), mesh.V(i, 2));
//   }
//   auto V = builder.CreateVectorOfStructs(vertexVector);
//
//   std::vector<GSP::FB::Int3> faceVector;
//   faceVector.reserve(mesh.F.rows());
//   for (int i = 0; i < mesh.F.rows(); ++i) {
//     faceVector.emplace_back(mesh.F(i, 0), mesh.F(i, 1), mesh.F(i, 2));
//   }
//   auto F = builder.CreateVector(faceVector);
//
//   auto meshData =
//       GSP::FB::CreateMeshDataDirect(builder, &vertexVector, &faceVector);
//
//   builder.Finish(meshData);
//   return {builder.GetBufferPointer(),
//           builder.GetBufferPointer() + builder.GetSize()};
// }
//
// Mesh deserializeMesh(const uint8_t* data, size_t size) {
//   auto meshData = flatbuffers::GetRoot<GSP::FB::MeshData>(data);
//
//   Mesh mesh;
//   mesh.V.resize(meshData->vertices()->size(), 3);
//   for (size_t i = 0; i < meshData->vertices()->size(); ++i) {
//     const auto* v =
//         meshData->vertices()->Get(i);  // Fix: Use pointer dereference
//     mesh.V(i, 0) = v->x();
//     mesh.V(i, 1) = v->y();
//     mesh.V(i, 2) = v->z();
//   }
//
//   mesh.F.resize(meshData->faces()->size(), 3);
//   for (size_t i = 0; i < meshData->faces()->size(); ++i) {
//     const auto* f = meshData->faces()->Get(i);
//     mesh.F(i, 0) = f->x();
//     mesh.F(i, 1) = f->y();
//     mesh.F(i, 2) = f->z();
//   }
//
//   return mesh;
// }
}  // namespace GeoSharPlusCPP::Serialization
