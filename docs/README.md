# ClickUp .NET SDK Documentation

This directory contains the documentation for the ClickUp .NET SDK.

## Structure

- **`docfx/`** - DocFX documentation site source
  - **`articles/`** - Hand-written documentation articles
  - **`api/`** - Auto-generated API documentation (created during build)
  - **`images/`** - Images and assets
  - **`docfx.json`** - DocFX configuration
  - **`index.md`** - Documentation homepage
- **`plans/`** - Development plans and design documents (not published)

## Building Documentation Locally

### Prerequisites

```bash
# Install DocFX
dotnet tool install -g docfx
```

### Build Steps

```bash
# 1. Build the solution to generate XML documentation
dotnet build ../src/ClickUp.Api.sln --configuration Release

# 2. Navigate to DocFX directory
cd docfx

# 3. Generate API metadata
docfx metadata docfx.json

# 4. Build the documentation site
docfx build docfx.json

# 5. Serve locally (optional)
docfx serve _site --port 8080
```

The documentation will be available at `http://localhost:8080`

## Live Documentation Site

The documentation is automatically built and deployed to GitHub Pages:

**Production Site**: `https://your-username.github.io/ClickUpApiMcpLib/`

## Contributing to Documentation

### Adding New Articles

1. Create a new `.md` file in `docfx/articles/`
2. Add it to `docfx/toc.yml`
3. Follow the existing style and structure

### Updating API Documentation

API documentation is automatically generated from XML comments in the source code. To improve it:

1. Add or enhance XML documentation comments in the source code
2. Use standard XML doc tags: `<summary>`, `<param>`, `<returns>`, `<example>`, etc.
3. The changes will appear in the next documentation build

### Guidelines

- Use clear, concise language
- Include code examples for complex topics
- Cross-reference related articles and API documentation
- Test your changes locally before submitting

## Deployment

Documentation is automatically deployed via GitHub Actions when changes are pushed to the main branch. See `.github/workflows/docs.yml` for the deployment configuration.

## Troubleshooting

### Common Issues

1. **Build fails**: Ensure the solution builds successfully first
2. **Missing API docs**: Check that XML documentation is enabled in project files
3. **Broken links**: Use relative paths and verify file names

### Getting Help

- Check the [DocFX Setup Guide](docfx/articles/docfx-setup.md) for detailed instructions
- Review the GitHub Actions logs for deployment issues
- Open an issue if you encounter problems