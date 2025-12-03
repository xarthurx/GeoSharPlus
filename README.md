# GeoSharPlus

A C++/C# bridge library template for geometry processing, designed for use with Rhino/Grasshopper plugins.

## 🚀 Quick Start (Using This Template)

### 1. Create Your Project from This Template

Click **"Use this template"** on GitHub to create your own repository.

### 2. Clone and Build

```bash
git clone https://github.com/YOUR_USERNAME/YOUR_PROJECT.git
cd YOUR_PROJECT

# Build C++ library (requires vcpkg)
cd GeoSharPlusCPP
cmake --preset x64-release
cmake --build build --config Release

# Build C# library
cd ../GeoSharPlusNET
dotnet build
```

### 3. Add Your Custom Functions

See the [Extension Guide](#-adding-your-own-functions) below.

---

## 📁 Project Structure

```
GeoSharPlus/
├── GeoSharPlusCPP/              # C++ Native Library
│   ├── include/GeoSharPlusCPP/
│   │   ├── Core/                # ⚠️ UPSTREAM - Core types & macros
│   │   ├── Serialization/       # ⚠️ UPSTREAM - FlatBuffers serialization
│   │   └── Extensions/          # ✅ YOUR CODE HERE
│   ├── src/
│   │   ├── Core/                # ⚠️ UPSTREAM
│   │   ├── Serialization/       # ⚠️ UPSTREAM
│   │   └── Extensions/          # ✅ YOUR CODE HERE
│   └── schema/
│       ├── *.fbs                # ⚠️ UPSTREAM - Core schemas
│       └── extensions/          # ✅ YOUR SCHEMAS HERE
│
├── GeoSharPlusNET/              # C# Wrapper Library
│   ├── NativeBridge.cs          # ⚠️ UPSTREAM - Platform initialization
│   ├── Wrapper.cs               # ⚠️ UPSTREAM - FlatBuffers serialization
│   └── Extensions/              # ✅ YOUR CODE HERE
│       └── ExampleExtensions.cs # Example: Point/Mesh roundtrip
│
├── GSPdemoGH/                   # Example Grasshopper Plugin
├── GSPdemoConsole/              # Example Console App
├── scripts/
│   └── sync-upstream.ps1        # Update from template
└── UPSTREAM_FILES.md            # List of upstream files
```

---

## ✨ Adding Your Own Functions

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

### Step 3: Add C# P/Invoke

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
  private static extern bool MyComputeSomethingWin(
      byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

  [DllImport(MacLibName, EntryPoint = "my_compute_something",
             CallingConvention = CallingConvention.Cdecl)]
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

### Step 4: Rebuild

```bash
# Rebuild C++
cd GeoSharPlusCPP
cmake --build build --config Release

# Rebuild C#
cd ../GeoSharPlusNET
dotnet build
```

---

## 🔄 Updating from Upstream Template

When the GeoSharPlus template gets updates, you can sync them:

```powershell
# First time: add upstream remote
git remote add upstream https://github.com/xarthurx/GeoSharPlus.git

# Sync updates
.\scripts\sync-upstream.ps1
```

This will update all **upstream files** while preserving your **Extensions** folders.

---

## 📋 Prerequisites

- **CMake** 3.31+
- **vcpkg** with `VCPKG_ROOT` environment variable set
- **Visual Studio 2022** (Windows) or Clang (macOS)
- **.NET 8.0 SDK**
- **Rhino 8** (for Grasshopper development)

### vcpkg Dependencies

Automatically installed via `vcpkg.json`:

- Eigen3
- libigl
- FlatBuffers

---

## 📄 License

MIT License - See [LICENSE](LICENSE) for details.
