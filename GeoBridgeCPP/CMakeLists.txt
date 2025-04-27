cmake_minimum_required(VERSION 3.15)


set(PROJECT_NAME GeomBridgeCPP)

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
project(${PROJECT_NAME} LANGUAGES C CXX)

# Set C++ standard
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

message(STATUS "Build type: ${CMAKE_BUILD_TYPE}")
message(STATUS "C++ compiler: ${CMAKE_CXX_COMPILER}")
message(STATUS "Generator: ${CMAKE_GENERATOR}")

# Find dependencies
find_package(Eigen3 CONFIG REQUIRED)
message(STATUS "Found Eigen3: ${EIGEN3_INCLUDE_DIR}")

find_package(libigl CONFIG REQUIRED)
message(STATUS "Found libigl: ${libigl_DIR}")

find_package(Flatbuffers CONFIG REQUIRED)
message(STATUS "Found Flatbuffers: ${Flatbuffers_DIR}")

# Print all CMake variables for debugging
message(STATUS "CMAKE_PREFIX_PATH: ${CMAKE_PREFIX_PATH}")
message(STATUS "CMAKE_MODULE_PATH: ${CMAKE_MODULE_PATH}")

###########################################
# FLATBUFFER TARGET
###########################################
# FlatBuffers code generation setup
set(FLATBUFFERS_SCHEMA_DIR "${CMAKE_CURRENT_SOURCE_DIR}/gbCPP/schemas")
set(FLATBUFFERS_GENERATED_DIR "${CMAKE_CURRENT_BINARY_DIR}/generated")

# Create custom command for schema compilation
add_custom_command(
    OUTPUT "${FLATBUFFERS_GENERATED_DIR}/GeoBridgeCPP/FB/geometry_generated.h"
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
    "${FLATBUFFERS_GENERATED_DIR}/GeoBridgeCPP/FB/geometry_generated.h"
)

# Create generated directory before use
file(MAKE_DIRECTORY "${FLATBUFFERS_GENERATED_DIR}")


###########################################
# GEOBRIDGECPP TARGET
###########################################
# Add header files to the target
file(GLOB_RECURSE HEADERS CONFIGURE_DEPENDS 
    "${CMAKE_CURRENT_SOURCE_DIR}/include/*.h"
)

file(GLOB_RECURSE SOURCES CONFIGURE_DEPENDS 
    "${CMAKE_CURRENT_SOURCE_DIR}/src/*.cpp"
)

add_library(${PROJECT_NAME} SHARED ${HEADERS} ${SOURCES})

# Organize VS folders
source_group("Header Files" FILES ${HEADERS})
source_group("Source Files" FILES ${SOURCES})

# Critical include path
target_include_directories(${PROJECT_NAME} PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}/include
    ${FLATBUFFERS_GENERATED_DIR}  # For generated FlatBuffers code
    ${libigl_INCLUDE_DIRS}
)

# Rest of your CMakeLists.txt remains the same
# Dependencies
target_link_libraries(${PROJECT_NAME} PRIVATE
    Eigen3::Eigen
    igl::igl_core
    flatbuffers::flatbuffers
    # Threads::Threads
)

# Make sure we generate schemas before building
add_dependencies(${PROJECT_NAME} GenerateFlatBuffers)

# Platform-specific configuration
if(WIN32)
    target_compile_definitions(${PROJECT_NAME} PRIVATE 
        GEOBRIDGE_API=__declspec(dllexport))
    target_compile_options(${PROJECT_NAME} PRIVATE /W4 /EHsc)
else()
    target_compile_definitions(${PROJECT_NAME} PRIVATE 
        GEOBRIDGE_API=__attribute__((visibility("default"))))
    target_compile_options(${PROJECT_NAME} PRIVATE -Wall -Wextra -pedantic)
endif()
