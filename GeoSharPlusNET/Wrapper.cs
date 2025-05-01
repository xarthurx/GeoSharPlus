using Google.FlatBuffers;
using Rhino.Geometry;

namespace GSP
{
  public static class Wrapper
  {
    public static byte[] ToPointBuffer(Point3d pt)
    {
      var builder = new FlatBufferBuilder(64); // Enough for a single Vector3d

      // Create a Vector3d in the FlatBuffer
      FB.PointData.StartPointData(builder);
      var vecOffset = FB.Vec3.CreateVec3(builder, pt.X, pt.Y, pt.Z);
      FB.PointData.AddPoint(builder, vecOffset);
      var offset = FB.PointData.EndPointData(builder);

      // Finish the buffer with the root table offset
      var wrapper = FB.GSPWrapper.CreateGSPWrapper(builder, FB.Data.PointData, offset.Value);
      builder.Finish(wrapper.Value);

      // Now get the completed buffer
      return builder.SizedByteArray();
    }

    public static Point3d FromPointBuffer(byte[] buffer)
    {
      var ptByteBuffer = new ByteBuffer(buffer);
      var vec = FB.GSPWrapper.GetRootAsGSPWrapper(ptByteBuffer);
      var pt = vec.DataAsPointData();

      var val = pt.Point;
      return val.HasValue ? new Point3d(val.Value.X, val.Value.Y, val.Value.Z)
        : new Point3d(0, 0, 0); // Default value if null
    }
  }
}
