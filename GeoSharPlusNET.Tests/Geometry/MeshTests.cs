using Xunit;
using GSP.Geometry;

namespace GeoSharPlusNET.Tests.Geometry;

/// <summary>
/// Tests for GSP.Geometry.Mesh
/// </summary>
public class MeshTests {
  [Fact]
  public void Constructor_CreatesEmptyMesh() {
    var mesh = new Mesh();
    Assert.Empty(mesh.Vertices);
    Assert.Empty(mesh.TriangleFaces);
    Assert.Empty(mesh.QuadFaces);
  }

  [Fact]
  public void Constructor_WithTriangles_Works() {
    var vertices = new Vec3[] {
      new(0, 0, 0),
      new(1, 0, 0),
      new(0, 1, 0)
    };
    var faces = new (int, int, int)[] { (0, 1, 2) };

    var mesh = new Mesh(vertices, faces);

    Assert.Equal(3, mesh.VertexCount);
    Assert.Single(mesh.TriangleFaces);
    Assert.Empty(mesh.QuadFaces);
  }

  [Fact]
  public void Constructor_WithQuads_Works() {
    var vertices = new Vec3[] {
      new(0, 0, 0),
      new(1, 0, 0),
      new(1, 1, 0),
      new(0, 1, 0)
    };
    var faces = new (int, int, int, int)[] { (0, 1, 2, 3) };

    var mesh = new Mesh(vertices, faces);

    Assert.Equal(4, mesh.VertexCount);
    Assert.Empty(mesh.TriangleFaces);
    Assert.Single(mesh.QuadFaces);
  }

  [Fact]
  public void HasTriangles_ReturnsCorrectValue() {
    var mesh = new Mesh {
      Vertices = new Vec3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0) },
      TriangleFaces = new (int, int, int)[] { (0, 1, 2) }
    };

    Assert.True(mesh.HasTriangles);
    Assert.False(mesh.HasQuads);
  }

  [Fact]
  public void HasQuads_ReturnsCorrectValue() {
    var mesh = new Mesh {
      Vertices = new Vec3[] { new(0, 0, 0), new(1, 0, 0), new(1, 1, 0), new(0, 1, 0) },
      QuadFaces = new (int, int, int, int)[] { (0, 1, 2, 3) }
    };

    Assert.False(mesh.HasTriangles);
    Assert.True(mesh.HasQuads);
  }

  [Fact]
  public void FaceCount_ReturnsTotalFaces() {
    var mesh = new Mesh {
      Vertices = new Vec3[] {
        new(0, 0, 0), new(1, 0, 0), new(1, 1, 0), new(0, 1, 0),
        new(2, 0, 0), new(2, 1, 0)
      },
      TriangleFaces = new (int, int, int)[] { (0, 4, 5) },
      QuadFaces = new (int, int, int, int)[] { (0, 1, 2, 3) }
    };

    Assert.Equal(2, mesh.FaceCount);
  }

  [Fact]
  public void VertexCount_ReturnsCorrectValue() {
    var mesh = new Mesh {
      Vertices = new Vec3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0) }
    };

    Assert.Equal(3, mesh.VertexCount);
  }

  [Fact]
  public void IsValid_ReturnsTrueForValidMesh() {
    var mesh = new Mesh {
      Vertices = new Vec3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0) },
      TriangleFaces = new (int, int, int)[] { (0, 1, 2) }
    };

    Assert.True(mesh.IsValid);
  }

  [Fact]
  public void IsValid_ReturnsFalseForEmptyMesh() {
    var mesh = new Mesh();
    Assert.False(mesh.IsValid);
  }

  [Fact]
  public void IsValid_ReturnsFalseForMeshWithoutFaces() {
    var mesh = new Mesh {
      Vertices = new Vec3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0) }
    };

    Assert.False(mesh.IsValid);
  }

  [Fact]
  public void Triangulate_ConvertsQuadsToTriangles() {
    var mesh = new Mesh {
      Vertices = new Vec3[] {
        new(0, 0, 0), new(1, 0, 0), new(1, 1, 0), new(0, 1, 0)
      },
      QuadFaces = new (int, int, int, int)[] { (0, 1, 2, 3) }
    };

    mesh.Triangulate();

    Assert.Equal(2, mesh.TriangleFaces.Length);
    Assert.Empty(mesh.QuadFaces);
    Assert.Equal((0, 1, 2), mesh.TriangleFaces[0]);
    Assert.Equal((0, 2, 3), mesh.TriangleFaces[1]);
  }

  [Fact]
  public void Triangulate_PreservesExistingTriangles() {
    var mesh = new Mesh {
      Vertices = new Vec3[] {
        new(0, 0, 0), new(1, 0, 0), new(1, 1, 0), new(0, 1, 0), new(2, 0, 0)
      },
      TriangleFaces = new (int, int, int)[] { (0, 1, 4) },
      QuadFaces = new (int, int, int, int)[] { (0, 1, 2, 3) }
    };

    mesh.Triangulate();

    Assert.Equal(3, mesh.TriangleFaces.Length);
    Assert.Empty(mesh.QuadFaces);
  }

  [Fact]
  public void Clone_CreatesDeepCopy() {
    var original = new Mesh {
      Vertices = new Vec3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0) },
      TriangleFaces = new (int, int, int)[] { (0, 1, 2) }
    };

    var clone = original.Clone();

    Assert.Equal(original.VertexCount, clone.VertexCount);
    Assert.Equal(original.FaceCount, clone.FaceCount);

    // Modify original and verify clone is unaffected
    original.Vertices[0] = new Vec3(99, 99, 99);
    Assert.NotEqual(original.Vertices[0], clone.Vertices[0]);
  }

  [Fact]
  public void GetBoundingBox_ReturnsCorrectBounds() {
    var mesh = new Mesh {
      Vertices = new Vec3[] {
        new(0, 0, 0),
        new(10, 5, 3),
        new(-2, 8, -1)
      }
    };

    var (min, max) = mesh.GetBoundingBox();

    Assert.Equal(-2, min.X);
    Assert.Equal(0, min.Y);
    Assert.Equal(-1, min.Z);
    Assert.Equal(10, max.X);
    Assert.Equal(8, max.Y);
    Assert.Equal(3, max.Z);
  }

  [Fact]
  public void GetBoundingBox_ReturnsZeroForEmptyMesh() {
    var mesh = new Mesh();
    var (min, max) = mesh.GetBoundingBox();

    Assert.Equal(Vec3.Zero, min);
    Assert.Equal(Vec3.Zero, max);
  }

  [Fact]
  public void ToString_ReturnsFormattedString() {
    var mesh = new Mesh {
      Vertices = new Vec3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0) },
      TriangleFaces = new (int, int, int)[] { (0, 1, 2) }
    };

    var str = mesh.ToString();
    Assert.Contains("V:3", str);
    Assert.Contains("T:1", str);
    Assert.Contains("Q:0", str);
  }

  [Fact]
  public void CubeMesh_HasCorrectStructure() {
    // Create a cube mesh (8 vertices, 6 quad faces)
    var mesh = new Mesh {
      Vertices = new Vec3[] {
        new(0, 0, 0), new(1, 0, 0), new(1, 1, 0), new(0, 1, 0),
        new(0, 0, 1), new(1, 0, 1), new(1, 1, 1), new(0, 1, 1)
      },
      QuadFaces = new (int, int, int, int)[] {
        (0, 1, 2, 3),  // Bottom
        (4, 7, 6, 5),  // Top
        (0, 4, 5, 1),  // Front
        (1, 5, 6, 2),  // Right
        (2, 6, 7, 3),  // Back
        (3, 7, 4, 0)   // Left
      }
    };

    Assert.Equal(8, mesh.VertexCount);
    Assert.Equal(6, mesh.FaceCount);
    Assert.True(mesh.IsValid);
    Assert.True(mesh.HasQuads);
    Assert.False(mesh.HasTriangles);
  }
}
