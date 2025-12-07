using Xunit;
using GSP.Core;
using GSP.Geometry;

namespace GeoSharPlusNET.Tests.Core;

/// <summary>
/// Tests for GSP.Core.Serializer
/// </summary>
public class SerializerTests {
  #region Vec3 Serialization

  [Fact]
  public void Serialize_Vec3_RoundTrip() {
    var original = new Vec3(1.5, 2.5, 3.5);

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeVec3(buffer);

    Assert.True(original.ApproximatelyEquals(result));
  }

  [Fact]
  public void Serialize_Vec3_Zero() {
    var original = Vec3.Zero;

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeVec3(buffer);

    Assert.Equal(original, result);
  }

  [Fact]
  public void Serialize_Vec3_NegativeValues() {
    var original = new Vec3(-100.5, -200.25, -300.125);

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeVec3(buffer);

    Assert.True(original.ApproximatelyEquals(result));
  }

  [Fact]
  public void Serialize_Vec3_LargeValues() {
    var original = new Vec3(1e15, 2e15, 3e15);

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeVec3(buffer);

    Assert.True(original.ApproximatelyEquals(result, 1e6)); // Larger tolerance for large values
  }

  #endregion

  #region Vec3 Array Serialization

  [Fact]
  public void Serialize_Vec3Array_RoundTrip() {
    var original = new Vec3[] {
      new(1, 2, 3),
      new(4, 5, 6),
      new(7, 8, 9)
    };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeVec3Array(buffer);

    Assert.Equal(original.Length, result.Length);
    for (int i = 0; i < original.Length; i++) {
      Assert.True(original[i].ApproximatelyEquals(result[i]));
    }
  }

  [Fact]
  public void Serialize_Vec3Array_Empty() {
    var original = Array.Empty<Vec3>();

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeVec3Array(buffer);

    Assert.Empty(result);
  }

  [Fact]
  public void Serialize_Vec3Array_Single() {
    var original = new Vec3[] { new(42, 43, 44) };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeVec3Array(buffer);

    Assert.Single(result);
    Assert.True(original[0].ApproximatelyEquals(result[0]));
  }

  [Fact]
  public void Serialize_Vec3Array_Large() {
    var original = new Vec3[1000];
    for (int i = 0; i < 1000; i++) {
      original[i] = new Vec3(i, i * 2, i * 3);
    }

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeVec3Array(buffer);

    Assert.Equal(1000, result.Length);
    for (int i = 0; i < 1000; i++) {
      Assert.True(original[i].ApproximatelyEquals(result[i]));
    }
  }

  [Fact]
  public void Serialize_Vec3List_RoundTrip() {
    var original = new List<Vec3> {
      new(1, 2, 3),
      new(4, 5, 6)
    };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeVec3List(buffer);

    Assert.Equal(original.Count, result.Count);
  }

  #endregion

  #region Mesh Serialization

  [Fact]
  public void Serialize_Mesh_TriangleMesh_RoundTrip() {
    var original = new Mesh {
      Vertices = new Vec3[] {
        new(0, 0, 0),
        new(1, 0, 0),
        new(0, 1, 0)
      },
      TriangleFaces = new (int, int, int)[] { (0, 1, 2) }
    };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeMesh(buffer);

    Assert.Equal(original.VertexCount, result.VertexCount);
    Assert.Equal(original.TriangleFaces.Length, result.TriangleFaces.Length);
    Assert.Equal(original.TriangleFaces[0], result.TriangleFaces[0]);
  }

  [Fact]
  public void Serialize_Mesh_QuadMesh_RoundTrip() {
    var original = new Mesh {
      Vertices = new Vec3[] {
        new(0, 0, 0),
        new(1, 0, 0),
        new(1, 1, 0),
        new(0, 1, 0)
      },
      QuadFaces = new (int, int, int, int)[] { (0, 1, 2, 3) }
    };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeMesh(buffer);

    Assert.Equal(original.VertexCount, result.VertexCount);
    Assert.Equal(original.QuadFaces.Length, result.QuadFaces.Length);
    Assert.Equal(original.QuadFaces[0], result.QuadFaces[0]);
  }

  [Fact]
  public void Serialize_Mesh_CubeMesh_RoundTrip() {
    var original = new Mesh {
      Vertices = new Vec3[] {
        new(0, 0, 0), new(1, 0, 0), new(1, 1, 0), new(0, 1, 0),
        new(0, 0, 1), new(1, 0, 1), new(1, 1, 1), new(0, 1, 1)
      },
      QuadFaces = new (int, int, int, int)[] {
        (0, 1, 2, 3),
        (4, 7, 6, 5),
        (0, 4, 5, 1),
        (1, 5, 6, 2),
        (2, 6, 7, 3),
        (3, 7, 4, 0)
      }
    };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeMesh(buffer);

    Assert.Equal(8, result.VertexCount);
    Assert.Equal(6, result.QuadFaces.Length);
  }

  [Fact]
  public void Serialize_Mesh_WithTriangulation_ConvertsQuads() {
    var original = new Mesh {
      Vertices = new Vec3[] {
        new(0, 0, 0), new(1, 0, 0), new(1, 1, 0), new(0, 1, 0)
      },
      QuadFaces = new (int, int, int, int)[] { (0, 1, 2, 3) }
    };

    var buffer = Serializer.Serialize(original, triangulate: true);
    var result = Serializer.DeserializeMesh(buffer);

    Assert.Equal(4, result.VertexCount);
    Assert.Equal(2, result.TriangleFaces.Length);
    Assert.Empty(result.QuadFaces);
  }

  #endregion

  #region Int Array Serialization

  [Fact]
  public void Serialize_IntArray_RoundTrip() {
    var original = new int[] { 1, 2, 3, 4, 5 };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeIntArray(buffer);

    Assert.Equal(original, result);
  }

  [Fact]
  public void Serialize_IntArray_Empty() {
    var original = Array.Empty<int>();

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeIntArray(buffer);

    Assert.Empty(result);
  }

  [Fact]
  public void Serialize_IntArray_NegativeValues() {
    var original = new int[] { -100, -50, 0, 50, 100 };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeIntArray(buffer);

    Assert.Equal(original, result);
  }

  #endregion

  #region Double Array Serialization

  [Fact]
  public void Serialize_DoubleArray_RoundTrip() {
    var original = new double[] { 1.1, 2.2, 3.3, 4.4, 5.5 };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeDoubleArray(buffer);

    Assert.Equal(original.Length, result.Length);
    for (int i = 0; i < original.Length; i++) {
      Assert.Equal(original[i], result[i], 1e-10);
    }
  }

  [Fact]
  public void Serialize_DoubleArray_Empty() {
    var original = Array.Empty<double>();

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeDoubleArray(buffer);

    Assert.Empty(result);
  }

  #endregion

  #region Int Pair Array Serialization

  [Fact]
  public void Serialize_IntPairArray_RoundTrip() {
    var original = new (int, int)[] { (1, 2), (3, 4), (5, 6) };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeIntPairArray(buffer);

    Assert.Equal(original, result);
  }

  [Fact]
  public void Serialize_IntPairArray_Empty() {
    var original = Array.Empty<(int, int)>();

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeIntPairArray(buffer);

    Assert.Empty(result);
  }

  #endregion

  #region Double Pair Array Serialization

  [Fact]
  public void Serialize_DoublePairArray_RoundTrip() {
    var original = new (double, double)[] { (1.1, 2.2), (3.3, 4.4) };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeDoublePairArray(buffer);

    Assert.Equal(original.Length, result.Length);
    for (int i = 0; i < original.Length; i++) {
      Assert.Equal(original[i].Item1, result[i].Item1, 1e-10);
      Assert.Equal(original[i].Item2, result[i].Item2, 1e-10);
    }
  }

  #endregion

  #region Nested Int Array Serialization

  [Fact]
  public void Serialize_NestedIntArray_RoundTrip() {
    var original = new List<List<int>> {
      new() { 1, 2, 3 },
      new() { 4, 5 },
      new() { 6, 7, 8, 9 }
    };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeNestedIntArray(buffer);

    Assert.Equal(original.Count, result.Count);
    for (int i = 0; i < original.Count; i++) {
      Assert.Equal(original[i], result[i]);
    }
  }

  [Fact]
  public void Serialize_NestedIntArray_Empty() {
    var original = new List<List<int>>();

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeNestedIntArray(buffer);

    Assert.Empty(result);
  }

  [Fact]
  public void Serialize_NestedIntArray_WithEmptySubArrays() {
    var original = new List<List<int>> {
      new() { 1, 2 },
      new(),  // Empty
      new() { 3 }
    };

    var buffer = Serializer.Serialize(original);
    var result = Serializer.DeserializeNestedIntArray(buffer);

    Assert.Equal(3, result.Count);
    Assert.Equal(2, result[0].Count);
    Assert.Empty(result[1]);
    Assert.Single(result[2]);
  }

  #endregion
}
