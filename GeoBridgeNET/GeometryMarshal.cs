using System;
using System.Runtime.InteropServices;
using Rhino.Geometry;
using FlatBuffers;
using GeomBridge.FB; // Generated FlatBuffers namespace

namespace GeoBridge.NET
{
    public static class GeometryMarshal
    {
        // Convert Rhino mesh to native buffer
        public static IntPtr ToNativeBuffer(Mesh mesh)
        {
            var builder = new FlatBufferBuilder(1024);

            // Serialize vertices
            var verticesOffset = MeshData.CreateVerticesVectorBlock(
                builder, 
                mesh.Vertices.ToFloatArray(), 
                mesh.Vertices.Count);

            // Serialize faces (triangles only)
            var faces = new int[mesh.Faces.Count * 3];
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                var face = mesh.Faces[i];
                faces[i * 3] = face.A;
                faces[i * 3 + 1] = face.B;
                faces[i * 3 + 2] = face.C;
            }
            var facesOffset = MeshData.CreateFacesVectorBlock(
                builder, 
                faces);

            // Build final mesh
            var meshOffset = MeshData.CreateMeshData(
                builder, 
                verticesOffset, 
                facesOffset);

            builder.Finish(meshOffset.Value);
            
            // Copy to unmanaged memory
            var byteArray = builder.SizedByteArray();
            IntPtr bufferPtr = Marshal.AllocCoTaskMem(byteArray.Length);
            Marshal.Copy(byteArray, 0, bufferPtr, byteArray.Length);
            
            return bufferPtr;
        }

        // Convert native buffer to Rhino mesh
        public static Mesh FromNativeBuffer(IntPtr bufferPtr)
        {
            // Get buffer size and copy
            var bufferSize = (int)NativeBridge.GetBufferSize(bufferPtr);
            var byteArray = new byte[bufferSize];
            Marshal.Copy(bufferPtr, byteArray, 0, bufferSize);
            NativeBridge.FreeBuffer(bufferPtr);

            // Parse FlatBuffers data
            var byteBuffer = new ByteBuffer(byteArray);
            var meshData = MeshData.GetRootAsMeshData(byteBuffer);

            // Rebuild Rhino mesh
            var mesh = new Mesh();
            
            // Add vertices
            for (int i = 0; i < meshData.VerticesLength; i++)
            {
                var vec = meshData.Vertices(i);
                mesh.Vertices.Add(
                    (float)vec.Value.X, 
                    (float)vec.Value.Y, 
                    (float)vec.Value.Z);
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

        // Process geometry example
        public static Mesh ProcessMesh(Mesh input)
        {
            var inputBuffer = ToNativeBuffer(input);
            var outputBuffer = NativeBridge.ProcessGeometry(
                inputBuffer, 
                (UIntPtr)Marshal.SizeOf(typeof(byte)) * (UIntPtr)inputBuffer);
            
            return FromNativeBuffer(outputBuffer);
        }
    }
}
