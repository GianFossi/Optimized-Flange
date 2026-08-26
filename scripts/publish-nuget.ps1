param(
    [string]$Configuration = "Release",
    [string]$Source = "https://api.nuget.org/v3/index.json",
    [string]$ApiKey = $env:NUGET_API_KEY,
    [string]$PackageRoot = "",
    [switch]$SkipTests,
    [switch]$SkipPack,
    [switch]$Publish,
    [switch]$IncludeSymbols
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot "OptimizedFlange.sln"

if ([string]::IsNullOrWhiteSpace($PackageRoot)) {
    $PackageRoot = $repoRoot
}

function Invoke-Step {
    param(
        [string]$Name,
        [string]$Command,
        [string[]]$Arguments
    )

    Write-Host ""
    Write-Host "== $Name =="
    & $Command @Arguments

    if ($LASTEXITCODE -ne 0) {
        throw "$Name failed with exit code $LASTEXITCODE."
    }
}

Invoke-Step "Restore" "dotnet" @("restore", $solution)
Invoke-Step "Build" "dotnet" @("build", $solution, "--configuration", $Configuration, "--no-restore")

if (-not $SkipTests) {
    Invoke-Step "Test" "dotnet" @("test", $solution, "--configuration", $Configuration, "--no-build")
}

if (-not $SkipPack) {
    $packArgs = @("pack", $solution, "--configuration", $Configuration, "--no-build")

    if ($IncludeSymbols) {
        $packArgs += @("--include-symbols", "-p:SymbolPackageFormat=snupkg")
    }

    Invoke-Step "Pack" "dotnet" $packArgs
}

$packages =
    Get-ChildItem -Path $PackageRoot -Recurse -File -Filter "*.nupkg" |
    Where-Object {
        $_.FullName -like "*\bin\$Configuration\*" -and
        $_.Name -notlike "*.symbols.nupkg"
    } |
    Sort-Object FullName

if ($packages.Count -eq 0) {
    throw "No .nupkg files found under '$PackageRoot' for configuration '$Configuration'."
}

Write-Host ""
Write-Host "Packages:"
$packages | ForEach-Object { Write-Host " - $($_.FullName)" }

if (-not $Publish) {
    Write-Host ""
    Write-Host "Dry run only. Re-run with -Publish to push packages."
    exit 0
}

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    throw "Missing NuGet API key. Set NUGET_API_KEY or pass -ApiKey."
}

foreach ($package in $packages) {
    Invoke-Step "Push $($package.Name)" "dotnet" @(
        "nuget",
        "push",
        $package.FullName,
        "--api-key",
        $ApiKey,
        "--source",
        $Source,
        "--skip-duplicate"
    )
}
