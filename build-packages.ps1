# Build and package ClickUp API Client NuGet packages
# This script builds all three NuGet packages for the ClickUp API Client

Write-Host "🔧 Building ClickUp API Client NuGet packages..." -ForegroundColor Cyan

# Clean previous builds
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Yellow
Set-Location src
dotnet clean ClickUp.Api.sln --configuration Release
Set-Location ..

# Create output directory
$OutputDir = "./nuget-packages"
if (Test-Path $OutputDir) {
    Remove-Item $OutputDir -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputDir | Out-Null

Write-Host "📦 Building solution..." -ForegroundColor Green
Set-Location src
dotnet build ClickUp.Api.sln --configuration Release

Write-Host "📦 Creating NuGet packages..." -ForegroundColor Green

# Set package icon to empty if icon.png doesn't exist
$IconParam = ""
if (-not (Test-Path "../icon.png")) {
    Write-Host "⚠️  Warning: icon.png not found, packages will be created without icon" -ForegroundColor Yellow
    $IconParam = "/p:PackageIcon="
}

# Pack each project
$projects = @(
    "ClickUp.Api.Client.Models/ClickUp.Api.Client.Models.csproj",
    "ClickUp.Api.Client.Abstractions/ClickUp.Api.Client.Abstractions.csproj",
    "ClickUp.Api.Client/ClickUp.Api.Client.csproj"
)

foreach ($project in $projects) {
    Write-Host "📦 Packing $project..." -ForegroundColor Blue
    if ($IconParam) {
        dotnet pack $project --configuration Release --no-build --output "../$OutputDir" $IconParam
    } else {
        dotnet pack $project --configuration Release --no-build --output "../$OutputDir"
    }
}

Set-Location ..

Write-Host "✅ Package creation completed!" -ForegroundColor Green
Write-Host "📁 Packages created in: $OutputDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Created packages:" -ForegroundColor White
Get-ChildItem "$OutputDir/*.nupkg" | ForEach-Object { Write-Host "  $($_.Name)" -ForegroundColor Gray }

Write-Host ""
Write-Host "🎯 To use these packages locally:" -ForegroundColor Cyan
Write-Host "   dotnet add package Stagware.ClickUp.Api.Client --source ./nuget-packages" -ForegroundColor White
Write-Host ""
Write-Host "📝 Note: Add icon.png to the root directory to include an icon in the packages" -ForegroundColor Yellow