#include "GeoSharPlusCPP/Core/Geometry.h"

namespace GeoSharPlusCPP {

// Polyline operations
double Polyline::length() const {
  double total = 0.0;
  const auto n = vertices.rows();

  for (Eigen::Index i = 1; i < n; ++i) {
    total += (vertices.row(i) - vertices.row(i - 1)).norm();
  }

  return total;
}

// Mesh validation implementation
bool Mesh::validate() const {
  // A mesh without vertices or faces is not valid. This check must come first:
  // Eigen's minCoeff() and maxCoeff() are undefined on an empty matrix.
  if (V.rows() == 0 || F.rows() == 0) {
    return false;
  }

  // Faces must be triangles or quads
  if (F.cols() != 3 && F.cols() != 4) {
    return false;
  }

  // Check face indices are within valid range
  return F.minCoeff() >= 0 && F.maxCoeff() < V.rows();
}

// Area-weighted centroid of the mesh surface
Eigen::Vector3d Mesh::centroid() const {
  if (V.rows() == 0) {
    return Vector3d::Zero();
  }

  Vector3d center = Vector3d::Zero();
  double totalArea = 0.0;

  const auto addTriangle = [&](Eigen::Index a, Eigen::Index b, Eigen::Index c) {
    const Vector3d v1 = V.row(a);
    const Vector3d v2 = V.row(b);
    const Vector3d v3 = V.row(c);

    const double area = 0.5 * (v2 - v1).cross(v3 - v1).norm();
    center += area * (v1 + v2 + v3) / 3.0;
    totalArea += area;
  };

  if (F.cols() == 3 || F.cols() == 4) {
    for (Eigen::Index i = 0; i < F.rows(); ++i) {
      addTriangle(F(i, 0), F(i, 1), F(i, 2));

      // A quad face (a, b, c, d) is the two triangles (a, b, c) and (a, c, d)
      if (F.cols() == 4) {
        addTriangle(F(i, 0), F(i, 2), F(i, 3));
      }
    }
  }

  if (totalArea > 0.0) {
    return center / totalArea;
  }

  // No faces, or only faces with zero area: use the average of the vertices
  return V.colwise().mean();
}

// Bounding box calculation for mesh
std::pair<Vector3d, Vector3d> Mesh::boundingBox() const {
  if (V.rows() == 0) {
    return {Vector3d::Zero(), Vector3d::Zero()};
  }

  Vector3d min = V.colwise().minCoeff();
  Vector3d max = V.colwise().maxCoeff();
  return {min, max};
}
}  // namespace GeoSharPlusCPP
