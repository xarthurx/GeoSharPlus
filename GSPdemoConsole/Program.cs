using System;
using System.Runtime.InteropServices;
using Rhino.Geometry;

namespace GSPdemoConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
      Console.WriteLine("║         GeoSharPlusCPP Interoperability Demo             ║");
      Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
      Console.ResetColor();

      // Testing Functions  
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
    /// Tests the round-trip conversion of a Point3d through the C++ library
    /// </summary>
    static void TestPoint3dRoundTrip()
    {
      PrintHeader("Point3d Round-Trip Test");

      // Define a test point
      var pt1 = new Point3d(1, 2, 3);

      // Display the original point before sending to native code
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine($"Original Point: {FormatPoint(pt1)}");
      Console.ResetColor();

      // ---- C# to C++ ----
      PrintOperation("--> Sending to C++ library...", ConsoleColor.Yellow);

      // Convert the point to a byte buffer for sending to C++
      var bufPtr = GSP.Wrapper.ToPointBuffer(pt1);

      // Send the point through the C++ round-trip function
      GSP.NativeBridge.Point3dRoundTrip(bufPtr, bufPtr.Length, out IntPtr outPtr, out int outSize);

      // ---- C++ to C# ----
      PrintOperation("<-- Receiving from C++ library...", ConsoleColor.Green);

      // Copy the result from unmanaged memory to a managed byte array
      var byteArray = new byte[outSize];
      Marshal.Copy(outPtr, byteArray, 0, (int)outSize);
      Marshal.FreeCoTaskMem(outPtr); // Free the unmanaged memory

      // Convert the byte buffer back to a Point3d
      var res = GSP.Wrapper.FromPointBuffer(byteArray);

      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine($"\nPoint after round-trip: {FormatPoint(res)}");
      Console.ResetColor();

      // Verify result
      bool success = pt1.Equals(res);
      PrintResult("Round-trip successful", success);

      PrintFooter();
    }

    /// <summary>
    /// Tests the round-trip conversion of an array of Point3d through the C++ library
    /// </summary>
    static void TestPoint3dArrayRoundTrip()
    {
      PrintHeader("Point3d Array Round-Trip Test");

      // Define a test array of points
      var points = new Point3d[]
      {
          new Point3d(1, 2, 3),
          new Point3d(4, 5, 6),
          new Point3d(7, 8, 9),
          new Point3d(10, 11, 12)
      };

      // Display the original points before sending to native code
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine($"Original Points Count: {points.Length}");
      Console.ResetColor();

      for (int i = 0; i < points.Length; i++)
      {
        Console.WriteLine($"  Point[{i}]: {FormatPoint(points[i])}");
      }

      // ---- C# to C++ ----
      PrintOperation("--> Sending to C++ library...", ConsoleColor.Yellow);

      // Convert the point array to a byte buffer for sending to C++
      var bufPtr = GSP.Wrapper.ToPointArrayBuffer(points);

      // Send the point array through the C++ round-trip function
      var suc = GSP.NativeBridge.Point3dArrayRoundTrip(bufPtr, bufPtr.Length, out IntPtr outPtr, out int outSize);

      // ---- C++ to C# ----
      PrintOperation("<-- Receiving from C++ library...", ConsoleColor.Green);

      // Copy the result from unmanaged memory to a managed byte array
      var byteArray = new byte[outSize];
      Marshal.Copy(outPtr, byteArray, 0, outSize);
      Marshal.FreeCoTaskMem(outPtr); // Free the unmanaged memory

      // Convert the byte buffer back to a Point3d array
      var resultPoints = GSP.Wrapper.FromPointArrayBuffer(byteArray);

      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine($"\nPoints after round-trip (Count: {resultPoints.Length}):");
      Console.ResetColor();

      for (int i = 0; i < resultPoints.Length; i++)
      {
        Console.WriteLine($"  Point[{i}]: {FormatPoint(resultPoints[i])}");
      }

      // Verify result
      bool success = true;
      if (points.Length != resultPoints.Length)
      {
        success = false;
      }
      else
      {
        for (int i = 0; i < points.Length; i++)
        {
          if (!points[i].Equals(resultPoints[i]))
          {
            success = false;
            break;
          }
        }
      }

      PrintResult("Round-trip successful", success);
      PrintFooter();
    }

    /// <summary>
    /// Tests the round-trip conversion of a Mesh through the C++ library
    /// </summary>
    static void TestMeshRoundTrip()
    {
      PrintHeader("Mesh Round-Trip Test");

      // Create a simple test mesh (a cube)
      var mesh = new Mesh();

      // Add vertices
      mesh.Vertices.Add(0, 0, 0);  // 0
      mesh.Vertices.Add(1, 0, 0);  // 1
      mesh.Vertices.Add(1, 1, 0);  // 2
      mesh.Vertices.Add(0, 1, 0);  // 3
      mesh.Vertices.Add(0, 0, 1);  // 4
      mesh.Vertices.Add(1, 0, 1);  // 5
      mesh.Vertices.Add(1, 1, 1);  // 6
      mesh.Vertices.Add(0, 1, 1);  // 7

      // Add faces (6 quad faces for the cube)
      mesh.Faces.AddFace(0, 1, 2, 3);  // Bottom
      mesh.Faces.AddFace(4, 7, 6, 5);  // Top
      mesh.Faces.AddFace(0, 4, 5, 1);  // Front
      mesh.Faces.AddFace(1, 5, 6, 2);  // Right
      mesh.Faces.AddFace(2, 6, 7, 3);  // Back
      mesh.Faces.AddFace(3, 7, 4, 0);  // Left

      mesh.RebuildNormals();

      // Display some info about the original mesh
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine($"Original Mesh: {FormatMesh(mesh)}");
      Console.ResetColor();

      // ---- C# to C++ ----
      PrintOperation("--> Sending to C++ library...", ConsoleColor.Yellow);

      // Convert the mesh to a byte buffer for sending to C++
      var bufPtr = GSP.Wrapper.ToMeshBuffer(mesh);

      // Send the mesh through the C++ round-trip function
      GSP.NativeBridge.MeshRoundTrip(bufPtr, bufPtr.Length, out IntPtr outPtr, out int outSize);

      // ---- C++ to C# ----
      PrintOperation("<-- Receiving from C++ library...", ConsoleColor.Green);

      // Copy the result from unmanaged memory to a managed byte array
      var byteArray = new byte[outSize];
      Marshal.Copy(outPtr, byteArray, 0, outSize);
      Marshal.FreeCoTaskMem(outPtr); // Free the unmanaged memory

      // Convert the byte buffer back to a Mesh
      var resultMesh = GSP.Wrapper.FromMeshBuffer(byteArray);

      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine($"\nMesh after round-trip: {FormatMesh(resultMesh)}");
      Console.ResetColor();

      // Verify result - compare vertex and face counts
      bool success =
        mesh.Vertices.Count == resultMesh.Vertices.Count &&
        mesh.Faces.Count == resultMesh.Faces.Count;

      PrintResult("Round-trip successful", success);
      PrintFooter();
    }


    #region Output Formatting Helpers

    private static void PrintHeader(string title)
    {
      Console.WriteLine();
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("╔═" + new string('═', title.Length) + "═╗");
      Console.WriteLine("║ " + title + " ║");
      Console.WriteLine("╚═" + new string('═', title.Length) + "═╝");
      Console.ResetColor();
      Console.WriteLine();
    }

    private static void PrintFooter()
    {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("\n" + new string('═', 60));
      Console.ResetColor();
    }

    private static void PrintOperation(string operation, ConsoleColor color)
    {
      Console.ForegroundColor = color;
      Console.WriteLine("\n" + operation);
      Console.ResetColor();
    }

    private static void PrintResult(string message, bool success)
    {
      Console.Write("\n" + message + ": ");
      Console.ForegroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;
      Console.WriteLine(success);
      Console.ResetColor();
    }

    private static string FormatPoint(Point3d point)
    {
      return $"({point.X:F2}, {point.Y:F2}, {point.Z:F2})";
    }
    /// <summary>
    /// Helper to format mesh information
    /// </summary>
    private static string FormatMesh(Mesh mesh)
    {
      return $"Vertices: {mesh.Vertices.Count}, Faces: {mesh.Faces.Count}";
    }

    #endregion
  }
}
