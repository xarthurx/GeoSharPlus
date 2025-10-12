# GeoSharPlus

A high-performance, Win/Mac cross-platform, C# ↔ C++ interoperability library for geometry data exchange using [FlatBuffers](https://flatbuffers.dev/). Originally developed for the Rhino/Grasshopper platform, this library enables efficient serialization and transmission of geometric data between managed (.NET) and unmanaged (C++) environments. While designed with CAD applications in mind, it can be used independently in any scenario requiring high-performance geometry data exchange.

## Features

- 🚀 **High Performance**: Zero-copy serialization using FlatBuffers
- 🔄 **Bi-directional**: Seamless data exchange between C# and C++
- 📐 **Rich Geometry Support**: Points, point arrays, meshes, and more
- 🛠 **Extensible**: Easy to add new geometric types
- 🎯 **Cross-Platform**: Windows and macOS support
- 🏗️ **CAD Integration**: Built-in support for CAD geometry types (currently Rhino, contributions for other CAD platforms welcome!)

## Quick Examples

### Point3d Round-Trip

**C# Side:**

```csharp
using GSP;
using Rhino.Geometry;

// Create a point
var originalPoint = new Point3d(1.5, 2.7, 3.9);

// Serialize to buffer for C++
byte[] buffer = Wrapper.ToPointBuffer(originalPoint);

// Send to C++ and get result back
NativeBridge.Point3dRoundTrip(buffer, buffer.Length, out IntPtr outPtr, out int outSize);

// Convert back to C#
byte[] resultBuffer = new byte[outSize];
Marshal.Copy(outPtr, resultBuffer, 0, outSize);
Marshal.FreeCoTaskMem(outPtr);

Point3d resultPoint = Wrapper.FromPointBuffer(resultBuffer);
Console.WriteLine($"Original: {originalPoint}, Result: {resultPoint}");
```

**C++ Side:**

```cpp
extern "C" {
GSP_API bool GSP_CALL point3d_roundtrip(const uint8_t* inBuffer,
                                        int inSize,
                                        uint8_t** outBuffer,
                                        int* outSize) {
  // Deserialize from FlatBuffer
  GeoSharPlusCPP::Vector3d point;
  if (!GS::deserializePoint(inBuffer, inSize, point)) {
    return false;
  }

  // Process the point (e.g., transform, analyze, etc.)
  // ... your C++ geometry operations here ...

  // Serialize back to FlatBuffer
  return GS::serializePoint(point, *outBuffer, *outSize);
}
}
```

### Point Array Processing

**C# Side:**

```csharp
// Create point array
var points = new Point3d[] {
    new Point3d(0, 0, 0),
    new Point3d(1, 1, 1),
    new Point3d(2, 2, 2)
};

// Serialize and send to C++
byte[] buffer = Wrapper.ToPointArrayBuffer(points);
NativeBridge.Point3dArrayRoundTrip(buffer, buffer.Length, out IntPtr outPtr, out int outSize);

// Get processed results
byte[] resultBuffer = new byte[outSize];
Marshal.Copy(outPtr, resultBuffer, 0, outSize);
Marshal.FreeCoTaskMem(outPtr);

Point3d[] processedPoints = Wrapper.FromPointArrayBuffer(resultBuffer);
```

**C++ Side:**

```cpp
GSP_API bool GSP_CALL point3d_array_roundtrip(const uint8_t* inBuffer,
                                              int inSize,
                                              uint8_t** outBuffer,
                                              int* outSize) {
  // Deserialize point array
  std::vector<GeoSharPlusCPP::Vector3d> points;
  if (!GS::deserializePointArray(inBuffer, inSize, points)) {
    return false;
  }

  // Process the points (e.g., apply transformations, filtering, etc.)
  for (auto& point : points) {
    // Example: scale all points by 2
    point *= 2.0;
  }

  // Serialize back
  return GS::serializePointArray(points, *outBuffer, *outSize);
}
```

## Setup

### Requirements

- **vcpkg**: The C++ sub-project uses vcpkg's manifest mode for dependency management
- **Visual Studio** (Windows) or **CMake + compatible compiler** (cross-platform)
- **.NET 7.0** or later

### Installation

1. **Install vcpkg** and set the `VCPKG_ROOT` environment variable:

   ```bash
   git clone https://github.com/Microsoft/vcpkg.git
   cd vcpkg
   ./bootstrap-vcpkg.sh  # Linux/macOS
   # or
   .\bootstrap-vcpkg.bat  # Windows
   ```

2. **Configure the project**:

   ```pwsh
   # Navigate to the C++ directory
   cd GeoSharPlusCPP

   # Install required packages
   vcpkg install

   # Configure with CMake
   cmake -B build .
   ```

3. **Build the solution**:

   ```pwsh
   # Open in Visual Studio
   start GSP.DEMO.sln

   # Or build with CMake
   cmake --build build --config Release
   ```

After setup, you should see this structure:

```
GeoSharPlus/
├── GeoSharPlusCPP/       # C++ core library
├── GeoSharPlusNET/       # C# wrapper library
├── GSPdemoGH/            # Grasshopper demo project
├── GSPdemoConsole/       # Console demo project
├── cppPrebuild/          # Compiled C++ libraries (.dll/.dylib)
└── generated/            # Auto-generated FlatBuffer code
    └── GSP_FB/
        ├── cpp/          # C++ FlatBuffer headers
        └── csharp/       # C# FlatBuffer classes
```

## Library Demo

Open the `GSP.DEMO.sln` solution file in Visual Studio and build the project to see working examples.

## Developer Guide

### Adding New Geometry Types

To extend the library with new geometric types, follow these steps:

#### 1. Define FlatBuffer Schema

Create a new `.fbs` file in `GeoSharPlusCPP/schema/`:

```flatbuffers
// Example: circle.fbs
include "base.fbs";

namespace GSP.FB;

table CircleData {
  center: Vec3;
  radius: double;
  normal: Vec3;
}
root_type CircleData;
```

#### 2. Add C++ Serialization

Add functions to `GeoSharPlusCPP/src/Serialization/GeoSerializer.cpp`:

```cpp
bool serializeCircle(const Circle& circle, uint8_t*& resBuffer, int& resSize) {
  flatbuffers::FlatBufferBuilder builder;

  auto center = GSP::FB::Vec3(circle.center[0], circle.center[1], circle.center[2]);
  auto normal = GSP::FB::Vec3(circle.normal[0], circle.normal[1], circle.normal[2]);

  auto circleOffset = GSP::FB::CreateCircleData(builder, &center, circle.radius, &normal);
  builder.Finish(circleOffset);

  resSize = builder.GetSize();
  resBuffer = static_cast<uint8_t*>(CoTaskMemAlloc(resSize));
  std::memcpy(resBuffer, builder.GetBufferPointer(), resSize);
  return true;
}

bool deserializeCircle(const uint8_t* buffer, int size, Circle& circle) {
  auto circleData = GSP::FB::GetCircleData(buffer);
  if (!circleData) return false;

  circle.center = Vector3d(circleData->center()->x(),
                          circleData->center()->y(),
                          circleData->center()->z());
  circle.radius = circleData->radius();
  circle.normal = Vector3d(circleData->normal()->x(),
                          circleData->normal()->y(),
                          circleData->normal()->z());
  return true;
}
```

#### 3. Add C++ API Functions

Add to `GeoSharPlusCPP/src/API/BridgeAPI.cpp`:

```cpp
extern "C" {
GSP_API bool GSP_CALL circle_roundtrip(const uint8_t* inBuffer,
                                       int inSize,
                                       uint8_t** outBuffer,
                                       int* outSize) {
  Circle circle;
  if (!GS::deserializeCircle(inBuffer, inSize, circle)) {
    return false;
  }

  // Process circle data here...

  return GS::serializeCircle(circle, *outBuffer, *outSize);
}
}
```

#### 4. Add C# Native Bridge

Add to `GeoSharPlusNET/NativeBridge.cs`:

```csharp
[DllImport(WinLibName, EntryPoint = "circle_roundtrip", CallingConvention = CallingConvention.Cdecl)]
private static extern bool CircleRoundTripWin(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

[DllImport(MacLibName, EntryPoint = "circle_roundtrip", CallingConvention = CallingConvention.Cdecl)]
private static extern bool CircleRoundTripMac(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

public static bool CircleRoundTrip(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize) {
  if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    return CircleRoundTripWin(inBuffer, inSize, out outBuffer, out outSize);
  else
    return CircleRoundTripMac(inBuffer, inSize, out outBuffer, out outSize);
}
```

#### 5. Add C# Wrapper Functions

Add to `GeoSharPlusNET/Wrapper.cs`:

```csharp
public static byte[] ToCircleBuffer(Circle circle) {
  var builder = new FlatBufferBuilder(256);

  var centerOffset = FB.Vec3.CreateVec3(builder, circle.Center.X, circle.Center.Y, circle.Center.Z);
  var normalOffset = FB.Vec3.CreateVec3(builder, circle.Normal.X, circle.Normal.Y, circle.Normal.Z);

  FB.CircleData.StartCircleData(builder);
  FB.CircleData.AddCenter(builder, centerOffset);
  FB.CircleData.AddRadius(builder, circle.Radius);
  FB.CircleData.AddNormal(builder, normalOffset);
  var circleOffset = FB.CircleData.EndCircleData(builder);

  builder.Finish(circleOffset.Value);
  return builder.SizedByteArray();
}

public static Circle FromCircleBuffer(byte[] buffer) {
  var byteBuffer = new ByteBuffer(buffer);
  var circleData = FB.CircleData.GetRootAsCircleData(byteBuffer);

  var center = new Point3d(circleData.Center.Value.X, circleData.Center.Value.Y, circleData.Center.Value.Z);
  var normal = new Vector3d(circleData.Normal.Value.X, circleData.Normal.Value.Y, circleData.Normal.Value.Z);

  return new Circle(new Plane(center, normal), circleData.Radius);
}
```

#### 6. Build and Test

After adding your new type:

1. **Rebuild the project** to generate FlatBuffer code:

   ```bash
   cmake --build build --config Release
   ```

2. **Test your implementation** by creating a simple round-trip test similar to the examples above.

### Supported Types

Currently supported geometric types:

- `Point3d` / `Vector3d` - Single 3D points/vectors
- `Point3d[]` / `Vector3d[]` - Arrays of 3D points/vectors
- `Mesh` - Triangle meshes with vertices and faces
- `double[]` - Arrays of double values
- `int[]` - Arrays of integer values
- `Pair<int,int>[]` - Arrays of integer pairs
- `Pair<double,double>[]` - Arrays of double pairs

### Architecture Notes

- **FlatBuffers schemas** define the data structure contracts
- **C++ serialization** handles conversion between native types and FlatBuffer format
- **C API functions** provide the bridge between managed and unmanaged code
- **C# wrappers** provide type-safe, idiomatic .NET interfaces
- **Cross-platform support** is handled through platform-specific DLL imports

## Integration into Another Project

**The most important folders are `GeoSharPlusCPP` and `GeoSharPlusNET`.**

### Required Steps:

1. Copy both folders into your project
2. Modify `CMakeLists.txt` in `GeoSharPlusCPP` to add:
   - Additional C++ libraries you need
   - Custom pre-compilation processes
   - Your specific geometric operations
3. Follow the [Setup](#setup) process

### For CAD Plugin Development:

**Rhino/Grasshopper Plugins:**

1. **Add project reference**: Right-click your main project → `Add` → `Project Reference...` → Select `GeoSharPlusNET`
2. **Copy build scripts**: Check `prebuild.ps1` and `postbuild.ps1` in `GSPdemoGH/` for build events
3. **Deploy native libraries**: Ensure the compiled C++ DLLs are copied to your output directory

**Other CAD Platforms:**
We welcome contributions for integration with other CAD software! The core library is platform-agnostic and can be adapted for:

- AutoCAD (.NET API)
- SolidWorks (SOLIDWORKS API)
- Fusion 360 (Fusion 360 API)
- FreeCAD (Python/C++ API)
- Other CAD platforms with .NET or C++ APIs

_Interested in adding support for your CAD platform? Please open an issue or submit a pull request!_

## Performance Characteristics

- **Zero-copy deserialization**: FlatBuffers allows direct access to serialized data without parsing
- **Compact representation**: Efficient binary format reduces memory usage and transfer time
- **Cross-platform compatibility**: Same binary format works across Windows, macOS, and Linux
- **Type safety**: Compile-time verification of data structure compatibility

## Best Practices

### Memory Management

```csharp
// Always free unmanaged memory after use
NativeBridge.SomeFunction(buffer, size, out IntPtr outPtr, out int outSize);
try {
    byte[] result = new byte[outSize];
    Marshal.Copy(outPtr, result, 0, outSize);
    // Use result...
} finally {
    Marshal.FreeCoTaskMem(outPtr); // Critical: prevent memory leaks
}
```

### Error Handling

```csharp
// Check return values from native calls
if (!NativeBridge.Point3dRoundTrip(buffer, size, out IntPtr outPtr, out int outSize)) {
    throw new InvalidOperationException("Native operation failed");
}
```

### Large Data Sets

```csharp
// For large point arrays, consider processing in chunks
const int ChunkSize = 10000;
var chunks = points.Chunk(ChunkSize);
foreach (var chunk in chunks) {
    byte[] buffer = Wrapper.ToPointArrayBuffer(chunk.ToArray());
    // Process chunk...
}
```

## Troubleshooting

### Common Issues

**"DLL not found" errors:**

- Ensure the native DLL is in the same directory as your executable
- Check that you're using the correct architecture (x64 vs x86)
- Verify that Visual C++ Redistributables are installed

**Memory access violations:**

- Always call `Marshal.FreeCoTaskMem()` after using returned pointers
- Don't access returned pointers after freeing them
- Check buffer sizes match between C# and C++

**Build failures:**

- Verify `VCPKG_ROOT` environment variable is set correctly
- Ensure all vcpkg packages are installed (`vcpkg install`)
- Check that CMake can find all dependencies

## Contributing

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Add tests** for new geometric types or functionality
4. **Commit** your changes (`git commit -m 'Add amazing feature'`)
5. **Push** to the branch (`git push origin feature/amazing-feature`)
6. **Open** a Pull Request

### Development Setup

```bash
# Clone and setup
git clone https://github.com/xarthurx/GeoSharPlus.git
cd GeoSharPlus/GeoSharPlusCPP
vcpkg install
cmake -B build .
cmake --build build --config Debug
```

## Used By

This library is trusted by these notable projects:

- [BeingAliveLanguage](https://beingalivelanguage.arch.ethz.ch) - A Grasshopper plugin to create computational diagrams for living systems
- [IG-Mesh](https://github.com/xarthurx/IG-Mesh) - An one-stop solution for mesh processing in Grasshopper (for Rhino)

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
