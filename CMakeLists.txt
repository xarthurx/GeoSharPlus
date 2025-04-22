cmake_minimum_required(VERSION 3.15)
project(GeomBridgeRHGH LANGUAGES C CXX)


# FlatBuffers code generation
flatbuffers_generate_headers(
    GEOMETRY_SCHEMA_HEADERS 
    SCHEMA "${CMAKE_CURRENT_SOURCE_DIR}/schemas/geometry.fbs"
)

add_library(GeomBridgeRHGH SHARED
    src/Core/Geometry.cpp
    src/Serialization/GeometrySerializer.cpp
    src/API/BridgeAPI.cpp
    ${GEOMETRY_SCHEMA_HEADERS}
)

target_include_directories(GeomBridge PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}/include
    ${CMAKE_CURRENT_BINARY_DIR}  # For generated FlatBuffers headers
)

target_link_libraries(GeomBridge PRIVATE
    Eigen3::Eigen
    libigl::core
    flatbuffers
)

# Windows DLL export handling
if(WIN32)
    target_compile_definitions(GeomBridge PRIVATE GEOMBRIDGE_API=__declspec(dllexport))
else()
    target_compile_definitions(GeomBridge PRIVATE GEOMBRIDGE_API=__attribute__((visibility("default"))))
endif()
