cmake_minimum_required(VERSION 3.15)

# Check for VCPKG_ROOT environment variable
if(DEFINED ENV{VCPKG_ROOT})
    set(CMAKE_TOOLCHAIN_FILE "$ENV{VCPKG_ROOT}/scripts/buildsystems/vcpkg.cmake")
    message(STATUS "Using vcpkg toolchain file: ${CMAKE_TOOLCHAIN_FILE}")
else()
    message(FATAL_ERROR "VCPKG_ROOT environment variable is not defined. Please set it to your vcpkg installation directory.")
endif()


# Explicitly set vcpkg target triplet if not already set
if(NOT DEFINED VCPKG_TARGET_TRIPLET)
    if(WIN32)
        set(VCPKG_TARGET_TRIPLET "x64-windows" CACHE STRING "vcpkg target triplet")
    elseif(APPLE)
        set(VCPKG_TARGET_TRIPLET "x64-osx" CACHE STRING "vcpkg target triplet")
    elseif(UNIX)
        set(VCPKG_TARGET_TRIPLET "x64-linux" CACHE STRING "vcpkg target triplet")
    endif()
    message(STATUS "Setting vcpkg triplet to: ${VCPKG_TARGET_TRIPLET}")
endif()


# Set C++ standard
project(GeomBridgeRHGH LANGUAGES C CXX)
message(STATUS "Build type: ${CMAKE_BUILD_TYPE}")
message(STATUS "C++ compiler: ${CMAKE_CXX_COMPILER}")
message(STATUS "Generator: ${CMAKE_GENERATOR}")


# Find dependencies
find_package(Eigen3 CONFIG REQUIRED)
find_package(libigl CONFIG REQUIRED)
find_package(Flatbuffers CONFIG REQUIRED)

# Print all CMake variables for debugging
message(STATUS "CMAKE_PREFIX_PATH: ${CMAKE_PREFIX_PATH}")
message(STATUS "CMAKE_MODULE_PATH: ${CMAKE_MODULE_PATH}")


# FlatBuffers code generation setup
set(FLATBUFFERS_SCHEMA_DIR "${CMAKE_CURRENT_SOURCE_DIR}/gbCPP/schemas")
set(FLATBUFFERS_GENERATED_DIR "${CMAKE_CURRENT_BINARY_DIR}/generated")

# Create custom command for schema compilation
add_custom_command(
    OUTPUT "${FLATBUFFERS_GENERATED_DIR}/GeoBridge/FB/geometry_generated.h"
    COMMAND ${Flatbuffers_FLATC_EXECUTABLE}
        -c
        --scoped-enums
        --gen-object-api
        -o "${FLATBUFFERS_GENERATED_DIR}"
        "${FLATBUFFERS_SCHEMA_DIR}/geometry.fbs"
    DEPENDS "${FLATBUFFERS_SCHEMA_DIR}/geometry.fbs"
    COMMENT "Generating FlatBuffers code from schema"
    VERBATIM
)

# Create target to drive code generation
add_custom_target(GenerateFlatBuffers DEPENDS
    "${FLATBUFFERS_GENERATED_DIR}/GeoBridgeRHGH/FB/geometry_generated.h"
)


# Create generated directory before use
add_subdirectory(gbCPP)

