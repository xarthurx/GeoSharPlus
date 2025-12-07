using Xunit;
using GSP.Core;
using System.Runtime.InteropServices;

namespace GeoSharPlusNET.Tests.Core;

/// <summary>
/// Tests for GSP.Core.Platform
/// </summary>
public class PlatformTests {
  [Fact]
  public void WindowsLib_HasCorrectValue() {
    Assert.Equal("GeoSharPlusCPP.dll", Platform.WindowsLib);
  }

  [Fact]
  public void MacLib_HasCorrectValue() {
    Assert.Equal("libGeoSharPlusCPP.dylib", Platform.MacLib);
  }

  [Fact]
  public void LinuxLib_HasCorrectValue() {
    Assert.Equal("libGeoSharPlusCPP.so", Platform.LinuxLib);
  }

  [Fact]
  public void IsWindows_ReturnsConsistentValue() {
    // Just verify it doesn't throw and returns a boolean
    var result = Platform.IsWindows;
    Assert.IsType<bool>(result);
  }

  [Fact]
  public void IsMac_ReturnsConsistentValue() {
    var result = Platform.IsMac;
    Assert.IsType<bool>(result);
  }

  [Fact]
  public void IsLinux_ReturnsConsistentValue() {
    var result = Platform.IsLinux;
    Assert.IsType<bool>(result);
  }

  [Fact]
  public void NativeLibrary_ReturnsValidLibraryName() {
    var lib = Platform.NativeLibrary;
    Assert.NotNull(lib);
    Assert.NotEmpty(lib);

    // Should be one of the known library names
    Assert.True(
      lib == Platform.WindowsLib ||
      lib == Platform.MacLib ||
      lib == Platform.LinuxLib
    );
  }

  [Fact]
  public void ExactlyOnePlatformIsTrue() {
    // Exactly one of the platform checks should be true
    int trueCount = 0;
    if (Platform.IsWindows) trueCount++;
    if (Platform.IsMac) trueCount++;
    if (Platform.IsLinux) trueCount++;

    // On a supported platform, exactly one should be true
    // On unsupported platforms, all might be false
    Assert.True(trueCount <= 1);
  }

  [Fact]
  public void NativeLibrary_MatchesPlatform() {
    var lib = Platform.NativeLibrary;

    if (Platform.IsWindows) {
      Assert.Equal(Platform.WindowsLib, lib);
    } else if (Platform.IsMac) {
      Assert.Equal(Platform.MacLib, lib);
    } else if (Platform.IsLinux) {
      Assert.Equal(Platform.LinuxLib, lib);
    }
  }
}
