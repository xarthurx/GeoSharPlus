using Xunit;
using GSP.Core;
using System.Runtime.InteropServices;

namespace GeoSharPlusNET.Tests.Core;

/// <summary>
/// Tests for GSP.Core.MarshalHelper
/// </summary>
public class MarshalHelperTests {
  [Fact]
  public void CopyAndFree_WithValidPointer_ReturnsData() {
    // Allocate some memory and write data to it
    int size = 10;
    IntPtr ptr = Marshal.AllocCoTaskMem(size);
    try {
      byte[] testData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
      Marshal.Copy(testData, 0, ptr, size);

      // This will copy and free the memory
      var result = MarshalHelper.CopyAndFree(ptr, size);

      Assert.Equal(testData, result);
    }
    catch {
      // If test fails before CopyAndFree, clean up
      Marshal.FreeCoTaskMem(ptr);
      throw;
    }
    // Note: ptr is freed by CopyAndFree, so don't free it again
  }

  [Fact]
  public void CopyAndFree_WithZeroPointer_ReturnsEmptyArray() {
    var result = MarshalHelper.CopyAndFree(IntPtr.Zero, 100);
    Assert.Empty(result);
  }

  [Fact]
  public void CopyAndFree_WithZeroSize_ReturnsEmptyArray() {
    IntPtr ptr = Marshal.AllocCoTaskMem(10);
    try {
      var result = MarshalHelper.CopyAndFree(ptr, 0);
      Assert.Empty(result);
    }
    finally {
      // ptr was not freed because size was 0
      Marshal.FreeCoTaskMem(ptr);
    }
  }

  [Fact]
  public void CopyAndFree_WithNegativeSize_ReturnsEmptyArray() {
    IntPtr ptr = Marshal.AllocCoTaskMem(10);
    try {
      var result = MarshalHelper.CopyAndFree(ptr, -5);
      Assert.Empty(result);
    }
    finally {
      Marshal.FreeCoTaskMem(ptr);
    }
  }

  [Fact]
  public void Copy_WithValidPointer_ReturnsData() {
    int size = 5;
    IntPtr ptr = Marshal.AllocCoTaskMem(size);
    try {
      byte[] testData = new byte[] { 10, 20, 30, 40, 50 };
      Marshal.Copy(testData, 0, ptr, size);

      var result = MarshalHelper.Copy(ptr, size);

      Assert.Equal(testData, result);
    }
    finally {
      Marshal.FreeCoTaskMem(ptr);
    }
  }

  [Fact]
  public void Copy_WithZeroPointer_ReturnsEmptyArray() {
    var result = MarshalHelper.Copy(IntPtr.Zero, 100);
    Assert.Empty(result);
  }

  [Fact]
  public void Copy_WithZeroSize_ReturnsEmptyArray() {
    IntPtr ptr = Marshal.AllocCoTaskMem(10);
    try {
      var result = MarshalHelper.Copy(ptr, 0);
      Assert.Empty(result);
    }
    finally {
      Marshal.FreeCoTaskMem(ptr);
    }
  }

  [Fact]
  public void Free_WithValidPointer_DoesNotThrow() {
    IntPtr ptr = Marshal.AllocCoTaskMem(10);
    var exception = Record.Exception(() => MarshalHelper.Free(ptr));
    Assert.Null(exception);
  }

  [Fact]
  public void Free_WithZeroPointer_DoesNotThrow() {
    var exception = Record.Exception(() => MarshalHelper.Free(IntPtr.Zero));
    Assert.Null(exception);
  }
}
