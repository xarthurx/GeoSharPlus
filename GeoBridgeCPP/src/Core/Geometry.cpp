#include "GeomBridgeCPP/Core/Geometry.h"

namespace GeoBridgeCPP {

// Mesh validation implementation
bool Mesh::validate() const {
  const auto n_vertices = V.rows();

  // Check face indices are within valid range
  return F.maxCoeff() < n_vertices && 
  F.minCoeff() >= 0 &&
  (F.cols() == 3 || F.cols() == 4); // triangles or quads
}

// Polyline operations
double Polyline::length() const {
  double total = 0.0;
  const auto n = vertices.rows();

  for(Eigen::Index i = 1; i < n; ++i) {
    total += (vertices.row(i) - vertices.row(i-1)).norm();
  }

  if(isClosed && n > 1) {
    total += (vertices.row(0) - vertices.row(n-1)).norm();
  }

  return total;
}

// Bounding box calculation for mesh
std::pair<Vector3d, Vector3d> Mesh::boundingBox() const {
  if(vertices.rows() == 0) {
    return {Vector3d::Zero(), Vector3d::Zero()};
  }

  Vector3d min = vertices.colwise().minCoeff();
  Vector3d max = vertices.colwise().maxCoeff();
  return {min, max};
}

} // namespace GeomBridge
