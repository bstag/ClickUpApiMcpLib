# DocFX with GitHub Pages Setup Guide

This guide explains how to set up DocFX documentation with automatic GitHub Pages deployment for the ClickUp .NET SDK.

## Overview

Our documentation system uses:
- **DocFX** - Static documentation generator for .NET projects
- **GitHub Actions** - Automated build and deployment
- **GitHub Pages** - Free hosting for the documentation site

## Project Structure

```
docs/
├── docfx/
│   ├── api/                    # Generated API documentation
│   ├── articles/               # Hand-written documentation
│   │   ├── getting-started.md
│   │   ├── authentication.md
│   │   └── ...
│   ├── images/                 # Images and assets
│   ├── styles/                 # Custom CSS (optional)
│   ├── docfx.json             # DocFX configuration
│   ├── filterConfig.yml       # API filtering rules
│   ├── index.md               # Homepage
│   └── toc.yml                # Table of contents
└── plans/                     # Development plans (not published)
```

## Configuration Files

### docfx.json

The main configuration file that defines:
- **Metadata extraction** from source code
- **Content sources** (articles, API docs)
- **Build settings** and templates
- **Global metadata** (site title, footer, etc.)

Key sections:
```json
{
  "metadata": [
    {
      "src": ["../../src/**/*.csproj"],
      "dest": "api",
      "filter": "filterConfig.yml"
    }
  ],
  "build": {
    "content": [
      { "files": ["api/**.yml", "api/index.md"] },
      { "files": ["articles/**.md", "toc.yml", "*.md"] }
    ],
    "template": ["default", "modern"],
    "globalMetadata": {
      "_appTitle": "ClickUp .NET SDK Documentation"
    }
  }
}
```

### filterConfig.yml

Controls which APIs are included in documentation:
```yaml
apiRules:
- exclude:
    uidRegex: ^System\.
    type: Namespace
- include:
    uidRegex: ^ClickUp\.Api\.Client
    type: Namespace
```

## GitHub Actions Workflow

The `.github/workflows/docs.yml` file automates:

1. **Build Process**:
   - Checkout code with full git history
   - Setup .NET SDK
   - Install DocFX
   - Build solution to generate XML docs
   - Generate API metadata
   - Build documentation site

2. **Deployment**:
   - Upload artifacts for review
   - Deploy to GitHub Pages (on main branch)

### Workflow Triggers

- **Push to main** - Automatically deploys to production
- **Pull requests** - Builds docs for review (artifacts only)
- **Manual trigger** - Can be run manually from GitHub Actions tab

## Setting Up GitHub Pages

### 1. Repository Settings

1. Go to your repository on GitHub
2. Navigate to **Settings** → **Pages**
3. Under **Source**, select **GitHub Actions**
4. Save the settings

### 2. Required Permissions

The workflow needs these permissions (already configured):
```yaml
permissions:
  contents: read
  pages: write
  id-token: write
```

### 3. Environment Setup

GitHub automatically creates a `github-pages` environment when you enable Pages.

## Local Development

### Prerequisites

```bash
# Install DocFX globally
dotnet tool install -g docfx

# Or install locally in the project
dotnet tool install docfx
```

### Building Documentation Locally

```bash
# Navigate to docs directory
cd docs/docfx

# Generate API metadata (requires built solution)
dotnet build ../../src/ClickUp.Api.sln --configuration Release
docfx metadata docfx.json

# Build the documentation site
docfx build docfx.json

# Serve locally with live reload
docfx serve _site --port 8080
```

The site will be available at `http://localhost:8080`

### Live Preview During Development

```bash
# Watch for changes and auto-rebuild
docfx build docfx.json --serve --port 8080
```

## Customization

### Custom Styling

Create `styles/main.css` for custom styles:
```css
/* Custom styles for the documentation site */
.navbar-brand {
    font-weight: bold;
}

.hero-section {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    padding: 60px 0;
}
```

Reference in `docfx.json`:
```json
{
  "build": {
    "resource": [
      {
        "files": ["styles/**"]
      }
    ]
  }
}
```

### Custom Templates

For advanced customization, you can:
1. Create custom DocFX templates
2. Override specific template files
3. Add custom JavaScript functionality

### Logo and Branding

Add your logo to `images/logo.png` and configure in `docfx.json`:
```json
{
  "globalMetadata": {
    "_appLogoPath": "images/logo.png",
    "_appFaviconPath": "images/favicon.ico"
  }
}
```

## Content Management

### Adding New Articles

1. Create a new `.md` file in `articles/`
2. Add it to `toc.yml`:
   ```yaml
   - name: New Article
     href: articles/new-article.md
   ```

### API Documentation

API documentation is automatically generated from:
- XML documentation comments in source code
- Public classes, methods, and properties
- Filtered by `filterConfig.yml`

### Cross-References

Link between articles and API docs:
```markdown
<!-- Link to another article -->
[Authentication Guide](authentication.md)

<!-- Link to API documentation -->
[ITasksService](../api/ClickUp.Api.Client.Abstractions.Services.ITasksService.html)

<!-- Link with xref (auto-resolved) -->
<xref:ClickUp.Api.Client.Abstractions.Services.ITasksService>
```

## Troubleshooting

### Common Issues

1. **Build Fails**: Check that all referenced projects build successfully
2. **Missing API Docs**: Ensure XML documentation is enabled in project files
3. **Broken Links**: Use relative paths and check file names
4. **GitHub Pages Not Updating**: Check Actions tab for deployment status

### Debugging Locally

```bash
# Verbose output for debugging
docfx build docfx.json --log Verbose

# Check metadata generation
docfx metadata docfx.json --log Verbose
```

### GitHub Actions Debugging

- Check the Actions tab for detailed logs
- Download artifacts to inspect generated content
- Use `workflow_dispatch` to manually trigger builds

## Best Practices

### Documentation Writing

1. **Use clear headings** and consistent structure
2. **Include code examples** for all major features
3. **Cross-reference related content** liberally
4. **Keep examples up-to-date** with the latest API

### Maintenance

1. **Review generated API docs** after major changes
2. **Update articles** when adding new features
3. **Test documentation builds** in pull requests
4. **Monitor GitHub Pages** deployment status

### Performance

1. **Optimize images** before adding to `images/`
2. **Use appropriate filtering** to exclude unnecessary APIs
3. **Consider build time** when adding large amounts of content

## Deployment URL

Once set up, your documentation will be available at:
```
https://your-username.github.io/ClickUpApiMcpLib/
```

The URL structure will be:
- Homepage: `/`
- Articles: `/articles/getting-started.html`
- API Reference: `/api/ClickUp.Api.Client.html`

## Conclusion

This setup provides a professional, automatically-updated documentation site that:
- Generates API documentation from source code
- Hosts comprehensive guides and tutorials
- Deploys automatically on every change
- Provides search functionality
- Maintains version history through git

The documentation will help users understand and effectively use the ClickUp .NET SDK.