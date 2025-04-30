//using System;
using System.Runtime.InteropServices;

namespace GeoBridgeNET
{
  public static class NativeBridge
  {
    private const string WindowsLibName = @"GeoSharPlusCPP.dll";
    private const string MacOSLibName = @"GeoSharPlusCPP.dylib";

    // For each function, we create 3 functions: Windows, macOS implementations, and the public API

    // CreatePoint3dBuffer
    [DllImport(WindowsLibName, EntryPoint = "create_point3d_buffer", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreatePoint3dBufferWindows(byte[] buffer, UIntPtr size);

    [DllImport(MacOSLibName, EntryPoint = "create_point3d_buffer", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreatePoint3dBufferMacOS(byte[] buffer, UIntPtr size);

    public static IntPtr CreatePoint3dBuffer(byte[] buffer, UIntPtr size)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return CreatePoint3dBufferWindows(buffer, size);
      else
        return CreatePoint3dBufferMacOS(buffer, size);
    }

    // CreatePolylineBuffer
    [DllImport(WindowsLibName, EntryPoint = "create_polyline_buffer", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreatePolylineBufferWindows(
        [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
        UIntPtr size);

    [DllImport(MacOSLibName, EntryPoint = "create_polyline_buffer", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreatePolylineBufferMacOS(
        [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
        UIntPtr size);

    public static IntPtr CreatePolylineBuffer(byte[] buffer, UIntPtr size)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return CreatePolylineBufferWindows(buffer, size);
      else
        return CreatePolylineBufferMacOS(buffer, size);
    }

    // CreateMeshBuffer
    [DllImport(WindowsLibName, EntryPoint = "create_mesh_buffer", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreateMeshBufferWindows(
        [MarshalAs(UnmanagedType.LPArray)] double[] vertices,
        UIntPtr vertexCount,
        [MarshalAs(UnmanagedType.LPArray)] int[] faces,
        UIntPtr faceCount);

    [DllImport(MacOSLibName, EntryPoint = "create_mesh_buffer", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreateMeshBufferMacOS(
        [MarshalAs(UnmanagedType.LPArray)] double[] vertices,
        UIntPtr vertexCount,
        [MarshalAs(UnmanagedType.LPArray)] int[] faces,
        UIntPtr faceCount);

    public static IntPtr CreateMeshBuffer(double[] vertices, UIntPtr vertexCount, int[] faces, UIntPtr faceCount)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return CreateMeshBufferWindows(vertices, vertexCount, faces, faceCount);
      else
        return CreateMeshBufferMacOS(vertices, vertexCount, faces, faceCount);
    }

    // GetBufferSize
    [DllImport(WindowsLibName, EntryPoint = "get_buffer_size", CallingConvention = CallingConvention.Cdecl)]
    private static extern UIntPtr GetBufferSizeWindows(IntPtr buffer);

    [DllImport(MacOSLibName, EntryPoint = "get_buffer_size", CallingConvention = CallingConvention.Cdecl)]
    private static extern UIntPtr GetBufferSizeMacOS(IntPtr buffer);

    public static UIntPtr GetBufferSize(IntPtr buffer)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return GetBufferSizeWindows(buffer);
      else
        return GetBufferSizeMacOS(buffer);
    }

    // FreeBuffer
    [DllImport(WindowsLibName, EntryPoint = "free_buffer", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FreeBufferWindows(IntPtr buffer);

    [DllImport(MacOSLibName, EntryPoint = "free_buffer", CallingConvention = CallingConvention.Cdecl)]
    private static extern void FreeBufferMacOS(IntPtr buffer);

    public static void FreeBuffer(IntPtr buffer)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        FreeBufferWindows(buffer);
      else
        FreeBufferMacOS(buffer);
    }
  }
}
