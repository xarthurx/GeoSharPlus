#include "flatbuffers/flatbuffers.h"
#include "GeoBridgeCPP/Serialization/GeoSerializer.h"
#include "GeoBridgeCPP/FB/geometry_generated.h"

namespace FB = GeoBridgeCPP::FB; 

namespace GeomBridgeCPP::Serialization {
    
    std::vector<uint8_t> serializeMesh(const Mesh& mesh) {
        flatbuffers::FlatBufferBuilder builder(1024);
        
        // Convert Eigen data to FlatBuffer vectors
        auto vertices = builder.CreateVectorOfStructs(
            reinterpret_cast<const FB::Vector3d*>(mesh.V.data()),
            mesh.V.rows()
        );
        
        auto faces = builder.CreateVector(mesh.faces.data(), mesh.faces.size());
        
        auto meshData = FB::CreateMeshDataDirect(
            builder,
            &vertices,
            &faces
        );
        
        builder.Finish(meshData);
        return {builder.GetBufferPointer(), builder.GetBufferPointer() + builder.GetSize()};
    }

    Mesh deserializeMesh(const uint8_t* data, size_t size) {
        auto meshData = FB::GetMeshData(data);
        
        Mesh mesh;
        mesh.vertices.resize(meshData->vertices()->size(), 3);
        std::memcpy(
            mesh.vertices.data(),
            meshData->vertices()->Data(),
            meshData->vertices()->size() * sizeof(FB::Vector3d)
        );
        
        // Similar for faces...
        return mesh;
    }
}
