# Deployment

This document outlines the process for deploying new versions of the ClickUp API Client SDK to NuGet.

## Versioning

The SDK follows [Semantic Versioning (SemVer)](https://semver.org/). Before creating a release, ensure the version number in the `.csproj` files of the library projects (`ClickUp.Api.Client`, `ClickUp.Api.Client.Abstractions`, `ClickUp.Api.Client.Models`) is updated according to the nature of the changes:

- **MAJOR** version for incompatible API changes.
- **MINOR** version for adding functionality in a backward-compatible manner.
- **PATCH** version for backward-compatible bug fixes.

## Release Process

The release process is currently manual and involves the following steps:

1. **Ensure the `main` branch is stable**: All tests should be passing and the code should be in a releasable state.

2. **Update the version number**: As described above, update the version number in the `.csproj` files.

3. **Create a release branch**: Create a new release branch from `main`, e.g., `release/v1.2.3`.

4. **Update the release notes**: Add a new entry to the `RELEASENOTES.md` file, detailing the changes in the new version.

5. **Create and push a git tag**: Tag the release commit with the version number, e.g., `v1.2.3`.

   ```bash
   git tag -a v1.2.3 -m "Release v1.2.3"
   git push origin v1.2.3
   ```

6. **Create the NuGet package**:

   ```bash
   dotnet pack --configuration Release
   ```

   This will create the `.nupkg` files in the `bin/Release` directory of each library project.

7. **Publish the package to NuGet**: You will need a NuGet API key to publish the package.

   ```bash
   dotnet nuget push "path/to/package.nupkg" --api-key YOUR_NUGET_API_KEY --source https://api.nuget.org/v3/index.json
   ```

8. **Merge the release branch**: Merge the release branch back into `main` and then delete the release branch.

## Continuous Integration / Continuous Deployment (CI/CD)

A CI/CD pipeline for automated releases is not yet implemented. The existing GitHub Actions workflow only builds and tests the solution. Future work could involve extending this workflow to automate the packaging and publishing process when a new tag is pushed to the repository.