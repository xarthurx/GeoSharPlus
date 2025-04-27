#include "GeomBridgeCPP/API/BridgeAPI.h"
#include "GeomBridgeCPP/Serialization/GeometrySerializer.h"

extern "C" {
    
    GEOMBRIDGE_API void* GEOMBRIDGE_CALL create_mesh_buffer(
        const double* vertices,
        size_t vertex_count,
        const int* faces,
        size_t face_count)
    {
        GeomBridge::Mesh mesh;
        mesh.vertices = Eigen::Map<const MatrixX3d>(
            vertices, 
            vertex_count, 
            3
        );
        
        mesh.faces = Eigen::Map<const MatrixX3i>(
            faces,
            face_count / 3,
            3
        );
        
        auto buffer = new std::vector<uint8_t>(
            Serialization::serializeMesh(mesh)
        );
        
        return buffer;
    }

    GEOMBRIDGE_API void GEOMBRIDGE_CALL free_buffer(void* buffer) {
        delete static_cast<std::vector<uint8_t>*>(buffer);
    }
}
