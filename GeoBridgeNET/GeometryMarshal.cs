using System;
using System.Runtime.InteropServices;
using Google.FlatBuffers;
using Rhino.Geometry;
using FB = GeoBridgeFB;

namespace GeoBridgeNET
{
  public static class GeometryMarshal
  {
    // Rhino Polyline IO
    public static IntPtr ToNativePolyline(Polyline polyline)
    {
      var builder = new FlatBufferBuilder(1024);
      // Serialize vertices
      var verticesOffset = default(VectorOffset);
      if (polyline.Count > 0)
      {
        FB.MeshData.StartVerticesVector(builder, polyline.Count);
        // Add vertices in reverse order (important!)
        for (int i = polyline.Count - 1; i >= 0; i--)
        {
          var v = polyline[i];
          FB.Vector3d.CreateVector3d(builder, v.X, v.Y, v.Z);
        }
        verticesOffset = builder.EndVector();
      }
      // Build final polyline
      var polylineOffset = FB.PolylineData.CreatePolylineData(
          builder,
          verticesOffset);
      builder.Finish(polylineOffset.Value);
      // Copy to unmanaged memory
      var byteArray = builder.SizedByteArray();
      IntPtr bufferPtr = Marshal.AllocCoTaskMem(byteArray.Length);
      Marshal.Copy(byteArray, 0, bufferPtr, byteArray.Length);
      return bufferPtr;
    }

    public static Polyline FromNativePolyline(IntPtr bufferPtr)
    {
      // Get buffer size and copy
      var bufferSize = (int)NativeBridge.GetBufferSize(bufferPtr);
      var byteArray = new byte[bufferSize];
      Marshal.Copy(bufferPtr, byteArray, 0, bufferSize);
      NativeBridge.FreeBuffer(bufferPtr);

      // Parse FlatBuffers data
      var byteBuffer = new ByteBuffer(byteArray);
      var polylineData = FB.PolylineData.GetRootAsPolylineData(byteBuffer);

      // Create new Rhino polyline
      var polyline = new Polyline();

      // Add vertices
      for (int i = 0; i < polylineData.PointsLength; i++)
      {
        var vec = polylineData.Points(i);
        // Handle potential null value
        polyline.Add(
            x: vec?.X ?? 0,
            y: vec?.Y ?? 0,
            z: vec?.Z ?? 0);
      }

      return polyline;
    }

    // Rhino Mesh IO 
    public static IntPtr ToNativeMesh(Mesh mesh)
    {
      // Convert vertices to array of doubles
      double[] vertices = new double[mesh.Vertices.Count * 3];
      for (int i = 0; i < mesh.Vertices.Count; i++)
      {
        vertices[i * 3] = mesh.Vertices[i].X;
        vertices[i * 3 + 1] = mesh.Vertices[i].Y;
        vertices[i * 3 + 2] = mesh.Vertices[i].Z;
      }

      // Convert faces to array of ints
      int[] faces = new int[mesh.Faces.Count * 3];
      for (int i = 0; i < mesh.Faces.Count; i++)
      {
        faces[i * 3] = mesh.Faces[i].A;
        faces[i * 3 + 1] = mesh.Faces[i].B;
        faces[i * 3 + 2] = mesh.Faces[i].C;
      }

      // Use the native function to create the mesh buffer
      return NativeBridge.CreateMeshBuffer(
          vertices,
          (UIntPtr)mesh.Vertices.Count,
          faces,
          (UIntPtr)(mesh.Faces.Count * 3)
      );
    }

    public static Mesh FromNativeMesh(IntPtr bufferPtr)
    {
      // Get buffer size and copy
      var bufferSize = (int)NativeBridge.GetBufferSize(bufferPtr);
      var byteArray = new byte[bufferSize];
      Marshal.Copy(bufferPtr, byteArray, 0, bufferSize);
      NativeBridge.FreeBuffer(bufferPtr);

      // Parse FlatBuffers data
      var byteBuffer = new ByteBuffer(byteArray);
      var meshData = FB.MeshData.GetRootAsMeshData(byteBuffer);

      // Rebuild Rhino mesh
      var mesh = new Mesh();

      // Add vertices
      for (int i = 0; i < meshData.VerticesLength; i++)
      {
        var vec = meshData.Vertices(i);
        // Updated line to handle potential null value
        mesh.Vertices.Add(
            x: (float)(vec?.X ?? 0),
            y: (float)(vec?.Y ?? 0),
            z: (float)(vec?.Z ?? 0));
      }

      // Add faces
      for (int i = 0; i < meshData.FacesLength; i += 3)
      {
        mesh.Faces.AddFace(
            meshData.Faces(i),
            meshData.Faces(i + 1),
            meshData.Faces(i + 2));
      }

      return mesh;
    }

  }
}
