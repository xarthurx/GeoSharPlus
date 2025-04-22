#pragma once
#include "MathTypes.h"
#include <vector>

namespace GeomBridgeRHGH {
    struct Polyline {
        MatrixX3d vertices;
        bool isClosed;
    };

    struct Mesh {
        // Mesh data: V - vertices, F - faces (triangles or quads)
        MatrixX3d V;
        MatrixX3i F;
        
        // Optional per-vertex data
        Eigen::VectorXd C; 
    };
}
