# NuGet Packages

The ClickUp .NET SDK is distributed as three NuGet packages under the **Stagware** company namespace:

## Available Packages

### 1. Stagware.ClickUp.Api.Client.Models
**Core models and DTOs for the ClickUp API**
- Contains all request/response models
- Entity definitions
- Custom exceptions
- Serialization converters

### 2. Stagware.ClickUp.Api.Client.Abstractions  
**Interfaces and abstractions**
- Service interfaces for dependency injection
- Configuration options
- HTTP connection abstractions
- Extensibility contracts

### 3. Stagware.ClickUp.Api.Client
**Main client library**
- Complete ClickUp API client implementation
- Fluent API support
- Built-in resilience with Polly
- Dependency injection integration

## Installation

### Install the main package (recommended)
```bash
dotnet add package Stagware.ClickUp.Api.Client
```

This will automatically include the Models and Abstractions packages as dependencies.

### Install individual packages
```bash
# For just the models (if building your own client)
dotnet add package Stagware.ClickUp.Api.Client.Models

# For abstractions only (if implementing custom services)
dotnet add package Stagware.ClickUp.Api.Client.Abstractions
```

## Package Features

- **Automatic Versioning**: Packages are versioned using GitVersion based on Git commits
- **Symbol Packages**: Debug symbols (.snupkg) are included for better debugging experience
- **Source Link**: Full source code navigation in debuggers
- **Documentation**: XML documentation included in all packages
- **Strong Naming**: All assemblies are signed for enterprise compatibility

## Local Development

### Building Packages Locally

Use the provided build script to create packages locally:

```bash
# Make the script executable (Linux/macOS)
chmod +x build-packages.sh

# Run the build script
./build-packages.sh
```

This will create packages in the `./nuget-packages` directory.

### Using Local Packages

To test with locally built packages:

```bash
# Add local package source
dotnet add package Stagware.ClickUp.Api.Client --source ./nuget-packages

# Or create a nuget.config file
dotnet new nugetconfig
# Edit nuget.config to add local source
```

### Adding Package Icon

To include an icon in the packages, add an `icon.png` file to the root directory of the repository. The build process will automatically include it in all packages.

## CI/CD Integration

The GitHub Actions workflow automatically:

1. **Builds** all packages on every push/PR
2. **Versions** packages using GitVersion
3. **Tests** package installation
4. **Validates** package integrity
5. **Uploads** packages as build artifacts

### Workflow Triggers

- **Push to main/develop**: Creates packages with appropriate version tags
- **Pull Requests**: Creates packages for testing
- **Releases**: Creates final release packages
- **Manual**: Can be triggered manually via GitHub Actions

## Package Metadata

All packages include comprehensive metadata:

- **Company**: Stagware
- **Authors**: Stagware Development Team
- **License**: MIT
- **Repository**: https://github.com/stagware/ClickUpApiMcpLib
- **Tags**: clickup, api, sdk, dotnet, csharp, rest, client, productivity, project-management
- **Target Framework**: .NET 9.0

## Versioning Strategy

The project uses [GitVersion](https://gitversion.net/) for automatic semantic versioning, starting from **0.1.0** to indicate pre-release status:

- **main branch**: Patch increments (0.1.0, 0.1.1, 0.1.2, etc.)
- **develop branch**: Alpha pre-releases (0.2.0-alpha.1, etc.)
- **feature branches**: Alpha pre-releases with branch name
- **hotfix branches**: Beta pre-releases
- **release tags**: Final release versions (1.0.0 when ready for production)

The project will remain in 0.x versions until the API is considered stable and ready for the first major release (1.0.0).

## Support

For package-related issues:

1. Check the [GitHub Issues](https://github.com/stagware/ClickUpApiMcpLib/issues)
2. Review the [Release Notes](https://github.com/stagware/ClickUpApiMcpLib/releases)
3. Consult the [Documentation](docs/)

## Contributing

When contributing changes that affect the packages:

1. Ensure all three packages build successfully
2. Run the local build script to test package creation
3. Update this documentation if package structure changes
4. Follow semantic versioning guidelines for breaking changes