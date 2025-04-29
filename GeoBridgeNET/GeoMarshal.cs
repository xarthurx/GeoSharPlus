using System;
using System.Runtime.InteropServices;
using Google.FlatBuffers;
using Rhino.Geometry;
using FB = GeoBridgeFB;

namespace GeoBridgeNET
{
  public static class GeoMarshal
  {
    // Rhino Point3D - Simplest Example
    public static IntPtr ToNativePoint3d(Point3d pt)
    {
      var builder = new FlatBufferBuilder(24); // Enough for a single Vector3d

      // Create a Vector3d in the FlatBuffer
      FB.Point3dData.StartPoint3dData(builder);
      var vecOffset = FB.Vec3.CreateVec3(builder, pt.X, pt.Y, pt.Z);
      FB.Point3dData.AddVec(builder, vecOffset);
      var offset = FB.Point3dData.EndPoint3dData(builder);

      // Finish the buffer with the root table offset
      builder.Finish(offset.Value);

      // Now get the completed buffer
      var buf = builder.DataBuffer;
      var vec3d = FB.Point3dData.GetRootAsPoint3dData(buf);

      // Get the bytes and send to native code
      var bytes = builder.SizedByteArray();
      return NativeBridge.CreatePoint3dBuffer(bytes, (UIntPtr)bytes.Length);
    }

    public static Point3d FromNativePoint3d(IntPtr bufferPtr)
    {
      // Get buffer size and copy
      var bufferSize = (int)NativeBridge.GetBufferSize(bufferPtr);
      var byteArray = new byte[bufferSize];
      Marshal.Copy(bufferPtr, byteArray, 0, bufferSize);
      NativeBridge.FreeBuffer(bufferPtr);

      // Parse FlatBuffers data
      var byteBuffer = new ByteBuffer(byteArray);
      var vec = FB.Point3dData.GetRootAsPoint3dData(byteBuffer).Vec;

      // Create Rhino Point3d from the Vector3d
      return vec == null ? new Point3d(0, 0, 0) : new Point3d(vec.Value.X, vec.Value.Y, vec.Value.Z);
    }


    // Rhino Polyline IO
    public static IntPtr ToNativePolyline(Polyline polyline)
    {
      var builder = new FlatBufferBuilder(1024);

      // Serialize vertices
      var verticesOffset = default(VectorOffset);
      if (polyline.Count > 0)
      {
        // Start the vertices vector - specify size of struct (24 bytes) and count
        FB.PolylineData.StartVerticesVector(builder, polyline.Count);

        // Add vertices in reverse order (FlatBuffers builds vectors backward)
        for (int i = polyline.Count - 1; i >= 0; i--)
        {
          var v = polyline[i];
          // For structs in vectors, we add them directly to the buffer
          builder.AddDouble(v.Z); // Values are written in reverse order
          builder.AddDouble(v.Y);
          builder.AddDouble(v.X);
        }

        // End the vector and get its offset
        verticesOffset = builder.EndVector();
      }

      // Build final polyline
      var polylineOffset = FB.PolylineData.CreatePolylineData(
          builder,
          verticesOffset);

      // Finish the buffer
      builder.Finish(polylineOffset.Value);

      // Get the bytes and send to native code
      var bytes = builder.SizedByteArray();
      return NativeBridge.CreatePolylineBuffer(bytes, (UIntPtr)bytes.Length);
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
      for (int i = 0; i < polylineData.VerticesLength; i++)
      {
        var vec = polylineData.Vertices(i);
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
        mesh.Vertices.Add(
            x: (float)(vec?.X ?? 0),
            y: (float)(vec?.Y ?? 0),
            z: (float)(vec?.Z ?? 0));
      }

      // Add faces
      for (int i = 0; i < meshData.FacesLength; i += 3)
      {
        var face = meshData.Faces(i);
        mesh.Faces.AddFace(
          face?.X ?? 0,
          face?.Y ?? 0,
          face?.Z ?? 0);
      }

      return mesh;
    }

  }
}
