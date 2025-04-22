extern "C" {
__declspec(dllexport) void* create_mesh_buffer(
    const double* vertices, 
    size_t vertex_count,
    const int* faces,
    size_t face_count);
    
__declspec(dllexport) void free_buffer(void* buffer);
}
