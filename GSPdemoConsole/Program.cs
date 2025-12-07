using System;
using System.Runtime.InteropServices;
using GSP.Core;
using GSP.Geometry;

namespace GSPdemoConsole {
class Program {
  // P/Invoke declarations using GSP.Core.Platform constants
  [DllImport(Platform.WindowsLib,
             EntryPoint = "example_point3d_roundtrip",
             CallingConvention = CallingConvention.Cdecl)]
  private static extern bool
  ExamplePoint3dRoundTripWin(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

  [DllImport(Platform.MacLib,
             EntryPoint = "example_point3d_roundtrip",
             CallingConvention = CallingConvention.Cdecl)]
  private static extern bool
  ExamplePoint3dRoundTripMac(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

  [DllImport(Platform.WindowsLib,
             EntryPoint = "example_point3d_array_roundtrip",
             CallingConvention = CallingConvention.Cdecl)]
  private static extern bool ExamplePoint3dArrayRoundTripWin(byte[] inBuffer,
                                                             int inSize,
                                                             out IntPtr outBuffer,
                                                             out int outSize);

  [DllImport(Platform.MacLib,
             EntryPoint = "example_point3d_array_roundtrip",
             CallingConvention = CallingConvention.Cdecl)]
  private static extern bool ExamplePoint3dArrayRoundTripMac(byte[] inBuffer,
                                                             int inSize,
                                                             out IntPtr outBuffer,
                                                             out int outSize);

  [DllImport(Platform.WindowsLib,
             EntryPoint = "example_mesh_roundtrip",
             CallingConvention = CallingConvention.Cdecl)]
  private static extern bool
  ExampleMeshRoundTripWin(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

  [DllImport(Platform.MacLib,
             EntryPoint = "example_mesh_roundtrip",
             CallingConvention = CallingConvention.Cdecl)]
  private static extern bool
  ExampleMeshRoundTripMac(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

  // Cross-platform wrappers
  private static bool
  Point3dRoundTrip(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize) {
    if (Platform.IsWindows)
      return ExamplePoint3dRoundTripWin(inBuffer, inSize, out outBuffer, out outSize);
    else
      return ExamplePoint3dRoundTripMac(inBuffer, inSize, out outBuffer, out outSize);
  }

  private static bool
  Point3dArrayRoundTrip(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize) {
    if (Platform.IsWindows)
      return ExamplePoint3dArrayRoundTripWin(inBuffer, inSize, out outBuffer, out outSize);
    else
      return ExamplePoint3dArrayRoundTripMac(inBuffer, inSize, out outBuffer, out outSize);
  }

  private static bool
  MeshRoundTrip(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize) {
    if (Platform.IsWindows)
      return ExampleMeshRoundTripWin(inBuffer, inSize, out outBuffer, out outSize);
    else
      return ExampleMeshRoundTripMac(inBuffer, inSize, out outBuffer, out outSize);
  }

  static void Main(string[] args) {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║         GeoSharPlus Demo - C#/C++ Bridge Examples        ║");
    Console.WriteLine("║       (Standalone mode - no RhinoCommon dependency)      ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
    Console.ResetColor();

    Console.WriteLine(
        $"\nPlatform: {(Platform.IsWindows ? "Windows" : Platform.IsMac ? "macOS" : "Linux")}");
    Console.WriteLine($"Native Library: {Platform.NativeLibrary}");

    // Test basic interop using GSP.Geometry types
    TestPoint3dRoundTrip();
    TestPoint3dArrayRoundTrip();
    TestMeshRoundTrip();

    // Wait for user input before closing
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("\nPress any key to close the program...");
    Console.ResetColor();
    Console.ReadKey();
  }

  /// <summary>
  /// Tests the round-trip of a Vec3 point using GSP.Core.Serializer
  /// </summary>
  static void TestPoint3dRoundTrip() {
    PrintHeader("Vec3 Round-Trip Test");

    // Create a point using GSP.Geometry.Vec3
    var point = new Vec3(1.0, 2.0, 3.0);
    Console.WriteLine($"Original: {point}");

    // Serialize using GSP.Core.Serializer
    var buffer = Serializer.Serialize(point);

    // Call C++ roundtrip
    bool success = Point3dRoundTrip(buffer, buffer.Length, out IntPtr outPtr, out int outSize);

    if (success && outPtr != IntPtr.Zero) {
      // Marshal result using GSP.Core.MarshalHelper
      var resultBuffer = MarshalHelper.CopyAndFree(outPtr, outSize);

      // Deserialize using GSP.Core.Serializer
      var result = Serializer.DeserializeVec3(resultBuffer);

      Console.WriteLine($"Result:   {result}");
      PrintResult("Success", point.ApproximatelyEquals(result));
    } else {
      PrintResult("Failed (C++ call)", false);
    }

    PrintFooter();
  }

  /// <summary>
  /// Tests the round-trip of a Vec3 array using GSP.Core.Serializer
  /// </summary>
  static void TestPoint3dArrayRoundTrip() {
    PrintHeader("Vec3 Array Round-Trip Test");

    // Create points using GSP.Geometry.Vec3
    var points = new Vec3[] { new(1.0, 2.0, 3.0), new(4.0, 5.0, 6.0), new(7.0, 8.0, 9.0) };

    Console.WriteLine($"Original: {points.Length} points");
    foreach (var pt in points)
      Console.WriteLine($"  {pt}");

    // Serialize using GSP.Core.Serializer
    var buffer = Serializer.Serialize(points);

    // Call C++ roundtrip
    bool success = Point3dArrayRoundTrip(buffer, buffer.Length, out IntPtr outPtr, out int outSize);

    if (success && outPtr != IntPtr.Zero) {
      // Marshal result using GSP.Core.MarshalHelper
      var resultBuffer = MarshalHelper.CopyAndFree(outPtr, outSize);

      // Deserialize using GSP.Core.Serializer
      var result = Serializer.DeserializeVec3Array(resultBuffer);

      Console.WriteLine($"\nResult: {result.Length} points");
      foreach (var pt in result)
        Console.WriteLine($"  {pt}");

      // Verify results
      bool allMatch = points.Length == result.Length;
      for (int i = 0; i < points.Length && allMatch; i++)
        allMatch = points[i].ApproximatelyEquals(result[i]);

      PrintResult("Success", allMatch);
    } else {
      PrintResult("Failed (C++ call)", false);
    }

    PrintFooter();
  }

  /// <summary>
  /// Tests the round-trip of a Mesh using GSP.Core.Serializer
  /// </summary>
  static void TestMeshRoundTrip() {
    PrintHeader("Mesh Round-Trip Test");

    // Create a simple cube mesh using GSP.Geometry.Mesh
    var mesh = new Mesh { Vertices = new Vec3[] { new(0, 0, 0),
                                                        new(1, 0, 0),
                                                        new(1, 1, 0),
                                                        new(0, 1, 0),
                                                        new(0, 0, 1),
                                                        new(1, 0, 1),
                                                        new(1, 1, 1),
                                                        new(0, 1, 1) },
                                QuadFaces = new(int, int, int, int)[] {
                                  (0, 1, 2, 3),  // Bottom
                                  (4, 7, 6, 5),  // Top
                                  (0, 4, 5, 1),  // Front
                                  (1, 5, 6, 2),  // Right
                                  (2, 6, 7, 3),  // Back
                                  (3, 7, 4, 0)   // Left
                                } };

    Console.WriteLine($"Original: {mesh}");

    // Serialize using GSP.Core.Serializer
    var buffer = Serializer.Serialize(mesh);

    // Call C++ roundtrip
    bool success = MeshRoundTrip(buffer, buffer.Length, out IntPtr outPtr, out int outSize);

    if (success && outPtr != IntPtr.Zero) {
      // Marshal result using GSP.Core.MarshalHelper
      var resultBuffer = MarshalHelper.CopyAndFree(outPtr, outSize);

      // Deserialize using GSP.Core.Serializer
      var result = Serializer.DeserializeMesh(resultBuffer);

      Console.WriteLine($"Result:   {result}");

      bool matchVertices = mesh.VertexCount == result.VertexCount;
      bool matchFaces = mesh.FaceCount == result.FaceCount;

      PrintResult("Success", matchVertices && matchFaces);
    } else {
      PrintResult("Failed (C++ call)", false);
    }

    PrintFooter();
  }

#region Output Formatting Helpers

  static void PrintHeader(string title) {
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"═══ {title} ═══");
    Console.ResetColor();
  }

  static void PrintFooter() {
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine(new string('─', 50));
    Console.ResetColor();
  }

  static void PrintResult(string message, bool success) {
    Console.Write($"{message}: ");
    Console.ForegroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine(success ? "✓" : "✗");
    Console.ResetColor();
  }

#endregion
}
}
