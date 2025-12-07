// ============================================
// GeoSharPlus Runtime Initialization
// ============================================
// This file handles cross-platform native library loading (Windows/macOS).
// It provides utilities for checking library status and error logging.
//
// For P/Invoke declarations, see:
//   - GeoSharPlusNET/Extensions/ (example extensions)
//   - Add your own P/Invoke declarations in Extensions/
// ============================================

using System.Runtime.InteropServices;

namespace GSP {
  
/// <summary>
/// Runtime initialization and platform utilities.
/// Handles cross-platform native library loading for Windows and macOS.
/// </summary>
public static class RuntimeInit {
  public const string WinLibName = @"GeoSharPlusCPP.dll";
  public const string MacLibName = @"libGeoSharPlusCPP.dylib";

  // System debugging functions for cross-platform support

#region cross - platform debug
  // Store error messages that can be retrieved by Grasshopper components
  private static readonly List<string> _errorLog = new List<string>();
  private static bool _isNativeLibraryLoaded = false;
  private static string _loadedLibraryPath = string.Empty;
  private static bool _initializationAttempted = false;

  /// <summary>
  /// Returns true if the native library was successfully loaded
  /// </summary>
  public static bool IsNativeLibraryLoaded {
    get {
      if (!_initializationAttempted) {
        InitializeNativeLibrary();
      }
      return _isNativeLibraryLoaded;
    }
  }

  /// <summary>
  /// Path where the library was loaded from (empty if not loaded)
  /// </summary>
  public static string LoadedLibraryPath {
    get {
      if (!_initializationAttempted) {
        InitializeNativeLibrary();
      }
      return _loadedLibraryPath;
    }
  }

  /// <summary>
  /// Get error messages that can be displayed in Grasshopper components
  /// </summary>
  public static string[] GetErrorMessages() {
    lock (_errorLog) {
      return _errorLog.ToArray();
    }
  }

  /// <summary>
  /// Clear the error log
  /// </summary>
  public static void ClearErrorLog() {
    lock (_errorLog) {
      _errorLog.Clear();
    }
  }

  /// <summary>
  /// Add an error message to the log
  /// </summary>
  private static void LogError(string message) {
    lock (_errorLog) {
      _errorLog.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");

      // Keep log at a reasonable size
      if (_errorLog.Count > 100)
        _errorLog.RemoveAt(0);
    }
  }

  // P/Invoke declarations for macOS dynamic library loading
  [DllImport("libdl.dylib", EntryPoint = "dlopen")]
  private static extern IntPtr dlopen_mac(string path, int flags);

  [DllImport("libdl.dylib", EntryPoint = "dlerror")]
  private static extern IntPtr dlerror_mac();
  
  private static string dlerror() {
    IntPtr ptr = dlerror_mac();
    if (ptr == IntPtr.Zero) return string.Empty;
    return Marshal.PtrToStringAnsi(ptr) ?? string.Empty;
  }

  /// <summary>
  /// Initialize native library loading - called lazily instead of in static constructor
  /// This prevents the entire assembly from failing to load if the native lib isn't found
  /// </summary>
  private static void InitializeNativeLibrary() {
    if (_initializationAttempted) return;
    
    lock (_errorLog) {
      if (_initializationAttempted) return;
      _initializationAttempted = true;
      
      try {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
          InitializeMacOSLibrary();
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
          InitializeWindowsLibrary();
        } else {
          LogError("Unsupported operating system. Only Windows and macOS are supported.");
        }
      } catch (Exception ex) {
        LogError($"Exception during native library initialization: {ex.Message}");
        LogError($"Stack trace: {ex.StackTrace}");
      }
    }
  }

  private static void InitializeMacOSLibrary() {
    try {
      // List all possible library locations to try
      var searchLocations = new List<string>();

      // 1. Load from assembly directory
      string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
      string? assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
      if (assemblyDirectory != null) {
        searchLocations.Add(Path.Combine(assemblyDirectory, MacLibName));
      }

      // 2. Try parent directory (sometimes needed for GH plugins)
      if (assemblyDirectory != null) {
        string? parentDir = Path.GetDirectoryName(assemblyDirectory);
        if (parentDir != null) {
          searchLocations.Add(Path.Combine(parentDir, MacLibName));
        }
      }

      // 3. Try current directory
      searchLocations.Add(Path.Combine(Directory.GetCurrentDirectory(), MacLibName));

      // 4. Add standard system locations
      searchLocations.Add(MacLibName);  // Default system search paths

      LogError($"Searching for {MacLibName} in the following locations:");
      foreach (var path in searchLocations) {
        LogError($"- {path} (exists: {File.Exists(path)})");
      }

      // Try to load from each location
      IntPtr handle = IntPtr.Zero;
      foreach (var libraryPath in searchLocations) {
        if (File.Exists(libraryPath)) {
          LogError($"Attempting to load native library from: {libraryPath}");
          
          try {
            handle = dlopen_mac(libraryPath, 2);  // RTLD_NOW = 2

            if (handle != IntPtr.Zero) {
              _isNativeLibraryLoaded = true;
              _loadedLibraryPath = libraryPath;
              LogError($"Successfully loaded native library from: {libraryPath}");
              return;
            } else {
              string errorMsg = dlerror();
              LogError($"Failed to load library from {libraryPath}: {errorMsg}");
            }
          } catch (Exception ex) {
            LogError($"Exception loading from {libraryPath}: {ex.Message}");
          }
        }
      }

      LogError($"Failed to load native library from any location. Plugin functionality will be limited.");
    } catch (Exception ex) {
      LogError($"Exception while setting up native library path: {ex.Message}");
      LogError($"Stack trace: {ex.StackTrace}");
    }
  }

  private static void InitializeWindowsLibrary() {
    try {
      // Locate the DLL file for Windows
      string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
      string? assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
      string dllPath = string.Empty;

      if (assemblyDirectory != null) {
        dllPath = Path.Combine(assemblyDirectory, WinLibName);
        if (!File.Exists(dllPath)) {
          // Try parent directory
          string? parentDir = Path.GetDirectoryName(assemblyDirectory);
          if (parentDir != null) {
            dllPath = Path.Combine(parentDir, WinLibName);
          }
        }
      }

      if (File.Exists(dllPath)) {
        _isNativeLibraryLoaded = true;
        _loadedLibraryPath = dllPath;
        LogError($"Successfully located native library at: {dllPath}");
      } else {
        LogError($"Failed to locate native library {WinLibName} in expected locations.");
        LogError($"Searched: {dllPath}");
      }
    } catch (Exception ex) {
      LogError($"Exception while locating native library path: {ex.Message}");
      LogError($"Stack trace: {ex.StackTrace}");
    }
  }

#endregion
}
}
