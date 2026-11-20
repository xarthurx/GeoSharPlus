![GeoSharPlus Logo](.github/logo.png)

<div align="center">
<h1><b>GeoSharPlus</b></h1>
</div>

GeoSharPlus is a C++/C# interoperability library for geometry data exchange. It uses [FlatBuffers](https://flatbuffers.dev/) to move geometry between managed (.NET) and native (C++) code on Windows and macOS. Rhino and Grasshopper were the first target. This repository is a **template**: click "Use this template" and add your own C++ functions.

## 🚀 Quick Start (Using This Template)

### 1. Create Your Project from This Template

Click **"Use this template"** on GitHub to create your own repository.

### 2. Clone and Build

```bash
git clone https://github.com/YOUR_USERNAME/YOUR_PROJECT.git
cd YOUR_PROJECT

# Set VCPKG_ROOT to your vcpkg checkout. The CMake presets read it.
# macOS / Linux shell:
export VCPKG_ROOT=/path/to/vcpkg
# Windows PowerShell:
#   $env:VCPKG_ROOT = "C:\path\to\vcpkg"

# Build the C++ library
cd GeoSharPlusCPP

# macOS
cmake --preset macos-default
cmake --build --preset macos-release

# Windows (use a Visual Studio Developer shell, so that Ninja finds the MSVC compiler)
cmake --preset windows-default
cmake --build --preset windows-release

# Build the C# library
cd ../GeoSharPlusNET
dotnet build
```

The `macos-default` preset uses the `arm64-osx` vcpkg triplet (Apple Silicon). The `windows-default` preset uses `x64-windows`. The first configure step downloads and builds the vcpkg dependencies, so it takes some time.

### 3. Add Your Custom Functions

See the [Extension Guide](#-adding-your-own-functions) below.

### 4. Run the Demos

The demo projects `GSPdemoConsole` and `GSPdemoGH` build and run on Windows and macOS. See [How the Native Library Reaches the Demos](#-how-the-native-library-reaches-the-demos).

---

## 📁 Project Structure

```
GeoSharPlus/
├── GeoSharPlusCPP/              # C++ Native Library
│   ├── CMakeLists.txt           # ⚠️ UPSTREAM - Build configuration
│   ├── CMakePresets.json        # ⚠️ UPSTREAM - Build presets (windows-*, macos-*)
│   ├── vcpkg.json               # ⚠️ UPSTREAM - Pinned dependencies
│   ├── include/GeoSharPlusCPP/
│   │   ├── Core/                # ⚠️ UPSTREAM - Core types & macros (Macro.h, MathTypes.h, Geometry.h)
│   │   ├── Serialization/       # ⚠️ UPSTREAM - FlatBuffers serialization (Serializer.h)
│   │   └── Extensions/          # ✅ YOUR CODE HERE (ExampleExtensions.h)
│   ├── src/
│   │   ├── Core/                # ⚠️ UPSTREAM - Core implementations
│   │   ├── Serialization/       # ⚠️ UPSTREAM - Serialization implementations
│   │   └── Extensions/          # ✅ YOUR CODE HERE (ExampleExtensions.cpp)
│   └── schema/
│       ├── *.fbs                # ⚠️ UPSTREAM - Core FlatBuffer schemas
│       └── extensions/          # ✅ YOUR SCHEMAS HERE
│
├── GeoSharPlusNET/              # C# Wrapper Library
│   ├── Core/                    # ⚠️ UPSTREAM - Core utilities
│   │   ├── MarshalHelper.cs     # Memory marshaling helpers
│   │   ├── Platform.cs          # OS detection, library paths
│   │   └── Serializer.cs        # FlatBuffers serialization for GSP types
│   ├── Geometry/                # ⚠️ UPSTREAM - Platform-independent geometry types
│   │   ├── Vec2.cs              # 2D point/vector
│   │   ├── Vec3.cs              # 3D point/vector
│   │   ├── Mesh.cs              # Mesh with vertices and faces
│   │   └── IGeometryAdapter.cs  # Interface for CAD platform adapters
│   ├── Adapters/                # CAD Platform Adapters
│   │   └── Rhino/               # ⚠️ UPSTREAM - Rhino adapter
│   │       └── RhinoAdapter.cs  # Converts Rhino types ↔ GSP types
│   ├── Extensions/              # ✅ YOUR CODE HERE
│   │   ├── _README.md           # Detailed extension guide
│   │   └── ExampleExtensions.cs # Example: Point/Mesh roundtrip
│   ├── RuntimeInit.cs           # ⚠️ UPSTREAM - Platform initialization
│   └── Wrapper.cs               # ⚠️ UPSTREAM - High-level API for Rhino types
│
├── GeoSharPlusNET.Tests/        # xUnit tests for Core and Geometry (no Rhino needed)
├── generated/                   # ⚠️ UPSTREAM - Generated FlatBuffers code (tracked)
│   └── GSP_FB/
│       ├── cpp/                 # *_generated.h
│       └── csharp/              # *_generated.cs
├── cppPrebuild/                 # Build output: native library (created by CMake, not tracked)
├── GSPdemoGH/                   # Example Grasshopper Plugin
├── GSPdemoConsole/              # Example Console App
├── GSP.DEMO.sln                 # Solution for the demos
├── scripts/
│   └── sync-upstream.ps1        # Update from template
└── UPSTREAM_FILES.md            # List of upstream files
```

---

## 📦 Architecture Overview

The library uses a layered architecture with platform-independent core types:

```
┌─────────────────────────────────────────────────────────────┐
│  Your Application (Grasshopper Plugin, Console App, etc.)  │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│  GSP.Extensions (Your custom C++↔C# bridge functions)      │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│  GSP.Adapters (CAD-specific: Rhino, AutoCAD, etc.)         │
│  Converts CAD types ↔ GSP.Geometry types                   │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│  GSP.Geometry (Platform-independent: Vec3, Mesh, etc.)     │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│  GSP.Core (Serializer, MarshalHelper, Platform detection)  │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│  GeoSharPlusCPP.dll / libGeoSharPlusCPP.dylib (C++ Native) │
└─────────────────────────────────────────────────────────────┘
```

**Key Concepts:**

- **GSP.Geometry**: Platform-independent types (`Vec3`, `Vec2`, `Mesh`) that don't depend on any CAD software
- **GSP.Adapters**: Convert between CAD-specific types (e.g., `Rhino.Geometry.Point3d`) and GSP types
- **GSP.Core**: Serialization and memory management utilities
- **GSP.Extensions**: Your custom C++ functions with C# bindings

---

## ✨ Adding Your Own Functions

> 💡 For a detailed step-by-step guide with more examples, see `GeoSharPlusNET/Extensions/_README.md`

### Step 1: Create C++ Header

Create a new file in `GeoSharPlusCPP/include/GeoSharPlusCPP/Extensions/`:

```cpp
// MyFunctions.h
#pragma once
#include <cstdint>
#include "GeoSharPlusCPP/Core/Macro.h"

extern "C" {

GSP_API bool GSP_CALL my_compute_something(
    const uint8_t* inBuffer,
    int inSize,
    uint8_t** outBuffer,
    int* outSize);

}  // extern "C"
```

### Step 2: Implement in C++

Create a new file in `GeoSharPlusCPP/src/Extensions/`:

```cpp
// MyFunctions.cpp
#include "GeoSharPlusCPP/Extensions/MyFunctions.h"
#include "GeoSharPlusCPP/Core/MathTypes.h"
#include "GeoSharPlusCPP/Serialization/Serializer.h"

namespace GS = GeoSharPlusCPP::Serialization;

extern "C" {

GSP_API bool GSP_CALL my_compute_something(
    const uint8_t* inBuffer,
    int inSize,
    uint8_t** outBuffer,
    int* outSize) {

  // Your implementation using IGL, Eigen, etc.

  return true;
}

}  // extern "C"
```

### Step 3: Add C# P/Invoke Bridge

Create a new file in `GeoSharPlusNET/Extensions/`:

```csharp
// MyFunctionsBridge.cs
using System.Runtime.InteropServices;

namespace GSP.Extensions {

public static class MyFunctionsBridge {
  private const string WinLibName = @"GeoSharPlusCPP.dll";
  private const string MacLibName = @"libGeoSharPlusCPP.dylib";

  [DllImport(WinLibName, EntryPoint = "my_compute_something",
             CallingConvention = CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.I1)]
  private static extern bool MyComputeSomethingWin(
      byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

  [DllImport(MacLibName, EntryPoint = "my_compute_something",
             CallingConvention = CallingConvention.Cdecl)]
  [return: MarshalAs(UnmanagedType.I1)]
  private static extern bool MyComputeSomethingMac(
      byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

  public static bool MyComputeSomething(
      byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize) {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      return MyComputeSomethingWin(inBuffer, inSize, out outBuffer, out outSize);
    else
      return MyComputeSomethingMac(inBuffer, inSize, out outBuffer, out outSize);
  }
}

}
```

> ⚠️ A C++ `bool` has 1 byte. The default .NET marshalling reads a 4-byte Win32 `BOOL`. Add `[return: MarshalAs(UnmanagedType.I1)]` to every `DllImport` that returns `bool`.

### Step 4: Create High-Level Wrapper (Optional)

For cleaner API usage, create a wrapper that handles serialization:

```csharp
// MyFunctionsUtils.cs
using GSP.Core;
using GSP.Geometry;

namespace GSP.Extensions {

public static class MyFunctionsUtils {
  /// <summary>
  /// Process points using your custom C++ algorithm.
  /// Works with platform-independent GSP.Geometry.Vec3 types.
  /// </summary>
  public static Vec3[]? ProcessPoints(Vec3[] points) {
    // Serialize input
    var buffer = Serializer.Serialize(points);

    // Call native function
    if (!MyFunctionsBridge.MyComputeSomething(buffer, buffer.Length,
            out IntPtr outPtr, out int outSize))
      return null;

    // Marshal and deserialize result
    var resultBuffer = MarshalHelper.CopyAndFree(outPtr, outSize);
    return Serializer.DeserializeVec3Array(resultBuffer);
  }

  /// <summary>
  /// Process Rhino points using your custom C++ algorithm.
  /// Convenience overload for Rhino users.
  /// </summary>
  public static Rhino.Geometry.Point3d[]? ProcessPoints(Rhino.Geometry.Point3d[] points) {
    var adapter = GSP.Adapters.Rhino.RhinoAdapter.Instance;
    var gspPoints = adapter.PointsToGSP(points);
    var result = ProcessPoints(gspPoints);
    return result != null ? adapter.PointsFromGSP(result) : null;
  }
}

}
```

### Step 5: Rebuild

```bash
# Rebuild C++ (use windows-release on Windows)
cd GeoSharPlusCPP
cmake --build --preset macos-release

# Rebuild C#
cd ../GeoSharPlusNET
dotnet build
```

If you add a new C++ source file, run the configure preset again (`cmake --preset macos-default` or `cmake --preset windows-default`) before you build.

---

## 🧩 How the Native Library Reaches the Demos

The demo projects do not call PowerShell scripts. MSBuild targets in the `.csproj` files copy the native library to the build output.

1. The CMake post-build step writes the native library into `cppPrebuild/` at the repository root: `GeoSharPlusCPP.dll` on Windows and `libGeoSharPlusCPP.dylib` on macOS.
2. The MSBuild targets in `GSPdemoGH` and `GSPdemoConsole` copy that file from `cppPrebuild/` to the output folder of the demo.
3. The demos run on Windows and macOS.

Build the C++ library first. If `cppPrebuild/` is empty, the demo build has no native library to copy.

If you use the project in your own solution, copy the same MSBuild targets from the demo `.csproj` files.

---

## 🔄 Updating from Upstream Template

When the GeoSharPlus template gets updates, you can sync them:

```powershell
# First time: add upstream remote
git remote add upstream https://github.com/xarthurx/GeoSharPlus.git

# Sync updates
.\scripts\sync-upstream.ps1
```

This updates all **upstream files** that are listed in [UPSTREAM_FILES.md](UPSTREAM_FILES.md). It does not change your **Extensions** folders. The script compares your current `HEAD` with `upstream/main`.

---

## 🔌 Adding Support for Other CAD Platforms

The library is designed to support multiple CAD platforms through the adapter pattern. To add support for a new CAD platform:

### 1. Create a New Adapter

Create a new folder in `GeoSharPlusNET/Adapters/` (e.g., `AutoCAD/`):

```csharp
// GeoSharPlusNET/Adapters/AutoCAD/AutoCADAdapter.cs
using Autodesk.AutoCAD.Geometry;
using GSP.Geometry;

namespace GSP.Adapters.AutoCAD {
  public class AutoCADAdapter : IGeometryAdapter<Point3d, Autodesk.AutoCAD.DatabaseServices.Mesh> {
    public static AutoCADAdapter Instance { get; } = new();

    public Vec3 PointToGSP(Point3d point) => new(point.X, point.Y, point.Z);
    public Point3d PointFromGSP(Vec3 point) => new(point.X, point.Y, point.Z);

    // Implement other interface methods...
  }
}
```

### 2. Use Your Adapter

```csharp
var adapter = AutoCADAdapter.Instance;
var gspPoints = adapter.PointsToGSP(autocadPoints);
var result = MyUtils.ProcessPoints(gspPoints);
var autocadResult = adapter.PointsFromGSP(result);
```

We welcome contributions for adapters to other CAD platforms!

---

## 📋 Prerequisites

- **CMake** 3.21+
- **Ninja** (the CMake presets use it)
- **vcpkg** with the `VCPKG_ROOT` environment variable set
- **Visual Studio 2022** or later with the C++ workload (Windows), or Xcode command line tools / Clang (macOS)
- **.NET 8.0 SDK**
- **Rhino 8** (only for Grasshopper development)

### vcpkg Dependencies

vcpkg installs these packages from `GeoSharPlusCPP/vcpkg.json`. The `builtin-baseline` pins all ports to vcpkg release 2026.10.16:

| Package     | Version |
|-------------|---------|
| Eigen3      | 5.0.1   |
| libigl      | 2.6.0   |
| FlatBuffers | 25.2.10 |

> ⚠️ **FlatBuffers version rule:** the vcpkg `flatbuffers` version and the `Google.FlatBuffers` NuGet version must be equal. The `flatc` compiler writes a version check into the generated C++ and C# code, so a different version breaks the build. An `overrides` entry in `vcpkg.json` holds FlatBuffers at 25.2.10, because 25.2.10 is the newest `Google.FlatBuffers` NuGet version. If you change one version, change the other.

---

## Used By

- [BeingAliveLanguage](https://beingalivelanguage.arch.ethz.ch) - A Grasshopper plugin to create computational diagrams for living systems
- [IG-Mesh](https://github.com/xarthurx/IG-Mesh) - A one-stop solution for mesh processing in Grasshopper (for Rhino)

---

## 📄 License

MIT License - See [LICENSE](LICENSE) for details.
