#pragma once
#include <cstdint>

#include "GeoBridgeCPP/Core/Macro.h"

extern "C" {
// New point3d functions
GEOBRIDGE_API void* GEOBRIDGE_CALL create_point3d_buffer(const uint8_t* buffer,
                                                         size_t size);

GEOBRIDGE_API void* GEOBRIDGE_CALL create_polyline_buffer(const uint8_t* buffer,
                                                          size_t size);

//GEOBRIDGE_API void* GEOBRIDGE_CALL create_mesh_buffer(const double* vertices,
//                                                      size_t vertex_count,
//                                                      const int* faces,
//                                                      size_t face_count);

GEOBRIDGE_API int GEOBRIDGE_CALL get_buffer_size(void* buffer);

GEOBRIDGE_API void GEOBRIDGE_CALL free_buffer(void* buffer);
}