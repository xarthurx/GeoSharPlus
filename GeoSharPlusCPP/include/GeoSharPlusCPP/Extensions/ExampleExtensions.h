#pragma once
#include <cstdint>

#include "GeoSharPlusCPP/Core/Macro.h"

extern "C" {

// ============================================
// GeoSharPlus Example Extensions
// ============================================
// This file contains example extension functions demonstrating:
// - Data serialization/deserialization between C# and C++
// - Point, Point Array, and Mesh roundtrip examples
//
// These are EXAMPLE functions that you can modify or replace.
// Use them as templates for your own extensions.
// ============================================

// --------------------------------
// Point3d Roundtrip Example
// --------------------------------
// Demonstrates sending a single 3D point from C# to C++ and back.
// The point is deserialized, can be processed, then re-serialized.
GSP_API bool GSP_CALL example_point3d_roundtrip(const uint8_t* inBuffer,
                                                 int inSize,
                                                 uint8_t** outBuffer,
                                                 int* outSize);

// --------------------------------
// Point3d Array Roundtrip Example
// --------------------------------
// Demonstrates sending an array of 3D points from C# to C++ and back.
GSP_API bool GSP_CALL example_point3d_array_roundtrip(const uint8_t* inBuffer,
                                                       int inSize,
                                                       uint8_t** outBuffer,
                                                       int* outSize);

// --------------------------------
// Mesh Roundtrip Example
// --------------------------------
// Demonstrates sending a mesh (vertices + faces) from C# to C++ and back.
// Supports both triangle and quad meshes.
GSP_API bool GSP_CALL example_mesh_roundtrip(const uint8_t* inBuffer,
                                              int inSize,
                                              uint8_t** outBuffer,
                                              int* outSize);

}  // extern "C"
