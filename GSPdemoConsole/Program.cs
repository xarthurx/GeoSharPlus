using System;
using System.Runtime.InteropServices;

namespace GSPdemoConsole {
class Program {
  // P/Invoke declarations for direct C++ calls
  private const string LibName = @"GeoSharPlusCPP.dll";

  [DllImport(LibName,
             EntryPoint = "example_point3d_roundtrip",
             CallingConvention = CallingConvention.Cdecl)]
  private static extern bool
  ExamplePoint3dRoundTrip(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

  [DllImport(LibName,
             EntryPoint = "example_point3d_array_roundtrip",
             CallingConvention = CallingConvention.Cdecl)]
  private static extern bool
  ExamplePoint3dArrayRoundTrip(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

  [DllImport(
      LibName, EntryPoint = "example_mesh_roundtrip", CallingConvention = CallingConvention.Cdecl)]
  private static extern bool
  ExampleMeshRoundTrip(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

  static void Main(string[] args) {
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║         GeoSharPlus Demo - C#/C++ Bridge Examples        ║");
    Console.WriteLine("║       (Standalone mode - no RhinoCommon dependency)      ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
    Console.ResetColor();

    // Test basic interop without RhinoCommon
    TestPoint3dRoundTrip();
    TestPoint3dArrayRoundTrip();

    // Wait for user input before closing
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("\nPress any key to close the program...");
    Console.ResetColor();
    Console.ReadKey();
  }

  /// <summary>
  /// Tests the round-trip of a Point3d using raw FlatBuffers
  /// </summary>
  static void TestPoint3dRoundTrip() {
    PrintHeader("Point3d Round-Trip Test (Raw FlatBuffer)");

    // Create a simple point buffer manually using Google.FlatBuffers
    double x = 1.0, y = 2.0, z = 3.0;
    Console.WriteLine($"Original: ({x:F2}, {y:F2}, {z:F2})");

    // Build the FlatBuffer for a point
    var builder = new Google.FlatBuffers.FlatBufferBuilder(64);
    GSP.FB.PointData.StartPointData(builder);
    var vecOffset = GSP.FB.Vec3.CreateVec3(builder, x, y, z);
    GSP.FB.PointData.AddPoint(builder, vecOffset);
    var ptOffset = GSP.FB.PointData.EndPointData(builder);
    builder.Finish(ptOffset.Value);
    var buffer = builder.SizedByteArray();

    // Call C++ roundtrip
    bool success =
        ExamplePoint3dRoundTrip(buffer, buffer.Length, out IntPtr outPtr, out int outSize);

    if (success && outPtr != IntPtr.Zero) {
      // Read result
      var resultBuffer = new byte[outSize];
      Marshal.Copy(outPtr, resultBuffer, 0, outSize);
      Marshal.FreeCoTaskMem(outPtr);

      // Parse the result
      var byteBuffer = new Google.FlatBuffers.ByteBuffer(resultBuffer);
      var ptData = GSP.FB.PointData.GetRootAsPointData(byteBuffer);
      var pt = ptData.Point;

      if (pt.HasValue) {
        Console.WriteLine($"Result:   ({pt.Value.X:F2}, {pt.Value.Y:F2}, {pt.Value.Z:F2})");
        bool match = Math.Abs(pt.Value.X - x) < 0.001 && Math.Abs(pt.Value.Y - y) < 0.001 &&
                     Math.Abs(pt.Value.Z - z) < 0.001;
        PrintResult("Success", match);
      } else {
        PrintResult("Failed (null point)", false);
      }
    } else {
      PrintResult("Failed (C++ call)", false);
    }

    PrintFooter();
  }

  /// <summary>
  /// Tests the round-trip of a Point3d array using raw FlatBuffers
  /// </summary>
  static void TestPoint3dArrayRoundTrip() {
    PrintHeader("Point3d Array Round-Trip Test (Raw FlatBuffer)");

    // Define test points
    var points =
        new(double X, double Y, double Z)[] { (1.0, 2.0, 3.0), (4.0, 5.0, 6.0), (7.0, 8.0, 9.0) };

    Console.WriteLine($"Original: {points.Length} points");
    foreach (var pt in points)
      Console.WriteLine($"  ({pt.X:F2}, {pt.Y:F2}, {pt.Z:F2})");

    // Build the FlatBuffer for point array
    var builder = new Google.FlatBuffers.FlatBufferBuilder(1024);
    GSP.FB.PointArrayData.StartPointsVector(builder, points.Length);
    for (int i = points.Length - 1; i >= 0; i--) {
      GSP.FB.Vec3.CreateVec3(builder, points[i].X, points[i].Y, points[i].Z);
    }
    var ptOffset = builder.EndVector();
    var arrayOffset = GSP.FB.PointArrayData.CreatePointArrayData(builder, ptOffset);
    builder.Finish(arrayOffset.Value);
    var buffer = builder.SizedByteArray();

    // Call C++ roundtrip
    bool success =
        ExamplePoint3dArrayRoundTrip(buffer, buffer.Length, out IntPtr outPtr, out int outSize);

    if (success && outPtr != IntPtr.Zero) {
      // Read result
      var resultBuffer = new byte[outSize];
      Marshal.Copy(outPtr, resultBuffer, 0, outSize);
      Marshal.FreeCoTaskMem(outPtr);

      // Parse the result
      var byteBuffer = new Google.FlatBuffers.ByteBuffer(resultBuffer);
      var pointArray = GSP.FB.PointArrayData.GetRootAsPointArrayData(byteBuffer);

      Console.WriteLine($"\nResult: {pointArray.PointsLength} points");
      bool allMatch = pointArray.PointsLength == points.Length;

      for (int i = 0; i < pointArray.PointsLength; i++) {
        var pt = pointArray.Points(i);
        if (pt.HasValue) {
          Console.WriteLine($"  ({pt.Value.X:F2}, {pt.Value.Y:F2}, {pt.Value.Z:F2})");
          if (i < points.Length) {
            allMatch &= Math.Abs(pt.Value.X - points[i].X) < 0.001 &&
                        Math.Abs(pt.Value.Y - points[i].Y) < 0.001 &&
                        Math.Abs(pt.Value.Z - points[i].Z) < 0.001;
          }
        }
      }

      PrintResult("Success", allMatch);
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
