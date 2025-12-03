# GeoSharPlus Upstream Files

This document lists which files are maintained **upstream** (in the original GeoSharPlus template)
and should be updated via `scripts/sync-upstream.ps1` rather than modified directly.

## ⚠️ DO NOT MODIFY These Files Directly

When you need upstream updates, use:

```powershell
.\scripts\sync-upstream.ps1
```

---

## C++ Core Files (GeoSharPlusCPP)

### Headers

- `include/GeoSharPlusCPP/Core/Geometry.h` - Core geometry types
- `include/GeoSharPlusCPP/Core/Macro.h` - Platform macros
- `include/GeoSharPlusCPP/Core/MathTypes.h` - Math type definitions
- `include/GeoSharPlusCPP/Serialization/Serializer.h` - FlatBuffers serialization

### Sources

- `src/Core/Geometry.cpp` - Core geometry implementations
- `src/Serialization/Serializer.cpp` - Serialization implementations

### Schemas (Core)

- `schema/base.fbs`
- `schema/doubleArray.fbs`
- `schema/doublePairArray.fbs`
- `schema/intArray.fbs`
- `schema/intNestedArray.fbs`
- `schema/intPairArray.fbs`
- `schema/mesh.fbs`
- `schema/point.fbs`
- `schema/pointArray.fbs`

### Build Configuration

- `CMakeLists.txt` - CMake build configuration
- `CMakePresets.json` - CMake presets
- `vcpkg.json` - vcpkg dependencies

---

## C# Core Files (GeoSharPlusNET)

- `RuntimeInit.cs` - Platform initialization and library loading
- `Wrapper.cs` - FlatBuffers wrapper utilities
- `GeoSharPlusNET.csproj` - Project file

---

## ✅ YOU CAN MODIFY These Folders

Add your custom code to these **Extensions** folders:

### C++ Extensions

- `include/GeoSharPlusCPP/Extensions/` - Your custom header files
- `src/Extensions/` - Your custom source files
- `schema/extensions/` - Your custom FlatBuffer schemas

### C# Extensions

- `GeoSharPlusNET/Extensions/` - Your custom C# files

---

## Updating from Upstream

1. Add the original GeoSharPlus as an upstream remote (one time only):

   ```bash
   git remote add upstream https://github.com/xarthurx/GeoSharPlus.git
   ```

2. Run the sync script:

   ```powershell
   .\scripts\sync-upstream.ps1
   ```

3. Review and commit the changes:
   ```bash
   git diff
   git add -A
   git commit -m "Sync with upstream GeoSharPlus"
   ```
