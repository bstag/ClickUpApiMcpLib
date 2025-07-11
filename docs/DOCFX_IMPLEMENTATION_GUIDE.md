# DocFX with GitHub Pages Implementation Guide

This guide provides step-by-step instructions for implementing DocFX documentation with GitHub Pages for the ClickUp .NET SDK.

## 🎯 What We've Implemented

### ✅ Complete DocFX Setup
- **DocFX Configuration** (`docs/docfx/docfx.json`) - Comprehensive configuration for API and article generation
- **Filter Configuration** (`docs/docfx/filterConfig.yml`) - Controls which APIs are documented
- **Enhanced Homepage** (`docs/docfx/index.md`) - Professional landing page with feature overview
- **Article Structure** - Organized documentation articles with clear navigation

### ✅ GitHub Actions Automation
- **Automated Builds** (`.github/workflows/docs.yml`) - Builds documentation on every push
- **GitHub Pages Deployment** - Automatic deployment to GitHub Pages
- **Pull Request Previews** - Builds documentation for review on PRs
- **Manual Triggers** - Can be triggered manually when needed

### ✅ Development Tools
- **Build Scripts** (`build-docs.sh` and `build-docs.ps1`) - Local development helpers
- **Documentation README** (`docs/README.md`) - Clear instructions for contributors

## 🚀 Getting Started

### 1. Enable GitHub Pages

1. **Go to Repository Settings**
   - Navigate to your GitHub repository
   - Click on **Settings** tab
   - Scroll down to **Pages** section

2. **Configure Source**
   - Under **Source**, select **GitHub Actions**
   - Save the configuration

3. **Verify Permissions**
   - The workflow already includes required permissions:
     ```yaml
     permissions:
       contents: read
       pages: write
       id-token: write
     ```

### 2. Update Repository URLs

Update the repository URLs in the configuration files:

**In `docs/docfx/docfx.json`:**
```json
{
  "globalMetadata": {
    "_gitContribute": {
      "repo": "https://github.com/YOUR-USERNAME/ClickUpApiMcpLib",
      "branch": "main"
    }
  }
}
```

**In `docs/docfx/index.md`:**
```markdown
- [Contributing Guide](https://github.com/YOUR-USERNAME/ClickUpApiMcpLib/blob/main/CONTRIBUTING.md)
- [LICENSE](https://github.com/YOUR-USERNAME/ClickUpApiMcpLib/blob/main/LICENSE)
```

### 3. Test Local Build

```bash
# Make the script executable (Linux/Mac)
chmod +x build-docs.sh

# Build and serve locally
./build-docs.sh --serve

# Or on Windows
.\build-docs.ps1 -Serve
```

Visit `http://localhost:8080` to preview the documentation.

### 4. Trigger First Deployment

1. **Commit and Push Changes**
   ```bash
   git add .
   git commit -m "Add DocFX documentation with GitHub Pages"
   git push origin main
   ```

2. **Monitor GitHub Actions**
   - Go to the **Actions** tab in your repository
   - Watch the "Build and Deploy Documentation" workflow
   - Check for any errors in the build process

3. **Access Your Documentation**
   - Once deployed, visit: `https://YOUR-USERNAME.github.io/ClickUpApiMcpLib/`
   - It may take a few minutes for the site to be available

## 📁 File Structure Overview

```
├── .github/workflows/docs.yml          # GitHub Actions workflow
├── docs/
│   ├── docfx/
│   │   ├── api/                         # Generated API docs (auto-created)
│   │   ├── articles/                    # Hand-written documentation
│   │   │   ├── getting-started.md       # Installation and setup guide
│   │   │   ├── authentication.md        # Authentication documentation
│   │   │   ├── error-handling.md        # Error handling guide
│   │   │   ├── pagination.md            # Pagination documentation
│   │   │   ├── rate-limiting.md         # Rate limiting and resilience
│   │   │   ├── webhooks.md              # Webhooks documentation
│   │   │   ├── semantic-kernel-integration.md # AI integration
│   │   │   ├── workflow.md              # Documentation workflow
│   │   │   └── docfx-setup.md           # This setup guide
│   │   ├── images/                      # Images and assets
│   │   ├── styles/                      # Custom CSS (optional)
│   │   ├── docfx.json                   # Main DocFX configuration
│   │   ├── filterConfig.yml             # API filtering rules
│   │   ├── index.md                     # Documentation homepage
│   │   └── toc.yml                      # Table of contents
│   ├── plans/                           # Development plans (not published)
│   ├── README.md                        # Documentation overview
│   └── DOCFX_IMPLEMENTATION_GUIDE.md    # This file
├── build-docs.sh                        # Linux/Mac build script
└── build-docs.ps1                       # Windows PowerShell build script
```

## 🔧 Configuration Details

### DocFX Configuration (`docfx.json`)

Key sections explained:

1. **Metadata Section** - Extracts API documentation from source code
   ```json
   "metadata": [{
     "src": ["../../src/**/*.csproj"],
     "dest": "api",
     "filter": "filterConfig.yml"
   }]
   ```

2. **Build Section** - Defines content sources and build settings
   ```json
   "build": {
     "content": [
       { "files": ["api/**.yml", "api/index.md"] },
       { "files": ["articles/**.md", "toc.yml", "*.md"] }
     ],
     "template": ["default", "modern"]
   }
   ```

3. **Global Metadata** - Site-wide settings
   ```json
   "globalMetadata": {
     "_appTitle": "ClickUp .NET SDK Documentation",
     "_enableSearch": true,
     "_gitContribute": { "repo": "..." }
   }
   ```

### Filter Configuration (`filterConfig.yml`)

Controls API visibility:
```yaml
apiRules:
- exclude:
    uidRegex: ^System\.          # Exclude System namespaces
    type: Namespace
- include:
    uidRegex: ^ClickUp\.Api\.Client  # Include our namespaces
    type: Namespace
```

### GitHub Actions Workflow (`.github/workflows/docs.yml`)

The workflow:
1. **Triggers** on pushes to main, PRs, and manual dispatch
2. **Builds** the .NET solution to generate XML docs
3. **Generates** API metadata with DocFX
4. **Builds** the documentation site
5. **Deploys** to GitHub Pages (main branch only)
6. **Uploads** artifacts for PR previews

## 📝 Content Management

### Adding New Articles

1. **Create the Article**
   ```bash
   # Create new markdown file
   touch docs/docfx/articles/new-feature.md
   ```

2. **Add to Table of Contents**
   ```yaml
   # In docs/docfx/toc.yml
   - name: New Feature Guide
     href: articles/new-feature.md
   ```

3. **Write Content**
   ```markdown
   # New Feature Guide
   
   This guide explains how to use the new feature.
   
   ## Overview
   ...
   ```

### Improving API Documentation

API documentation is generated from XML comments in source code:

```csharp
/// <summary>
/// Gets tasks from the specified list with optional filtering.
/// </summary>
/// <param name="listId">The unique identifier of the list.</param>
/// <param name="configureParameters">Optional configuration for filtering and pagination.</param>
/// <param name="cancellationToken">Cancellation token for the operation.</param>
/// <returns>A paged result containing the tasks.</returns>
/// <example>
/// <code>
/// var tasks = await tasksService.GetTasksAsync("list123", parameters => {
///     parameters.Page = 0;
///     parameters.Limit = 50;
/// });
/// </code>
/// </example>
public async Task<IPagedResult<CuTask>> GetTasksAsync(
    string listId,
    Action<GetTasksRequestParameters>? configureParameters = null,
    CancellationToken cancellationToken = default)
```

### Cross-Referencing

Link between articles and API documentation:

```markdown
<!-- Link to another article -->
[Authentication Guide](authentication.md)

<!-- Link to API documentation -->
[ITasksService](../api/ClickUp.Api.Client.Abstractions.Services.ITasksService.html)

<!-- Auto-resolved cross-reference -->
<xref:ClickUp.Api.Client.Abstractions.Services.ITasksService>
```

## 🎨 Customization Options

### Custom Styling

Create `docs/docfx/styles/main.css`:
```css
/* Custom branding */
.navbar-brand {
    font-weight: bold;
    color: #7b68ee !important;
}

/* Custom hero section */
.hero-section {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    padding: 60px 0;
    margin-bottom: 40px;
}

/* Code block styling */
pre {
    background-color: #f8f9fa;
    border-left: 4px solid #7b68ee;
}
```

### Logo and Branding

1. **Add Logo**
   ```bash
   # Add your logo to images directory
   cp your-logo.png docs/docfx/images/logo.png
   cp your-favicon.ico docs/docfx/images/favicon.ico
   ```

2. **Update Configuration**
   ```json
   {
     "globalMetadata": {
       "_appLogoPath": "images/logo.png",
       "_appFaviconPath": "images/favicon.ico"
     }
   }
   ```

### Custom Templates

For advanced customization:
1. Create custom DocFX templates
2. Override specific template partials
3. Add custom JavaScript functionality

## 🔍 Troubleshooting

### Common Issues

1. **Build Fails in GitHub Actions**
   - Check that the solution builds successfully
   - Verify all project references are correct
   - Ensure XML documentation is enabled in all projects

2. **Missing API Documentation**
   - Verify `GenerateDocumentationFile` is `true` in `.csproj` files
   - Check that classes/methods are `public`
   - Ensure XML comments are properly formatted

3. **GitHub Pages Not Updating**
   - Check the Actions tab for deployment status
   - Verify GitHub Pages is configured to use GitHub Actions
   - Wait a few minutes for CDN cache to update

4. **Broken Links**
   - Use relative paths for internal links
   - Check file names and paths are correct
   - Test links in local build

### Debugging Steps

1. **Local Build**
   ```bash
   # Build with verbose logging
   cd docs/docfx
   docfx build docfx.json --log Verbose
   ```

2. **Check Generated Files**
   ```bash
   # Inspect generated API metadata
   ls -la docs/docfx/api/
   
   # Check build output
   ls -la docs/docfx/_site/
   ```

3. **GitHub Actions Logs**
   - Review detailed logs in the Actions tab
   - Download artifacts to inspect generated content
   - Check for specific error messages

## 🚀 Advanced Features

### Search Functionality

Search is enabled by default with:
```json
{
  "globalMetadata": {
    "_enableSearch": true
  },
  "postProcessors": ["ExtractSearchIndex"]
}
```

### Version Management

For multiple versions:
1. Create version-specific branches
2. Deploy each version to a subdirectory
3. Add version selector to the site

### Analytics Integration

Add Google Analytics or other tracking:
```json
{
  "globalMetadata": {
    "_googleAnalyticsTagId": "GA_TRACKING_ID"
  }
}
```

## 📊 Monitoring and Maintenance

### Regular Tasks

1. **Review Generated Docs** after major API changes
2. **Update Articles** when adding new features
3. **Check Links** periodically for broken references
4. **Monitor Build Times** and optimize if needed

### Performance Optimization

1. **Optimize Images** before adding to `images/`
2. **Use Appropriate Filtering** to exclude unnecessary APIs
3. **Consider Build Caching** for large projects

## 🎉 Success Metrics

After implementation, you'll have:

- ✅ **Professional Documentation Site** hosted on GitHub Pages
- ✅ **Automatic Updates** on every code change
- ✅ **Search Functionality** for easy navigation
- ✅ **Mobile-Responsive Design** that works on all devices
- ✅ **API Documentation** generated from source code
- ✅ **Cross-Referenced Content** linking articles and API docs
- ✅ **Version Control** for documentation changes
- ✅ **Contributor-Friendly** setup for easy maintenance

## 🔗 Useful Links

- [DocFX Official Documentation](https://dotnet.github.io/docfx/)
- [GitHub Pages Documentation](https://docs.github.com/en/pages)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Markdown Guide](https://www.markdownguide.org/)

## 📞 Support

If you encounter issues:
1. Check this guide and the troubleshooting section
2. Review the DocFX documentation
3. Check GitHub Actions logs for specific errors
4. Open an issue in the repository for help

---

**Your documentation site will be available at:**
`https://YOUR-USERNAME.github.io/ClickUpApiMcpLib/`

Happy documenting! 📚✨