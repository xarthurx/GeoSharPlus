using System;
using System.Runtime.InteropServices;
using Rhino.Geometry;

namespace GSPdemoConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine(":::::::::: Example Usage for GeoSharPlusCPP ::::::::::");

      // Test Point3d round-trip
      TestPoint3dRoundTrip();

      // Future tests can be added here
      // TestPointArrayRoundTrip();
      // TestMeshRoundTrip();
    }

    /// <summary>
    /// Tests the round-trip conversion of a Point3d through the C++ library
    /// </summary>
    /// <remarks>
    /// This method demonstrates the process of:
    /// 1. Converting a Rhino Point3d to a byte buffer
    /// 2. Passing the buffer to C++ code
    /// 3. Receiving processed data back from C++
    /// 4. Converting the returned buffer back to a Point3d
    /// 
    /// This validates the marshaling process between C# and C++ for geometry data.
    /// </remarks>
    static void TestPoint3dRoundTrip()
    {
      Console.WriteLine("\n----- Point3d Round-Trip Test -----");

      // Define a test point
      var pt1 = new Point3d(1, 2, 3);

      // Display the original point before sending to native code
      Console.WriteLine($"Original Point: {pt1}");

      // ---- C# to C++ ----
      Console.WriteLine("--> Sending to C++ library...");

      // Convert the point to a byte buffer for sending to C++
      var bufPtr = GSP.Wrapper.ToPointBuffer(pt1);

      // Send the point through the C++ round-trip function
      GSP.NativeBridge.Point3dRoundTrip(bufPtr, bufPtr.Length, out IntPtr outPtr, out int outSize);

      // ---- C++ to C# ----
      Console.WriteLine("<-- Receiving from C++ library...");

      // Copy the result from unmanaged memory to a managed byte array
      var byteArray = new byte[outSize];
      Marshal.Copy(outPtr, byteArray, 0, (int)outSize);
      Marshal.FreeCoTaskMem(outPtr); // Free the unmanaged memory

      // Convert the byte buffer back to a Point3d
      var res = GSP.Wrapper.FromPointBuffer(byteArray);
      Console.WriteLine($"Point after round-trip: {res}");

      // Verify result
      bool success = pt1.Equals(res);
      Console.WriteLine($"Round-trip successful: {success}");
      Console.WriteLine("-------------------------------");
    }
  }
}
