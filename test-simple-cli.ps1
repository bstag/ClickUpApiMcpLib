# Simple test to check if CLI is working at all
Write-Host "Testing basic CLI functionality..."

# Test root command help
Write-Host "\nTesting root command help:"
Write-Host "Command: dotnet examples/ClickUp.Api.Client.CLI/bin/Debug/net9.0/clickup-cli.dll --help"
Write-Host "=" * 60
dotnet examples/ClickUp.Api.Client.CLI/bin/Debug/net9.0/clickup-cli.dll --help
Write-Host "Exit code: $LASTEXITCODE"

# Test just the root command
Write-Host "\nTesting root command without arguments:"
Write-Host "Command: dotnet examples/ClickUp.Api.Client.CLI/bin/Debug/net9.0/clickup-cli.dll"
Write-Host "=" * 60
dotnet examples/ClickUp.Api.Client.CLI/bin/Debug/net9.0/clickup-cli.dll
Write-Host "Exit code: $LASTEXITCODE"

# Test list command help
Write-Host "\nTesting list command help:"
Write-Host "Command: dotnet examples/ClickUp.Api.Client.CLI/bin/Debug/net9.0/clickup-cli.dll list --help"
Write-Host "=" * 60
dotnet examples/ClickUp.Api.Client.CLI/bin/Debug/net9.0/clickup-cli.dll list --help
Write-Host "Exit code: $LASTEXITCODE"

Write-Host "\nTest completed."