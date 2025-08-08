# Suggested Commands for Windows Development

## Core Development Commands

### Build Commands
```powershell
# Build entire solution
dotnet build src/ClickUp.Api.sln --nologo

# Build in Release configuration
dotnet build src/ClickUp.Api.sln --configuration Release --nologo

# Clean build artifacts
dotnet clean src/ClickUp.Api.sln
```

### Alternative Build Script
```bash
# Use the provided build script (if in WSL/Git Bash)
./build_solution.sh
```

### Test Commands
```powershell
# Run unit tests
dotnet test src/ClickUp.Api.Client.Tests

# Run integration tests (requires CLICKUP_TOKEN environment variable)
dotnet test src/ClickUp.Api.Client.IntegrationTests

# Run all tests
dotnet test src/ClickUp.Api.sln

# Run tests with coverage
dotnet test src/ClickUp.Api.sln --collect:"XPlat Code Coverage"

# Run tests in Release configuration
dotnet test src/ClickUp.Api.sln --configuration Release
```

### Code Quality Commands
```powershell
# Format code according to .editorconfig
dotnet format src/ClickUp.Api.sln

# Format with verification (dry run)
dotnet format src/ClickUp.Api.sln --verify-no-changes

# Restore NuGet packages
dotnet restore src/ClickUp.Api.sln
```

### Package Management
```powershell
# Create NuGet packages (Windows PowerShell)
.\build-packages.ps1

# Create NuGet packages (Cross-platform)
.\build-packages.sh

# Install a local package
dotnet add package Stagware.ClickUp.Api.Client --source ./nuget-packages
```

## Example Applications

### Console Demo
```powershell
cd examples/ClickUp.Api.Client.Console
dotnet run
```

### Fluent API Example
```powershell
cd examples/ClickUp.Api.Client.Fluent.Console  
dotnet run
```

### CLI Tool
```powershell
cd examples/ClickUp.Api.Client.CLI

# Setup authentication
dotnet run -- config set-api-key YOUR_API_KEY

# List workspaces
dotnet run -- auth workspaces list

# Get current user
dotnet run -- auth user get

# List spaces in a workspace
dotnet run -- space list WORKSPACE_ID

# Debug mode (shows HTTP requests/responses)
dotnet run -- task list LIST_ID --debug
```

### Worker Service Example
```powershell
cd examples/ClickUp.Api.Client.Worker
dotnet run
```

## Documentation Commands
```powershell
# Build documentation (if DocFX is installed)
.\build-docs.ps1

# Alternative documentation build
.\build-docs.sh
```

## Git Commands (Windows)
```powershell
# Check status
git status

# View recent commits  
git log --oneline -10

# Create feature branch
git checkout -b feature/your-feature-name

# Stage changes
git add .

# Commit with conventional commit message
git commit -m "feat: add new feature description"

# Push branch
git push origin feature/your-feature-name
```

## Environment Setup
```powershell
# Set ClickUp API token for integration tests and examples
$env:CLICKUP_TOKEN = "your_token_here"

# Or set permanently for current user
[Environment]::SetEnvironmentVariable("CLICKUP_TOKEN", "your_token_here", "User")
```

## Utility Commands (Windows)
```powershell
# List files and directories
dir
# or
ls  # if using PowerShell 7+

# Change directory
cd path\to\directory

# Find files
dir /s *.cs  # Find all C# files recursively

# Search in files (using PowerShell)
Select-String -Pattern "search_term" -Path "*.cs" -Recurse

# Using git bash/WSL equivalents
find . -name "*.cs" | grep "pattern"
grep -r "search_term" --include="*.cs" .
```

## Development Workflow Commands
```powershell
# Complete development workflow
dotnet restore src/ClickUp.Api.sln
dotnet format src/ClickUp.Api.sln  
dotnet build src/ClickUp.Api.sln --nologo
dotnet test src/ClickUp.Api.sln
```