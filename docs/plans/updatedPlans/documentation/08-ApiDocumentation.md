# Detailed Plan: API Documentation (DocFX)

This document outlines the plan for setting up and generating comprehensive API documentation for the ClickUp API SDK using DocFX.

**Source Documents:**
*   `docs/plans/NEW_OVERALL_PLAN.md` (Phase 3, Step 11)
*   All other `docs/plans/updatedPlans/**/*.md` for conceptual content.

**Location in Codebase:**
*   DocFX Project: `docs/docfx/` (or a similar subdirectory within `docs/`)
*   Generated Documentation Output: `_site` (default DocFX output, will be within `docs/docfx/`) or a top-level `docs/api` for GitHub Pages.

## 1. DocFX Setup and Configuration

1.  **Install DocFX:**
    *   DocFX can be installed as a .NET global tool or used via its executable.
    *   Command: `dotnet tool update -g docfx` (or `install` if first time).

2.  **Initialize DocFX Project:**
    *   Navigate to the `docs/` directory (or create `docs/docfx/`).
    *   Run `docfx init -q` to create a basic `docfx.json` configuration file and default directory structure.

3.  **Configure `docfx.json`:**
    *   **Metadata Section:**
        *   Specify the source projects (`src`) for extracting API documentation from C# code.
            *   `"src": "../src/ClickUp.Api.Client.Abstractions/ClickUp.Api.Client.Abstractions.csproj"`
            *   `"src": "../src/ClickUp.Api.Client.Models/ClickUp.Api.Client.Models.csproj"`
            *   `"src": "../src/ClickUp.Api.Client/ClickUp.Api.Client.csproj"` (if public helper classes are intended for documentation).
        *   `"dest": "api"` (Output directory for generated API metadata files, relative to `docfx.json`).
        *   `"filter": "filterConfig.yml"` (Optional, to exclude specific namespaces/types if needed).
    *   **Build Section:**
        *   `"content": [{ "files": ["api/**.yml", "articles/**.md", "toc.yml", "*.md"] }]` (Specifies content to be processed: API docs, conceptual articles, main TOC, and any root markdown files).
        *   `"resource": [{ "files": ["images/**"] }]` (For any images used in documentation).
        *   `"output": "_site"` (Default output directory for the static website).
        *   `"template": ["default", "modern"]` (Choose a template. "modern" is a good choice, or custom templates).
        *   `"globalMetadata": { "_appTitle": "ClickUp .NET SDK Documentation", "_appFooter": "© YourCompany/Project - Licensed under MIT", ... }`
        *   `"xref": ["https://learn.microsoft.com/en-us/dotnet/api/"]` (To resolve .NET base class library links).

4.  **Create `toc.yml` (Table of Contents) Files:**
    *   **Root `toc.yml` (`docs/docfx/toc.yml`):** Main navigation for the documentation site.
        ```yaml
        - name: Home
          href: index.md
        - name: Introduction
          href: articles/intro.md
        - name: Getting Started
          href: articles/getting-started.md
        - name: Authentication
          href: articles/authentication.md
        - name: API Reference
          href: api/toc.html # Link to the auto-generated API TOC
        - name: Advanced Topics
          items:
            - name: Rate Limiting
              href: articles/rate-limiting.md
            - name: Pagination
              href: articles/pagination.md
            - name: Error Handling
              href: articles/error-handling.md
        # Add more conceptual topics as needed
        ```
    *   **API `toc.yml` (`docs/docfx/api/toc.yml`):** DocFX generates this from the metadata, but you can customize its structure if needed, or let DocFX generate it. Typically, it will group by namespace.

5.  **Create `index.md` (`docs/docfx/index.md`):**
    *   The homepage for the documentation website.
    *   Should provide an overview of the SDK, its purpose, and links to key sections like "Getting Started" and "API Reference."

## 2. XML Documentation Comments

*   **Mandatory for all public types and members:** This is crucial for DocFX to generate meaningful API documentation.
    *   Classes, records, structs, interfaces, enums.
    *   Methods, properties, constructors, enum members, events, fields.
*   **Standard XML Tags:**
    *   `<summary>`: Concise description of the type or member.
    *   `<param name="paramName">`: Description of a method parameter.
    *   `<typeparam name="TParamName">`: Description of a generic type parameter.
    *   `<returns>`: Description of a method's return value. For `void` or `Task`, indicate completion.
    *   `<exception cref="ExceptionType">`: Describes exceptions that can be thrown, with conditions.
    *   `<remarks>`: Additional details, usage notes, or more in-depth explanation.
    *   `<example>`: Code examples demonstrating usage.
    *   `<see cref="OtherTypeOrMember"/>`: For cross-referencing other types/members.
    *   `<seealso cref="AnotherTypeOrMember"/>`: Similar to `<see>`, often used for related topics.
    *   `<code>`: For inline code snippets.
*   **Enable XML Documentation File Generation in `.csproj` files:**
    For `ClickUp.Api.Client.Abstractions.csproj`, `ClickUp.Api.Client.Models.csproj`, and `ClickUp.Api.Client.csproj`:
    ```xml
    <PropertyGroup>
      <GenerateDocumentationFile>true</GenerateDocumentationFile>
      <!-- Optional: Set NoWarn for CS1591 if you want to avoid build warnings for missing XML comments during early development -->
      <!-- <NoWarn>$(NoWarn);CS1591</NoWarn> -->
    </PropertyGroup>
    ```
*   **Review and Enhance Existing Comments:** Go through all public APIs and ensure comments are comprehensive, accurate, and follow best practices.

## 3. Conceptual Documentation (Articles)

*   Location: `docs/docfx/articles/`
*   Create Markdown (`.md`) files for conceptual topics. These will be referenced in `toc.yml`.
*   **Key Topics to Cover:**
    1.  **`intro.md`**: Brief overview of the SDK, its purpose, and key features.
    2.  **`getting-started.md`**:
        *   Installation instructions (NuGet package).
        *   Basic setup for DI: How to use `services.AddClickUpApiClient(...)`.
        *   A simple example of initializing a service and making a call (e.g., getting user info or a list of tasks).
    3.  **`authentication.md`**:
        *   How to configure and use Personal API Tokens.
        *   Guidance on OAuth 2.0: Explain that the SDK supports using OAuth tokens but the consuming application must handle the OAuth flow and token management. Show how to provide the token to the SDK.
    4.  **`error-handling.md`**:
        *   Explanation of the `ClickUpApiException` hierarchy.
        *   How to catch specific exceptions (e.g., `ClickUpApiNotFoundException`, `ClickUpApiRateLimitException`).
        *   Information on accessing `HttpStatus`, `ApiErrorCode`, etc.
    5.  **`pagination.md`**:
        *   How to use the `IAsyncEnumerable<T>` helper methods for automatic pagination.
        *   Example using `await foreach`.
        *   Mentioning the underlying page-based methods for manual control if needed.
    6.  **`rate-limiting.md`**:
        *   Brief explanation of ClickUp API rate limits (if known, or link to official ClickUp docs).
        *   How the SDK's Polly policies (retry, circuit breaker) help manage this.
        *   How to interpret `ClickUpApiRateLimitException` and `RetryAfter` properties.
    7.  **(Optional) Advanced Topics:**
        *   Working with Custom Fields.
        *   Using Webhooks (if SDK provides helpers).
        *   Semantic Kernel integration.
*   Use Markdown features for formatting, code blocks, links, etc.
*   Include code snippets to illustrate usage.

## 4. Building and Serving Documentation

*   **Build Command:** `docfx build docs/docfx/docfx.json`
*   **Serve Command (for local preview):** `docfx serve docs/docfx/_site`
*   Iteratively build and review the documentation locally to ensure correctness and good presentation.

## 5. GitHub Actions Workflow for Documentation Deployment

*   Create a GitHub Actions workflow (e.g., `.github/workflows/docfx.yml`).
*   **Trigger:** On push to `main` branch or on release.
*   **Steps:**
    1.  Checkout code.
    2.  Set up .NET SDK.
    3.  Install/Restore DocFX tool.
    4.  Run `docfx metadata docs/docfx/docfx.json` (if you want to commit metadata separately, optional).
    5.  Run `docfx build docs/docfx/docfx.json`.
    6.  Deploy the contents of `docs/docfx/_site` to GitHub Pages.
        *   Use an action like `JamesIves/github-pages-deploy-action@v4` or similar.

```yaml
# .github/workflows/docfx.yml (Conceptual)
name: Deploy Documentation

on:
  push:
    branches:
      - main # Or your release branch
  workflow_dispatch:

jobs:
  deploy-docfx:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x' # Match your project's .NET version

    - name: Install DocFX
      run: dotnet tool update -g docfx --version "2.75.2" # Pin to a specific version

    - name: Build Documentation
      run: docfx docs/docfx/docfx.json

    - name: Deploy to GitHub Pages
      uses: JamesIves/github-pages-deploy-action@v4
      with:
        branch: gh-pages # The branch the action should deploy to.
        folder: docs/docfx/_site # The folder the action should deploy.
        token: ${{ secrets.GITHUB_TOKEN }} # Default GITHUB_TOKEN
```

## 6. Final Review
*   Thoroughly review all generated documentation for accuracy, completeness, and clarity.
*   Check for broken links and formatting issues.
*   Ensure code examples are correct and easy to follow.

This plan provides a comprehensive approach to creating high-quality documentation for the ClickUp API SDK.
```
