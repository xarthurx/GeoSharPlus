using Google.FlatBuffers;
using Rhino.Geometry;

namespace GSP
{
  public static class Wrapper
  {
    // Point3d IO
    public static byte[] ToPointBuffer(Point3d pt)
    {
      var builder = new FlatBufferBuilder(64); // Enough for a single Vector3d

      // Create a Vector3d in the FlatBuffer
      FB.PointData.StartPointData(builder);
      var vecOffset = FB.Vec3.CreateVec3(builder, pt.X, pt.Y, pt.Z);
      FB.PointData.AddPoint(builder, vecOffset);
      var ptOffset = FB.PointData.EndPointData(builder);

      // Finish the buffer with the root table offset
      builder.Finish(ptOffset.Value);

      // Now get the completed buffer
      return builder.SizedByteArray();
    }

    public static Point3d FromPointBuffer(byte[] buffer)
    {
      var ptByteBuffer = new ByteBuffer(buffer);
      var pt = FB.PointData.GetRootAsPointData(ptByteBuffer).Point;

      return pt.HasValue ? new Point3d(pt.Value.X, pt.Value.Y, pt.Value.Z)
        : new Point3d(0, 0, 0); // Default value if null
    }

    // Point3dArray IO
    public static byte[] ToPointArrayBuffer(Point3d[] points)
    {
      var builder = new FlatBufferBuilder(1024);

      // Add points in reverse order (FlatBuffers build vectors backwards) to build the point vector
      FB.PointArrayData.StartPointsVector(builder, points.Length);
      for (int i = points.Length - 1; i >= 0; i--)
      {
        FB.Vec3.CreateVec3(builder, points[i].X, points[i].Y, points[i].Z);
      }
      var ptOffset = builder.EndVector();

      var arrayOffset = FB.PointArrayData.CreatePointArrayData(builder, ptOffset);
      builder.Finish(arrayOffset.Value);

      return builder.SizedByteArray();
    }

    public static Point3d[] FromPointArrayBuffer(byte[] buffer)
    {
      var byteBuffer = new ByteBuffer(buffer);
      var pointArray = FB.PointArrayData.GetRootAsPointArrayData(byteBuffer);

      var pts = pointArray.Points;
      // 1. Check if the array is valid
      if (pointArray.PointsLength == 0)
        return Array.Empty<Point3d>();

      // 2. Extract structs directly
      var res = new Point3d[pointArray.PointsLength];
      for (int i = 0; i < pointArray.PointsLength; i++)
      {
        var pt = pointArray.Points(i);
        res[i] = pt.HasValue ? new Point3d(pt.Value.X, pt.Value.Y, pt.Value.Z) : new Point3d(0, 0, 0); // Default value if null
      }
      return res;
    }
  }
}
