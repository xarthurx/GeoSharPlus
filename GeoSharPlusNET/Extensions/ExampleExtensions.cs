using System;
using System.Runtime.InteropServices;
using Rhino.Geometry;

namespace GSP.Extensions {

  // ============================================
  // GeoSharPlus Example Extensions
  // ============================================
  // This file contains example extension functions demonstrating:
  // - P/Invoke declarations for C++ functions
  // - High-level wrapper methods for Rhino types
  //
  // These are EXAMPLE functions that you can modify or replace.
  // Use them as templates for your own extensions.
  // ============================================

  /// <summary>
  /// P/Invoke bridge for example extension functions.
  /// Handles platform-specific DLL loading (Windows/macOS).
  /// </summary>
  public static class ExampleBridge {
    private const string WinLibName = @"GeoSharPlusCPP.dll";
    private const string MacLibName = @"libGeoSharPlusCPP.dylib";

    // --------------------------------
    // Point3d Roundtrip
    // --------------------------------
    [DllImport(WinLibName, EntryPoint = "example_point3d_roundtrip", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ExamplePoint3dRoundTripWin(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

    [DllImport(MacLibName, EntryPoint = "example_point3d_roundtrip", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ExamplePoint3dRoundTripMac(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

    public static bool Point3dRoundTrip(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize) {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return ExamplePoint3dRoundTripWin(inBuffer, inSize, out outBuffer, out outSize);
      else
        return ExamplePoint3dRoundTripMac(inBuffer, inSize, out outBuffer, out outSize);
    }

    // --------------------------------
    // Point3d Array Roundtrip
    // --------------------------------
    [DllImport(WinLibName, EntryPoint = "example_point3d_array_roundtrip", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ExamplePoint3dArrayRoundTripWin(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

    [DllImport(MacLibName, EntryPoint = "example_point3d_array_roundtrip", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ExamplePoint3dArrayRoundTripMac(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

    public static bool Point3dArrayRoundTrip(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize) {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return ExamplePoint3dArrayRoundTripWin(inBuffer, inSize, out outBuffer, out outSize);
      else
        return ExamplePoint3dArrayRoundTripMac(inBuffer, inSize, out outBuffer, out outSize);
    }

    // --------------------------------
    // Mesh Roundtrip
    // --------------------------------
    [DllImport(WinLibName, EntryPoint = "example_mesh_roundtrip", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ExampleMeshRoundTripWin(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

    [DllImport(MacLibName, EntryPoint = "example_mesh_roundtrip", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ExampleMeshRoundTripMac(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize);

    public static bool MeshRoundTrip(byte[] inBuffer, int inSize, out IntPtr outBuffer, out int outSize) {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return ExampleMeshRoundTripWin(inBuffer, inSize, out outBuffer, out outSize);
      else
        return ExampleMeshRoundTripMac(inBuffer, inSize, out outBuffer, out outSize);
    }
  }

  /// <summary>
  /// High-level utilities for working with Rhino geometry.
  /// Wraps the low-level P/Invoke calls with Rhino types.
  /// </summary>
  public static class ExampleUtils {

    /// <summary>
    /// Send a Point3d to C++ and back (roundtrip example).
    /// </summary>
    public static Point3d? RoundTrip(Point3d point) {
      var buffer = Wrapper.ToPointBuffer(point);
      if (!ExampleBridge.Point3dRoundTrip(buffer, buffer.Length, out IntPtr outPtr, out int outSize))
        return null;

      var result = new byte[outSize];
      Marshal.Copy(outPtr, result, 0, outSize);
      Marshal.FreeCoTaskMem(outPtr);

      return Wrapper.FromPointBuffer(result);
    }

    /// <summary>
    /// Send a Point3d array to C++ and back (roundtrip example).
    /// </summary>
    public static Point3d[]? RoundTrip(Point3d[] points) {
      var buffer = Wrapper.ToPointArrayBuffer(points);
      if (!ExampleBridge.Point3dArrayRoundTrip(buffer, buffer.Length, out IntPtr outPtr, out int outSize))
        return null;

      var result = new byte[outSize];
      Marshal.Copy(outPtr, result, 0, outSize);
      Marshal.FreeCoTaskMem(outPtr);

      return Wrapper.FromPointArrayBuffer(result);
    }

    /// <summary>
    /// Send a Mesh to C++ and back (roundtrip example).
    /// </summary>
    public static Mesh? RoundTrip(Mesh mesh) {
      var buffer = Wrapper.ToMeshBuffer(mesh);
      if (!ExampleBridge.MeshRoundTrip(buffer, buffer.Length, out IntPtr outPtr, out int outSize))
        return null;

      var result = new byte[outSize];
      Marshal.Copy(outPtr, result, 0, outSize);
      Marshal.FreeCoTaskMem(outPtr);

      return Wrapper.FromMeshBuffer(result);
    }
  }
}
