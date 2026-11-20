cmake_minimum_required(VERSION 3.21)

set(PROJECT_NAME GeoSharPlusCPP)

# Set the Visual Studio toolset to the latest version (optional, for local development)
# This is only applied when explicitly requested via -DUSE_VS2026_TOOLSET=ON
if(MSVC AND USE_VS2026_TOOLSET)
    set(CMAKE_GENERATOR_TOOLSET "v145" CACHE STRING "Visual Studio toolset version" FORCE)
    message(STATUS "Using Visual Studio 2026 (Insider) toolset: ${CMAKE_GENERATOR_TOOLSET}")
endif()

# Check for VCPKG_ROOT environment variable
if(DEFINED ENV{VCPKG_ROOT})
    set(CMAKE_TOOLCHAIN_FILE "$ENV{VCPKG_ROOT}/scripts/buildsystems/vcpkg.cmake")
    message(STATUS "Using vcpkg toolchain file: ${CMAKE_TOOLCHAIN_FILE}")
elseif(DEFINED CMAKE_TOOLCHAIN_FILE)
    message(STATUS "Using provided toolchain file: ${CMAKE_TOOLCHAIN_FILE}")
else()
    message(FATAL_ERROR "VCPKG_ROOT environment variable is not defined and no CMAKE_TOOLCHAIN_FILE provided. Please set VCPKG_ROOT to your vcpkg installation directory.")
endif()


# Explicitly set the vcpkg target triplet on Windows if not already set.
# On macOS and Linux, vcpkg selects the triplet from the host architecture
# (arm64-osx on Apple Silicon, x64-osx on Intel), unless a preset sets it.
if(NOT DEFINED VCPKG_TARGET_TRIPLET AND WIN32)
    set(VCPKG_TARGET_TRIPLET "x64-windows" CACHE STRING "vcpkg target triplet")
    message(STATUS "Setting vcpkg triplet to: ${VCPKG_TARGET_TRIPLET}")
endif()


# Set C++ standard
project(${PROJECT_NAME} LANGUAGES C CXX)

# Set C++ standard to C++20
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

message(STATUS "Build type: ${CMAKE_BUILD_TYPE}")
message(STATUS "C++ compiler: ${CMAKE_CXX_COMPILER}")
message(STATUS "C++ standard: C++${CMAKE_CXX_STANDARD}")
message(STATUS "Generator: ${CMAKE_GENERATOR}")

# Find dependencies
find_package(Eigen3 CONFIG REQUIRED)
message(STATUS "Found Eigen3 ${Eigen3_VERSION}: ${Eigen3_DIR}")

find_package(libigl CONFIG REQUIRED)
message(STATUS "Found libigl: ${libigl_DIR}")

find_package(Flatbuffers CONFIG REQUIRED)
message(STATUS "Found Flatbuffers ${Flatbuffers_VERSION}: ${Flatbuffers_DIR}")

# Find the flatc compiler. Search the vcpkg tools folders first, because a
# flatc from the system PATH can have a different version.
if(NOT EXISTS "${FLATBUFFERS_FLATC_EXECUTABLE}")
    unset(FLATBUFFERS_FLATC_EXECUTABLE CACHE)
    find_program(FLATBUFFERS_FLATC_EXECUTABLE flatc
        HINTS
            "${VCPKG_INSTALLED_DIR}/${VCPKG_HOST_TRIPLET}/tools/flatbuffers"
            "${VCPKG_INSTALLED_DIR}/${VCPKG_TARGET_TRIPLET}/tools/flatbuffers"
            "${Flatbuffers_DIR}/../../tools/flatbuffers"
        PATH_SUFFIXES bin
    )
endif()
if(NOT FLATBUFFERS_FLATC_EXECUTABLE)
    message(FATAL_ERROR "flatc compiler not found. Please ensure Flatbuffers is properly installed.")
endif()
message(STATUS "flatc executable path: ${FLATBUFFERS_FLATC_EXECUTABLE}")

# The generated code contains a version check. flatc must have the same
# version as the FlatBuffers library, or the build fails with a static_assert.
execute_process(
    COMMAND "${FLATBUFFERS_FLATC_EXECUTABLE}" --version
    OUTPUT_VARIABLE FLATC_VERSION_OUTPUT
    OUTPUT_STRIP_TRAILING_WHITESPACE
)
string(REGEX MATCH "[0-9]+\\.[0-9]+\\.[0-9]+" FLATC_VERSION "${FLATC_VERSION_OUTPUT}")
if(Flatbuffers_VERSION AND NOT FLATC_VERSION VERSION_EQUAL Flatbuffers_VERSION)
    message(FATAL_ERROR
        "flatc version (${FLATC_VERSION}) is not equal to the FlatBuffers library version "
        "(${Flatbuffers_VERSION}). Use the flatc that vcpkg installs.")
endif()

# Print all CMake variables for debugging
message(STATUS "CMAKE_PREFIX_PATH: ${CMAKE_PREFIX_PATH}")
message(STATUS "CMAKE_MODULE_PATH: ${CMAKE_MODULE_PATH}")

###########################################
# FLATBUFFER GENERATION
###########################################
# FlatBuffers code generation setup
# Core schemas are in schema/. Your own schemas are in schema/extensions/.
file(GLOB FLATBUFFERS_SCHEMAS
    "${CMAKE_CURRENT_SOURCE_DIR}/schema/*.fbs"
    "${CMAKE_CURRENT_SOURCE_DIR}/schema/extensions/*.fbs"
)
# Lets a schema in schema/extensions/ use: include "base.fbs";
set(FLATBUFFERS_INCLUDE_DIR "${CMAKE_CURRENT_SOURCE_DIR}/schema")
set(FLATBUFFERS_OUTPUT_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../generated/GSP_FB")

# Create output directories
file(MAKE_DIRECTORY "${FLATBUFFERS_OUTPUT_DIR}")
file(MAKE_DIRECTORY "${FLATBUFFERS_OUTPUT_DIR}/cpp")
file(MAKE_DIRECTORY "${FLATBUFFERS_OUTPUT_DIR}/csharp")

# Process each schema file
message(STATUS "Found schema files: ${FLATBUFFERS_SCHEMAS}")
foreach(SCHEMA_FILE ${FLATBUFFERS_SCHEMAS})
    get_filename_component(SCHEMA_NAME ${SCHEMA_FILE} NAME_WE)
    message(STATUS "Processing schema: ${SCHEMA_NAME}")
    
    # Generate C++ code
    message(STATUS "Generating Flatbuffers C++ code for ${SCHEMA_NAME}...")
    execute_process(
        COMMAND "${FLATBUFFERS_FLATC_EXECUTABLE}"
        --cpp
        --scoped-enums
        -I "${FLATBUFFERS_INCLUDE_DIR}"
        -o "${FLATBUFFERS_OUTPUT_DIR}/cpp"
        "${SCHEMA_FILE}"
        RESULT_VARIABLE FLATC_CPP_RESULT
    )

    if(NOT FLATC_CPP_RESULT EQUAL 0)
        message(FATAL_ERROR "Failed to generate C++ Flatbuffers code for ${SCHEMA_NAME}")
    endif()

    # Generate C# code
    message(STATUS "Generating Flatbuffers C# code for ${SCHEMA_NAME}...")
    execute_process(
        COMMAND "${FLATBUFFERS_FLATC_EXECUTABLE}"
        --csharp
        --scoped-enums
        --gen-onefile
        --gen-object-api
        -I "${FLATBUFFERS_INCLUDE_DIR}"
        -o "${FLATBUFFERS_OUTPUT_DIR}/csharp"
        "${SCHEMA_FILE}"
        RESULT_VARIABLE FLATC_CSHARP_RESULT
    )

    if(NOT FLATC_CSHARP_RESULT EQUAL 0)
        message(FATAL_ERROR "Failed to generate C# Flatbuffers code for ${SCHEMA_NAME}")
    endif()
endforeach()

message(STATUS "Flatbuffers generation complete")


###########################################
# GEOSHARPLUS TARGET
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
    ${CMAKE_CURRENT_SOURCE_DIR}/../generated  # For generated FlatBuffers code
    ${libigl_INCLUDE_DIRS}
)

# Rest of your CMakeLists.txt remains the same
# Dependencies
target_link_libraries(${PROJECT_NAME} PRIVATE
    Eigen3::Eigen
    igl::igl_core
    flatbuffers::flatbuffers
)

# No need to add dependency since we generate at configure time

# Platform-specific configuration
if(WIN32)
    target_compile_definitions(${PROJECT_NAME} PRIVATE GEOSHARPLUS_EXPORTS)
    target_compile_options(${PROJECT_NAME} PRIVATE /W4 /EHsc)
else()
    target_compile_options(${PROJECT_NAME} PRIVATE -Wall -Wextra -pedantic)
endif()

############################################
# Post-build steps
############################################
# Create bin directory if it doesn't exist
# file(MAKE_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/../bin")
file(MAKE_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/../cppPrebuild")

# Add post-build command to copy the DLL to the bin directory
add_custom_command(
    TARGET ${PROJECT_NAME} POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy 
        $<TARGET_FILE:${PROJECT_NAME}> 
        "${CMAKE_CURRENT_SOURCE_DIR}/../cppPrebuild/$<TARGET_FILE_NAME:${PROJECT_NAME}>"
    COMMENT "Copying ${PROJECT_NAME} DLL to bin directory"
)
