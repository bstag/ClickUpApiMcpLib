# ClickUp .NET SDK Documentation Build Script (PowerShell)
# This script builds the documentation locally for development and testing

param(
    [switch]$Serve
)

$ErrorActionPreference = "Stop"

Write-Host "🔧 Building ClickUp .NET SDK Documentation..." -ForegroundColor Cyan

# Check if DocFX is installed
try {
    docfx --version | Out-Null
    Write-Host "✅ DocFX is already installed" -ForegroundColor Green
} catch {
    Write-Host "❌ DocFX is not installed. Installing..." -ForegroundColor Yellow
    dotnet tool install -g docfx
    Write-Host "✅ DocFX installed successfully" -ForegroundColor Green
}

# Build the solution first to generate XML documentation
Write-Host "🏗️  Building solution to generate XML documentation..." -ForegroundColor Cyan
try {
    dotnet build src/ClickUp.Api.sln --configuration Release --nologo
    Write-Host "✅ Solution built successfully" -ForegroundColor Green
} catch {
    Write-Host "❌ Solution build failed. Please fix build errors first." -ForegroundColor Red
    exit 1
}

# Navigate to DocFX directory
Push-Location docs/docfx

try {
    # Generate API metadata
    Write-Host "📚 Generating API metadata..." -ForegroundColor Cyan
    docfx metadata docfx.json
    Write-Host "✅ API metadata generated successfully" -ForegroundColor Green

    # Build the documentation site
    Write-Host "🔨 Building documentation site..." -ForegroundColor Cyan
    docfx build docfx.json
    Write-Host "✅ Documentation built successfully" -ForegroundColor Green

    # Check if --serve flag is provided
    if ($Serve) {
        Write-Host "🌐 Starting local server at http://localhost:8080" -ForegroundColor Cyan
        Write-Host "📝 Press Ctrl+C to stop the server" -ForegroundColor Yellow
        docfx serve _site --port 8080
    } else {
        Write-Host "🎉 Documentation build complete!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📁 Output location: docs/docfx/_site/" -ForegroundColor White
        Write-Host "🌐 To serve locally, run: docfx serve docs/docfx/_site --port 8080" -ForegroundColor White
        Write-Host "🔄 To build and serve in one command, run: .\build-docs.ps1 -Serve" -ForegroundColor White
    }
} catch {
    Write-Host "❌ Documentation build failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    Pop-Location
}