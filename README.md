# GeoBridgeCPP

This cpp sub-project use `vcpkg`'s manifest mode.
Please install the `vcpkg` on your system and set the `VCPKG_ROOT` environment variable to the path of your `vcpkg` installation.

To install the required packages and configure the project, run the following commands in your terminal:
```pwsh
# Install the required packages	
vcpkg install

# Cmake configure
cmake -B build .

# Generate the flatbuffer header file (needed if you modify the code)
cmake --build build --target GeneratedFlatBuffers
```
