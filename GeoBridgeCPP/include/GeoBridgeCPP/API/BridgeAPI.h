#pragma once
#include "GeoBridgeCPP/Core/Macro.h"

extern "C" {  
GEOBRIDGE_API void* GEOBRIDGE_CALL create_mesh_buffer(const double* vertices,  
                                                     size_t vertex_count,  
                                                     const int* faces,  
                                                     size_t face_count);  

GEOBRIDGE_API void GEOBRIDGE_CALL free_buffer(void* buffer);  
}

