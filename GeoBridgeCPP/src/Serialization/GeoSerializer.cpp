#include "GeoBridgeCPP/Serialization/GeoSerializer.h"

#include "GeoBridgeFB/cpp/geometry_generated.h"
#include "flatbuffers/flatbuffers.h"
#include "flatbuffers/idl.h"

namespace GeoBridgeCPP::Serialization {

std::vector<uint8_t> serializePointArray(const std::vector<Vector3d>& points) {
  flatbuffers::FlatBufferBuilder builder(1024);
  // Convert Eigen data to FlatBuffer vectors
  std::vector<GeoBridgeFB::Vector3d> pointVector;
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
      flatbuffers::GetRoot<flatbuffers::Vector<GeoBridgeFB::Vector3d>>(data);
  std::vector<Vector3d> points;
  points.reserve(pointArray->size());
  for (size_t i = 0; i < pointArray->size(); ++i) {
    const GeoBridgeFB::Vector3d& p =
        pointArray->Get(i);  // Fix: Use reference instead of pointer
    points.emplace_back(p.x(), p.y(), p.z());
  }
  return points;
}

std::vector<uint8_t> serializePolyline(const Polyline& polyline) {
  flatbuffers::FlatBufferBuilder builder(1024);

  // Convert Eigen data to FlatBuffer vectors
  std::vector<GeoBridgeFB::Vector3d> pointVector;
  pointVector.reserve(polyline.vertices.rows());
  for (int i = 0; i < polyline.vertices.rows(); ++i) {
    pointVector.emplace_back(polyline.vertices(i, 0), polyline.vertices(i, 1),
                             polyline.vertices(i, 2));
  }
  auto points = builder.CreateVectorOfStructs(pointVector);

  auto polylineData = GeoBridgeFB::CreatePolylineData(builder, points);

  builder.Finish(polylineData);
  return {builder.GetBufferPointer(),
          builder.GetBufferPointer() + builder.GetSize()};
}

Polyline deserializePolyline(const uint8_t* data, size_t size) {
  auto polylineData = flatbuffers::GetRoot<GeoBridgeFB::PolylineData>(data);

  Polyline polyline;
  polyline.vertices.resize(polylineData->points()->size(), 3);
  for (size_t i = 0; i < polylineData->points()->size(); ++i) {
    const auto* p = polylineData->points()->Get(i);
    polyline.vertices(i, 0) = p->x();
    polyline.vertices(i, 1) = p->y();
    polyline.vertices(i, 2) = p->z();
  }

  return polyline;
}

std::vector<uint8_t> serializeMesh(const Mesh& mesh) {
  flatbuffers::FlatBufferBuilder builder(1024);

  // Convert Eigen data to FlatBuffer vectors
  std::vector<GeoBridgeFB::Vector3d> vertexVector;
  vertexVector.reserve(mesh.V.rows());
  for (int i = 0; i < mesh.V.rows(); ++i) {
    vertexVector.emplace_back(mesh.V(i, 0), mesh.V(i, 1), mesh.V(i, 2));
  }
  auto V = builder.CreateVectorOfStructs(vertexVector);

  std::vector<int32_t> faceVector(mesh.F.data(), mesh.F.data() + mesh.F.size());
  auto F = builder.CreateVector(faceVector);

  auto meshData =
      GeoBridgeFB::CreateMeshDataDirect(builder, &vertexVector, &faceVector);

  builder.Finish(meshData);
  return {builder.GetBufferPointer(),
          builder.GetBufferPointer() + builder.GetSize()};
}

Mesh deserializeMesh(const uint8_t* data, size_t size) {
  auto meshData = flatbuffers::GetRoot<GeoBridgeFB::MeshData>(data);

  Mesh mesh;
  mesh.V.resize(meshData->vertices()->size(), 3);
  for (size_t i = 0; i < meshData->vertices()->size(); ++i) {
    const auto* v =
        meshData->vertices()->Get(i);  // Fix: Use pointer dereference
    mesh.V(i, 0) = v->x();
    mesh.V(i, 1) = v->y();
    mesh.V(i, 2) = v->z();
  }

  mesh.F.resize(meshData->faces()->size() / 3, 3);
  std::memcpy(mesh.F.data(), meshData->faces()->data(),
              meshData->faces()->size() * sizeof(int32_t));

  return mesh;
}
}  // namespace GeoBridgeCPP::Serialization
