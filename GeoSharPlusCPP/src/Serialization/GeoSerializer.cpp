#include "GeoSharPlusCPP/Serialization/GeoSerializer.h"

#include "GeoBridgeFB/cpp/geometry_generated.h"
#include "flatbuffers/flatbuffers.h"
#include "flatbuffers/idl.h"

namespace GeoSharPlusCPP::Serialization {

std::vector<uint8_t> serializePointArray(const std::vector<Vector3d>& points) {
  flatbuffers::FlatBufferBuilder builder(1024);
  // Convert Eigen data to FlatBuffer vectors
  std::vector<GeoBridgeFB::Vec3> pointVector;
  pointVector.reserve(points.size());
  for (const auto& p : points) {
    pointVector.emplace_back(p.x(), p.y(), p.z());
  }
  auto pointArray = builder.CreateVectorOfStructs(pointVector);
  builder.Finish(pointArray);
  return {builder.GetBufferPointer(),
          builder.GetBufferPointer() + builder.GetSize()};
}

std::vector<Vector3d> deserializePointArray(const uint8_t* data, size_t size) {
  auto pointArray =
      flatbuffers::GetRoot<flatbuffers::Vector<GeoBridgeFB::Vec3>>(data);
  std::vector<Vector3d> points;
  points.reserve(pointArray->size());
  for (size_t i = 0; i < pointArray->size(); ++i) {
    const GeoBridgeFB::Vec3& p =
        pointArray->Get(i);  // Fix: Use reference instead of pointer
    points.emplace_back(p.x(), p.y(), p.z());
  }
  return points;
}

//std::vector<uint8_t> serializeMesh(const Mesh& mesh) {
//  flatbuffers::FlatBufferBuilder builder(1024);
//
//  // Convert Eigen data to FlatBuffer vectors
//  std::vector<GeoBridgeFB::Vec3> vertexVector;
//  vertexVector.reserve(mesh.V.rows());
//  for (int i = 0; i < mesh.V.rows(); ++i) {
//    vertexVector.emplace_back(mesh.V(i, 0), mesh.V(i, 1), mesh.V(i, 2));
//  }
//  auto V = builder.CreateVectorOfStructs(vertexVector);
//
//  std::vector<GeoBridgeFB::Int3> faceVector;
//  faceVector.reserve(mesh.F.rows());
//  for (int i = 0; i < mesh.F.rows(); ++i) {
//    faceVector.emplace_back(mesh.F(i, 0), mesh.F(i, 1), mesh.F(i, 2));
//  }
//  auto F = builder.CreateVector(faceVector);
//
//  auto meshData =
//      GeoBridgeFB::CreateMeshDataDirect(builder, &vertexVector, &faceVector);
//
//  builder.Finish(meshData);
//  return {builder.GetBufferPointer(),
//          builder.GetBufferPointer() + builder.GetSize()};
//}
//
//Mesh deserializeMesh(const uint8_t* data, size_t size) {
//  auto meshData = flatbuffers::GetRoot<GeoBridgeFB::MeshData>(data);
//
//  Mesh mesh;
//  mesh.V.resize(meshData->vertices()->size(), 3);
//  for (size_t i = 0; i < meshData->vertices()->size(); ++i) {
//    const auto* v =
//        meshData->vertices()->Get(i);  // Fix: Use pointer dereference
//    mesh.V(i, 0) = v->x();
//    mesh.V(i, 1) = v->y();
//    mesh.V(i, 2) = v->z();
//  }
//
//  mesh.F.resize(meshData->faces()->size(), 3);
//  for (size_t i = 0; i < meshData->faces()->size(); ++i) {
//    const auto* f = meshData->faces()->Get(i);
//    mesh.F(i, 0) = f->x();
//    mesh.F(i, 1) = f->y();
//    mesh.F(i, 2) = f->z();
//  }
//
//  return mesh;
//}
}  // namespace GeoSharPlusCPP::Serialization
