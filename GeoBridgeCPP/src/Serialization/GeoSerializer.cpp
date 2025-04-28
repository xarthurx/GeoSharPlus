#include "GeoBridgeCPP/Serialization/GeoSerializer.h"

#include "GeoBridgeFB/cpp/geometry_generated.h"
#include "flatbuffers/flatbuffers.h"

namespace GeoBridgeCPP::Serialization {

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
