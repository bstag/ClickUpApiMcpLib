# Test script to verify CLI help functionality
Write-Host "Testing CLI help functionality..."

# Build the CLI project
Write-Host "Building CLI project..."
dotnet build examples/ClickUp.Api.Client.CLI/ClickUp.Api.Client.CLI.csproj --no-restore

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful. Testing help output..."
    
    # Test the problematic command
    Write-Host "\nTesting: dotnet run --project examples/ClickUp.Api.Client.CLI -- list --format json"
    Write-Host "Expected: Help message should appear"
    Write-Host "Actual output:"
    Write-Host "=" * 50
    
    dotnet run --project examples/ClickUp.Api.Client.CLI --no-build -- list --format json
    
    Write-Host "=" * 50
    Write-Host "Exit code: $LASTEXITCODE"
    
    # Test with uppercase format
    Write-Host "\nTesting: dotnet run --project examples/ClickUp.Api.Client.CLI -- list --format Json"
    Write-Host "Expected: Same help message should appear"
    Write-Host "Actual output:"
    Write-Host "=" * 50
    
    dotnet run --project examples/ClickUp.Api.Client.CLI --no-build -- list --format Json
    
    Write-Host "=" * 50
    Write-Host "Exit code: $LASTEXITCODE"
} else {
    Write-Host "Build failed with exit code: $LASTEXITCODE"
}

Write-Host "\nTest completed."