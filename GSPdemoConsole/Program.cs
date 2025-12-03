using System;
using System.Runtime.InteropServices;
using Rhino.Geometry;

namespace GSPdemoConsole {
  class Program {
    static void Main(string[] args) {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
      Console.WriteLine("║         GeoSharPlus Demo - C#/C++ Bridge Examples        ║");
      Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
      Console.ResetColor();

      // All examples use GSP.Extensions (the extension pattern)
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
    /// Tests the round-trip of a Point3d using the extension example
    /// </summary>
    static void TestPoint3dRoundTrip() {
      PrintHeader("Point3d Round-Trip Test");

      var pt = new Point3d(1, 2, 3);
      Console.WriteLine($"Original: {FormatPoint(pt)}");

      // Use the high-level ExampleUtils wrapper
      var result = GSP.Extensions.ExampleUtils.RoundTrip(pt);

      if (result.HasValue) {
        Console.WriteLine($"Result:   {FormatPoint(result.Value)}");
        PrintResult("Success", pt.Equals(result.Value));
      } else {
        PrintResult("Failed", false);
      }

      PrintFooter();
    }

    /// <summary>
    /// Tests the round-trip of a Point3d array
    /// </summary>
    static void TestPoint3dArrayRoundTrip() {
      PrintHeader("Point3d Array Round-Trip Test");

      var points = new Point3d[] {
        new Point3d(1, 2, 3),
        new Point3d(4, 5, 6),
        new Point3d(7, 8, 9)
      };

      Console.WriteLine($"Original: {points.Length} points");
      foreach (var pt in points)
        Console.WriteLine($"  {FormatPoint(pt)}");

      // Use the high-level ExampleUtils wrapper
      var result = GSP.Extensions.ExampleUtils.RoundTrip(points);

      if (result != null) {
        Console.WriteLine($"\nResult: {result.Length} points");
        foreach (var pt in result)
          Console.WriteLine($"  {FormatPoint(pt)}");

        bool success = points.Length == result.Length;
        for (int i = 0; i < points.Length && success; i++)
          success = points[i].Equals(result[i]);

        PrintResult("Success", success);
      } else {
        PrintResult("Failed", false);
      }

      PrintFooter();
    }

    /// <summary>
    /// Tests the round-trip of a Mesh
    /// </summary>
    static void TestMeshRoundTrip() {
      PrintHeader("Mesh Round-Trip Test");

      // Create a simple cube mesh
      var mesh = new Mesh();
      mesh.Vertices.Add(0, 0, 0);
      mesh.Vertices.Add(1, 0, 0);
      mesh.Vertices.Add(1, 1, 0);
      mesh.Vertices.Add(0, 1, 0);
      mesh.Vertices.Add(0, 0, 1);
      mesh.Vertices.Add(1, 0, 1);
      mesh.Vertices.Add(1, 1, 1);
      mesh.Vertices.Add(0, 1, 1);

      mesh.Faces.AddFace(0, 1, 2, 3);  // Bottom
      mesh.Faces.AddFace(4, 7, 6, 5);  // Top
      mesh.Faces.AddFace(0, 4, 5, 1);  // Front
      mesh.Faces.AddFace(1, 5, 6, 2);  // Right
      mesh.Faces.AddFace(2, 6, 7, 3);  // Back
      mesh.Faces.AddFace(3, 7, 4, 0);  // Left

      Console.WriteLine($"Original: {FormatMesh(mesh)}");

      // Use the high-level ExampleUtils wrapper
      var result = GSP.Extensions.ExampleUtils.RoundTrip(mesh);

      if (result != null) {
        Console.WriteLine($"Result:   {FormatMesh(result)}");
        bool success = mesh.Vertices.Count == result.Vertices.Count &&
                       mesh.Faces.Count == result.Faces.Count;
        PrintResult("Success", success);
      } else {
        PrintResult("Failed", false);
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

    static string FormatPoint(Point3d pt) => $"({pt.X:F2}, {pt.Y:F2}, {pt.Z:F2})";
    static string FormatMesh(Mesh m) => $"V:{m.Vertices.Count}, F:{m.Faces.Count}";

    #endregion
  }
}
