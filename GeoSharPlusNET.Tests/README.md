# GeoSharPlusNET Tests

Unit tests for the GeoSharPlus .NET framework.

## Test Structure

```
GeoSharPlusNET.Tests/
??? Core/
?   ??? SerializerTests.cs      # FlatBuffer serialization tests
?   ??? PlatformTests.cs        # Platform detection tests
?   ??? MarshalHelperTests.cs   # Memory marshaling tests
??? Geometry/
?   ??? Vec2Tests.cs            # 2D vector tests
?   ??? Vec3Tests.cs            # 3D vector tests
?   ??? MeshTests.cs            # Mesh structure tests
??? GeoSharPlusNET.Tests.csproj
```

## Running Tests

### From Command Line

```bash
# Run all tests
dotnet test GeoSharPlusNET.Tests/GeoSharPlusNET.Tests.csproj

# Run with verbose output
dotnet test GeoSharPlusNET.Tests/GeoSharPlusNET.Tests.csproj --verbosity normal

# Run with coverage
dotnet test GeoSharPlusNET.Tests/GeoSharPlusNET.Tests.csproj --collect:"XPlat Code Coverage"
```

### From Visual Studio

1. Open the solution in Visual Studio
2. Go to **Test** > **Test Explorer**
3. Click **Run All**

## Test Categories

### Geometry Tests

- **Vec2Tests**: Tests for 2D vector operations (addition, subtraction, dot product, etc.)
- **Vec3Tests**: Tests for 3D vector operations (cross product, normalization, etc.)
- **MeshTests**: Tests for mesh creation, triangulation, bounding box, etc.

### Core Tests

- **SerializerTests**: Tests FlatBuffer serialization round-trips for all supported types:
  - Vec3 (single and array)
  - Mesh (triangles and quads)
  - Primitive arrays (int[], double[])
  - Pair arrays ((int,int)[], (double,double)[])
  - Nested arrays (List<List<int>>)

- **PlatformTests**: Tests platform detection (Windows, macOS, Linux)

- **MarshalHelperTests**: Tests memory marshaling utilities

## CI/CD Integration

These tests run automatically via GitHub Actions on:
- Every push to `main`, `master`, `develop`, or `template-cleanup` branches
- Every pull request to these branches

### CI Jobs

| Job | Platforms | Description |
|-----|-----------|-------------|
| `test-dotnet-core` | Ubuntu, Windows, macOS | Runs all unit tests |
| `build-dotnet-full` | Windows | Builds the full .NET solution with RhinoCommon |
| `build-cpp-windows` | Windows | Builds C++ native library |
| `build-cpp-macos` | macOS | Builds C++ native library |
| `coverage` | Ubuntu | Generates test coverage report |
| `generate-flatbuffers` | Ubuntu | Regenerates FlatBuffer files (manual trigger) |

### Requirements for CI

The CI workflow requires that the generated FlatBuffer files are committed to the repository:

```
generated/
??? GSP_FB/
    ??? cpp/           # C++ generated files
    ?   ??? *.h
    ??? csharp/        # C# generated files
        ??? *_generated.cs
```

If you modify the FlatBuffer schemas (`GeoSharPlusCPP/schema/*.fbs`), you must:
1. Regenerate the files locally using CMake or `flatc`
2. Commit the updated generated files

Or trigger the `generate-flatbuffers` workflow manually and download the artifacts.

## Adding New Tests

1. Create a new test class in the appropriate folder (Core/ or Geometry/)
2. Use xUnit attributes:
   - `[Fact]` for simple tests
   - `[Theory]` with `[InlineData]` for parameterized tests
3. Follow the naming convention: `MethodName_Scenario_ExpectedBehavior`

Example:

```csharp
using Xunit;
using GSP.Geometry;

public class MyNewTests {
    [Fact]
    public void MyMethod_WithValidInput_ReturnsExpectedResult() {
        // Arrange
        var input = new Vec3(1, 2, 3);
        
        // Act
        var result = MyMethod(input);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(4, 5, 6)]
    public void MyMethod_WithVariousInputs_Works(double x, double y, double z) {
        var v = new Vec3(x, y, z);
        // ... test logic
    }
}
```

## Note on RhinoCommon

The test project does **not** depend on RhinoCommon. It compiles the Core and Geometry files directly, which allows tests to run in CI environments without Rhino installed.

The Rhino adapter (`GSP.Adapters.Rhino`) is not tested in the automated tests because it requires RhinoCommon. These should be tested manually or in a Rhino environment.

## Note on C++ Native Library

The unit tests in this project test only the .NET code (serialization, geometry types, etc.). They do **not** require the C++ native library (`GeoSharPlusCPP.dll`).

The C++ library is built separately in CI and produces artifacts that can be used for integration testing or deployment.
