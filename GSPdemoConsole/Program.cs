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

    #endregion
  }
}
