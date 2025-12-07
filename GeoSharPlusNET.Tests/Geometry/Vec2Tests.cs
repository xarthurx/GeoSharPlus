using Xunit;
using GSP.Geometry;

namespace GeoSharPlusNET.Tests.Geometry;

/// <summary>
/// Tests for GSP.Geometry.Vec2
/// </summary>
public class Vec2Tests {
  [Fact]
  public void Constructor_SetsValues() {
    var v = new Vec2(1.0, 2.0);
    Assert.Equal(1.0, v.X);
    Assert.Equal(2.0, v.Y);
  }

  [Fact]
  public void Constructor_FromArray_SetsValues() {
    var v = new Vec2(new double[] { 3.0, 4.0 });
    Assert.Equal(3.0, v.X);
    Assert.Equal(4.0, v.Y);
  }

  [Fact]
  public void Zero_ReturnsZeroVector() {
    var v = Vec2.Zero;
    Assert.Equal(0.0, v.X);
    Assert.Equal(0.0, v.Y);
  }

  [Fact]
  public void UnitX_ReturnsCorrectVector() {
    var v = Vec2.UnitX;
    Assert.Equal(1.0, v.X);
    Assert.Equal(0.0, v.Y);
  }

  [Fact]
  public void UnitY_ReturnsCorrectVector() {
    var v = Vec2.UnitY;
    Assert.Equal(0.0, v.X);
    Assert.Equal(1.0, v.Y);
  }

  [Fact]
  public void Length_ReturnsCorrectValue() {
    var v = new Vec2(3.0, 4.0);
    Assert.Equal(5.0, v.Length, 1e-10);
  }

  [Fact]
  public void LengthSquared_ReturnsCorrectValue() {
    var v = new Vec2(3.0, 4.0);
    Assert.Equal(25.0, v.LengthSquared, 1e-10);
  }

  [Fact]
  public void Normalized_ReturnsUnitVector() {
    var v = new Vec2(3.0, 4.0);
    var n = v.Normalized;
    Assert.Equal(1.0, n.Length, 1e-10);
    Assert.Equal(0.6, n.X, 1e-10);
    Assert.Equal(0.8, n.Y, 1e-10);
  }

  [Fact]
  public void Addition_Works() {
    var a = new Vec2(1.0, 2.0);
    var b = new Vec2(3.0, 4.0);
    var c = a + b;
    Assert.Equal(4.0, c.X);
    Assert.Equal(6.0, c.Y);
  }

  [Fact]
  public void Subtraction_Works() {
    var a = new Vec2(5.0, 7.0);
    var b = new Vec2(2.0, 3.0);
    var c = a - b;
    Assert.Equal(3.0, c.X);
    Assert.Equal(4.0, c.Y);
  }

  [Fact]
  public void ScalarMultiplication_Works() {
    var v = new Vec2(2.0, 3.0);
    var c = v * 2.0;
    Assert.Equal(4.0, c.X);
    Assert.Equal(6.0, c.Y);
  }

  [Fact]
  public void ScalarDivision_Works() {
    var v = new Vec2(4.0, 6.0);
    var c = v / 2.0;
    Assert.Equal(2.0, c.X);
    Assert.Equal(3.0, c.Y);
  }

  [Fact]
  public void Negation_Works() {
    var v = new Vec2(1.0, -2.0);
    var n = -v;
    Assert.Equal(-1.0, n.X);
    Assert.Equal(2.0, n.Y);
  }

  [Fact]
  public void Dot_ReturnsCorrectValue() {
    var a = new Vec2(1.0, 2.0);
    var b = new Vec2(3.0, 4.0);
    Assert.Equal(11.0, Vec2.Dot(a, b));
  }

  [Fact]
  public void Cross_ReturnsCorrectValue() {
    var a = new Vec2(1.0, 0.0);
    var b = new Vec2(0.0, 1.0);
    Assert.Equal(1.0, Vec2.Cross(a, b));
  }

  [Fact]
  public void Distance_ReturnsCorrectValue() {
    var a = new Vec2(0.0, 0.0);
    var b = new Vec2(3.0, 4.0);
    Assert.Equal(5.0, Vec2.Distance(a, b), 1e-10);
  }

  [Fact]
  public void Equality_Works() {
    var a = new Vec2(1.0, 2.0);
    var b = new Vec2(1.0, 2.0);
    var c = new Vec2(1.0, 3.0);
    Assert.Equal(a, b);
    Assert.NotEqual(a, c);
  }

  [Fact]
  public void ApproximatelyEquals_Works() {
    var a = new Vec2(1.0, 2.0);
    var b = new Vec2(1.0 + 1e-12, 2.0 - 1e-12);
    Assert.True(a.ApproximatelyEquals(b));
  }

  [Fact]
  public void ToArray_ReturnsCorrectArray() {
    var v = new Vec2(1.0, 2.0);
    var arr = v.ToArray();
    Assert.Equal(new double[] { 1.0, 2.0 }, arr);
  }

  [Fact]
  public void ToVec3_Works() {
    var v = new Vec2(1.0, 2.0);
    var v3 = v.ToVec3(3.0);
    Assert.Equal(1.0, v3.X);
    Assert.Equal(2.0, v3.Y);
    Assert.Equal(3.0, v3.Z);
  }

  [Fact]
  public void ToString_ReturnsFormattedString() {
    var v = new Vec2(1.5, 2.5);
    Assert.Contains("1.5", v.ToString());
    Assert.Contains("2.5", v.ToString());
  }
}
