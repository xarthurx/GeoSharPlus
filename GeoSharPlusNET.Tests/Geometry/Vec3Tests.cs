using Xunit;
using GSP.Geometry;

namespace GeoSharPlusNET.Tests.Geometry;

/// <summary>
/// Tests for GSP.Geometry.Vec3
/// </summary>
public class Vec3Tests {
  [Fact]
  public void Constructor_SetsValues() {
    var v = new Vec3(1.0, 2.0, 3.0);
    Assert.Equal(1.0, v.X);
    Assert.Equal(2.0, v.Y);
    Assert.Equal(3.0, v.Z);
  }

  [Fact]
  public void Constructor_FromArray_SetsValues() {
    var v = new Vec3(new double[] { 4.0, 5.0, 6.0 });
    Assert.Equal(4.0, v.X);
    Assert.Equal(5.0, v.Y);
    Assert.Equal(6.0, v.Z);
  }

  [Fact]
  public void Zero_ReturnsZeroVector() {
    var v = Vec3.Zero;
    Assert.Equal(0.0, v.X);
    Assert.Equal(0.0, v.Y);
    Assert.Equal(0.0, v.Z);
  }

  [Fact]
  public void UnitX_ReturnsCorrectVector() {
    var v = Vec3.UnitX;
    Assert.Equal(1.0, v.X);
    Assert.Equal(0.0, v.Y);
    Assert.Equal(0.0, v.Z);
  }

  [Fact]
  public void UnitY_ReturnsCorrectVector() {
    var v = Vec3.UnitY;
    Assert.Equal(0.0, v.X);
    Assert.Equal(1.0, v.Y);
    Assert.Equal(0.0, v.Z);
  }

  [Fact]
  public void UnitZ_ReturnsCorrectVector() {
    var v = Vec3.UnitZ;
    Assert.Equal(0.0, v.X);
    Assert.Equal(0.0, v.Y);
    Assert.Equal(1.0, v.Z);
  }

  [Fact]
  public void Length_ReturnsCorrectValue() {
    var v = new Vec3(1.0, 2.0, 2.0);
    Assert.Equal(3.0, v.Length, 1e-10);
  }

  [Fact]
  public void LengthSquared_ReturnsCorrectValue() {
    var v = new Vec3(1.0, 2.0, 2.0);
    Assert.Equal(9.0, v.LengthSquared, 1e-10);
  }

  [Fact]
  public void Normalized_ReturnsUnitVector() {
    var v = new Vec3(0.0, 3.0, 4.0);
    var n = v.Normalized;
    Assert.Equal(1.0, n.Length, 1e-10);
  }

  [Fact]
  public void Addition_Works() {
    var a = new Vec3(1.0, 2.0, 3.0);
    var b = new Vec3(4.0, 5.0, 6.0);
    var c = a + b;
    Assert.Equal(5.0, c.X);
    Assert.Equal(7.0, c.Y);
    Assert.Equal(9.0, c.Z);
  }

  [Fact]
  public void Subtraction_Works() {
    var a = new Vec3(5.0, 7.0, 9.0);
    var b = new Vec3(1.0, 2.0, 3.0);
    var c = a - b;
    Assert.Equal(4.0, c.X);
    Assert.Equal(5.0, c.Y);
    Assert.Equal(6.0, c.Z);
  }

  [Fact]
  public void ScalarMultiplication_Works() {
    var v = new Vec3(1.0, 2.0, 3.0);
    var c = v * 2.0;
    Assert.Equal(2.0, c.X);
    Assert.Equal(4.0, c.Y);
    Assert.Equal(6.0, c.Z);
  }

  [Fact]
  public void ScalarDivision_Works() {
    var v = new Vec3(2.0, 4.0, 6.0);
    var c = v / 2.0;
    Assert.Equal(1.0, c.X);
    Assert.Equal(2.0, c.Y);
    Assert.Equal(3.0, c.Z);
  }

  [Fact]
  public void Negation_Works() {
    var v = new Vec3(1.0, -2.0, 3.0);
    var n = -v;
    Assert.Equal(-1.0, n.X);
    Assert.Equal(2.0, n.Y);
    Assert.Equal(-3.0, n.Z);
  }

  [Fact]
  public void Dot_ReturnsCorrectValue() {
    var a = new Vec3(1.0, 2.0, 3.0);
    var b = new Vec3(4.0, 5.0, 6.0);
    Assert.Equal(32.0, Vec3.Dot(a, b));
  }

  [Fact]
  public void Cross_ReturnsCorrectValue() {
    var a = new Vec3(1.0, 0.0, 0.0);
    var b = new Vec3(0.0, 1.0, 0.0);
    var c = Vec3.Cross(a, b);
    Assert.Equal(0.0, c.X);
    Assert.Equal(0.0, c.Y);
    Assert.Equal(1.0, c.Z);
  }

  [Fact]
  public void Distance_ReturnsCorrectValue() {
    var a = new Vec3(0.0, 0.0, 0.0);
    var b = new Vec3(1.0, 2.0, 2.0);
    Assert.Equal(3.0, Vec3.Distance(a, b), 1e-10);
  }

  [Fact]
  public void Equality_Works() {
    var a = new Vec3(1.0, 2.0, 3.0);
    var b = new Vec3(1.0, 2.0, 3.0);
    var c = new Vec3(1.0, 2.0, 4.0);
    Assert.Equal(a, b);
    Assert.NotEqual(a, c);
  }

  [Fact]
  public void ApproximatelyEquals_Works() {
    var a = new Vec3(1.0, 2.0, 3.0);
    var b = new Vec3(1.0 + 1e-12, 2.0 - 1e-12, 3.0 + 1e-12);
    Assert.True(a.ApproximatelyEquals(b));
  }

  [Fact]
  public void ApproximatelyEquals_FailsForDifferentVectors() {
    var a = new Vec3(1.0, 2.0, 3.0);
    var b = new Vec3(1.1, 2.0, 3.0);
    Assert.False(a.ApproximatelyEquals(b));
  }

  [Fact]
  public void ToArray_ReturnsCorrectArray() {
    var v = new Vec3(1.0, 2.0, 3.0);
    var arr = v.ToArray();
    Assert.Equal(new double[] { 1.0, 2.0, 3.0 }, arr);
  }

  [Fact]
  public void ToString_ReturnsFormattedString() {
    var v = new Vec3(1.5, 2.5, 3.5);
    var str = v.ToString();
    Assert.Contains("1.5", str);
    Assert.Contains("2.5", str);
    Assert.Contains("3.5", str);
  }

  [Fact]
  public void GetHashCode_SameForEqualVectors() {
    var a = new Vec3(1.0, 2.0, 3.0);
    var b = new Vec3(1.0, 2.0, 3.0);
    Assert.Equal(a.GetHashCode(), b.GetHashCode());
  }
}
