cmake_minimum_required(VERSION 3.15)

# Create generated directory before use
file(MAKE_DIRECTORY "${FLATBUFFERS_GENERATED_DIR}")

add_library(GeoBridgeRHGH SHARED
    src/Core/Geometry.cpp
    src/Serialization/GeoSerializer.cpp
    src/API/BridgeAPI.cpp
)

# Include directories
target_include_directories(GeoBridgeRHGH PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}/include
    ${FLATBUFFERS_GENERATED_DIR}  # For generated FlatBuffers code
    ${libigl_INCLUDE_DIRS}
)

# Dependencies
target_link_libraries(GeoBridgeRHGH PRIVATE
    Eigen3::Eigen
    igl::igl_core
    flatbuffers::flatbuffers
)

# Make sure we generate schemas before building
add_dependencies(GeoBridgeRHGH GenerateFlatBuffers)

# Platform-specific configuration
if(WIN32)
target_compile_definitions(GeoBridgeRHGH PRIVATE 
        GEOBRIDGE_API=__declspec(dllexport))
    target_compile_options(GeoBridgeRHGH PRIVATE /W4 /EHsc)
else()
    target_compile_definitions(GeoBridgeRHGH PRIVATE 
        GEOBRIDGE_API=__attribute__((visibility("default"))))
    target_compile_options(GeoBridgeRHBH PRIVATE -Wall -Wextra -pedantic)
endif()

# Installation setup (optional)
# install(TARGETS GeoBridgeRHGH
#     LIBRARY DESTINATION lib
#     ARCHIVE DESTINATION lib
#     RUNTIME DESTINATION bin
# )
# install(DIRECTORY include/GeoBridgeRHGH DESTINATION include)
