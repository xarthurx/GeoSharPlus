using System;
using System.Runtime.InteropServices;

namespace GeomBridge.NET
{
    public static class NativeBridge
    {
        // Windows vs MacOS library name handling
        const string LibName = 
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 
            "GeoBridgeRHGH.dll" : "libGeoBridgeRHGH.dylib";

        [DllImport(LibName, EntryPoint = "create_mesh_buffer", 
            CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateMeshBuffer(
            [MarshalAs(UnmanagedType.LPArray)] double[] vertices,
            UIntPtr vertexCount,
            [MarshalAs(UnmanagedType.LPArray)] int[] faces,
            UIntPtr faceCount);

        [DllImport(LibName, EntryPoint = "get_buffer_size", 
            CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr GetBufferSize(IntPtr buffer);

        [DllImport(LibName, EntryPoint = "free_buffer", 
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void FreeBuffer(IntPtr buffer);

        // Generic geometry processing example
        [DllImport(LibName, EntryPoint = "process_geometry", 
            CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ProcessGeometry(
            IntPtr inputBuffer,
            UIntPtr bufferSize);
    }
}
