# GeoSharPlus Upstream Sync Script
# This script helps you pull updates from the upstream GeoSharPlus template repository.
#
# Usage:
#   .\scripts\sync-upstream.ps1           # Interactive mode - shows diffs and asks for confirmation
#   .\scripts\sync-upstream.ps1 -Force    # Applies changes without confirmation
#   .\scripts\sync-upstream.ps1 -DryRun   # Only shows what would change, doesn't apply

param(
    [switch]$Force,
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

# Define upstream files to sync
$UpstreamFiles = @(
    # C++ Core Headers
    "GeoSharPlusCPP/include/GeoSharPlusCPP/API/BridgeAPI.h",
    "GeoSharPlusCPP/include/GeoSharPlusCPP/Core/Geometry.h",
    "GeoSharPlusCPP/include/GeoSharPlusCPP/Core/Macro.h",
    "GeoSharPlusCPP/include/GeoSharPlusCPP/Core/MathTypes.h",
    "GeoSharPlusCPP/include/GeoSharPlusCPP/Serialization/Serializer.h",
    
    # C++ Core Sources
    "GeoSharPlusCPP/src/API/BridgeAPI.cpp",
    "GeoSharPlusCPP/src/Core/Geometry.cpp",
    "GeoSharPlusCPP/src/Serialization/Serializer.cpp",
    
    # C++ Build Configuration
    "GeoSharPlusCPP/CMakeLists.txt",
    "GeoSharPlusCPP/CMakePresets.json",
    "GeoSharPlusCPP/vcpkg.json",
    
    # Core Schemas
    "GeoSharPlusCPP/schema/base.fbs",
    "GeoSharPlusCPP/schema/doubleArray.fbs",
    "GeoSharPlusCPP/schema/doublePairArray.fbs",
    "GeoSharPlusCPP/schema/intArray.fbs",
    "GeoSharPlusCPP/schema/intNestedArray.fbs",
    "GeoSharPlusCPP/schema/intPairArray.fbs",
    "GeoSharPlusCPP/schema/mesh.fbs",
    "GeoSharPlusCPP/schema/point.fbs",
    "GeoSharPlusCPP/schema/pointArray.fbs",
    
    # C# Core Files
    "GeoSharPlusNET/NativeBridge.cs",
    "GeoSharPlusNET/Wrapper.cs",
    "GeoSharPlusNET/MeshUtils.cs",
    "GeoSharPlusNET/GeoSharPlusNET.csproj"
)

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "GeoSharPlus Upstream Sync" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Check if upstream remote exists
$upstreamUrl = git remote get-url upstream 2>$null
if (-not $upstreamUrl) {
    Write-Host "Upstream remote not found. Adding it now..." -ForegroundColor Yellow
    git remote add upstream https://github.com/xarthurx/GeoSharPlus.git
    Write-Host "Added upstream remote: https://github.com/xarthurx/GeoSharPlus.git" -ForegroundColor Green
}

# Fetch latest from upstream
Write-Host "Fetching latest from upstream..." -ForegroundColor Yellow
git fetch upstream

Write-Host ""
Write-Host "Checking for changes in upstream files..." -ForegroundColor Yellow
Write-Host ""

$changedFiles = @()
$unchangedFiles = @()

foreach ($file in $UpstreamFiles) {
    # Check if file has changes
    $diff = git diff main upstream/main -- $file 2>$null
    
    if ($diff) {
        $changedFiles += $file
        Write-Host "[CHANGED] $file" -ForegroundColor Yellow
    } else {
        $unchangedFiles += $file
    }
}

Write-Host ""
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Changed files: $($changedFiles.Count)" -ForegroundColor Yellow
Write-Host "  Unchanged files: $($unchangedFiles.Count)" -ForegroundColor Green
Write-Host ""

if ($changedFiles.Count -eq 0) {
    Write-Host "No upstream changes to apply. You're up to date!" -ForegroundColor Green
    exit 0
}

if ($DryRun) {
    Write-Host "=== DRY RUN MODE ===" -ForegroundColor Magenta
    Write-Host "The following files would be updated:" -ForegroundColor Magenta
    foreach ($file in $changedFiles) {
        Write-Host "  - $file" -ForegroundColor Yellow
    }
    Write-Host ""
    Write-Host "Run without -DryRun to apply changes." -ForegroundColor Magenta
    exit 0
}

# Show detailed diffs if not forcing
if (-not $Force) {
    Write-Host "=== Detailed Changes ===" -ForegroundColor Cyan
    foreach ($file in $changedFiles) {
        Write-Host ""
        Write-Host "--- $file ---" -ForegroundColor Yellow
        git diff main upstream/main -- $file | Select-Object -First 50
        Write-Host "..."
    }
    
    Write-Host ""
    $confirm = Read-Host "Apply these changes? (y/n)"
    if ($confirm -ne 'y' -and $confirm -ne 'Y') {
        Write-Host "Aborted." -ForegroundColor Red
        exit 1
    }
}

# Apply changes
Write-Host ""
Write-Host "Applying upstream changes..." -ForegroundColor Yellow

foreach ($file in $changedFiles) {
    try {
        git checkout upstream/main -- $file
        Write-Host "  Updated: $file" -ForegroundColor Green
    } catch {
        Write-Host "  Failed: $file - $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Sync complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Review the changes: git diff" -ForegroundColor White
Write-Host "  2. Test your build" -ForegroundColor White
Write-Host "  3. Commit when ready: git add -A && git commit -m 'Sync with upstream GeoSharPlus'" -ForegroundColor White
Write-Host "============================================" -ForegroundColor Cyan
