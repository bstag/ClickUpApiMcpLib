# Detailed Plan: API Documentation (DocFX)

This document outlines the plan for setting up and generating comprehensive API documentation for the ClickUp API SDK using DocFX.

**Source Documents:**
*   [`docs/plans/NEW_OVERALL_PLAN.md`](../NEW_OVERALL_PLAN.md) (Phase 3, Step 11)
*   All other `docs/plans/updatedPlans/**/*.md` for conceptual content.

**Location in Codebase:**
*   DocFX Project: `docs/docfx/` (or a similar subdirectory within `docs/`) - **Not yet created.**
*   Generated Documentation Output: `_site` (default DocFX output, will be within `docs/docfx/`) or a top-level `docs/api` for GitHub Pages.

## 1. DocFX Setup and Configuration

- [ ] **1. Install DocFX:**
    - [ ] DocFX global tool needs to be installed/updated. `dotnet tool update -g docfx`.

- [ ] **2. Initialize DocFX Project:**
    - [ ] Navigate to `docs/` and run `docfx init -q` (or create `docs/docfx/` and run it there).

- [ ] **3. Configure `docfx.json`:**
    - [ ] **Metadata Section:**
        - [ ] Specify source projects: `ClickUp.Api.Client.Abstractions.csproj`, `ClickUp.Api.Client.Models.csproj`, `ClickUp.Api.Client.csproj`.
        - [ ] Set `"dest": "api"`.
        - [ ] Consider `"filter": "filterConfig.yml"` if needed.
    - [ ] **Build Section:**
        - [ ] Configure `"content"`, `"resource"`, `"output"`, `"template"`.
        - [ ] Set `"globalMetadata"` (e.g., `_appTitle`).
        - [ ] Add .NET BCL `"xref"`.

- [ ] **4. Create `toc.yml` (Table of Contents) Files:**
    - [ ] **Root `toc.yml` (`docs/docfx/toc.yml`):** Define main navigation (Home, Intro, Getting Started, API Reference, conceptual topics).
    - [ ] **API `toc.yml` (`docs/docfx/api/toc.yml`):** Plan to let DocFX generate this initially.

- [ ] **5. Create `index.md` (`docs/docfx/index.md`):**
    - [ ] Create the homepage content.

## 2. XML Documentation Comments

- [x] **Mandatory for all public types and members.**
    *(Status: Some XML comments exist, e.g., in `ITasksService.cs` and `CuTask.cs`. However, comprehensive coverage across all public APIs is needed.)*
- [x] **Standard XML Tags:** Use `<summary>`, `<param>`, `<returns>`, etc.
- [ ] **Enable XML Documentation File Generation in `.csproj` files:**
    - [x] Add `<GenerateDocumentationFile>true</GenerateDocumentationFile>` to: (Completed 2024-07-12)
        - [x] `src/ClickUp.Api.Client.Abstractions/ClickUp.Api.Client.Abstractions.csproj`
        - [x] `src/ClickUp.Api.Client.Models/ClickUp.Api.Client.Models.csproj`
        - [x] `src/ClickUp.Api.Client/ClickUp.Api.Client.csproj`
- [x] **Review and Enhance Existing Comments:** (Completed 2024-07-12 - Interfaces reviewed and found complete. Service implementations use `inheritdoc`.) A full pass is required to ensure completeness and accuracy.

## 3. Conceptual Documentation (Articles)

- [ ] Location: `docs/docfx/articles/` (To be created).
- [ ] Create Markdown (`.md`) files for conceptual topics.
- [ ] **Key Topics to Cover:**
    - [ ] **`intro.md`**
    - [ ] **`getting-started.md`** (Installation, DI setup, basic example)
    - [ ] **`authentication.md`** (Personal API Tokens, OAuth guidance)
    - [ ] **`error-handling.md`** (Exception hierarchy, usage)
    - [ ] **`pagination.md`** (`IAsyncEnumerable<T>` usage)
    - [ ] **`rate-limiting.md`** (API limits, SDK's Polly policies)
    - [ ] **(Optional) Advanced Topics:** Custom Fields, Webhooks, Semantic Kernel.
- [ ] Use Markdown, include code snippets.

## 4. Building and Serving Documentation

- [ ] **Build Command:** `docfx build docs/docfx/docfx.json` (Once setup).
- [ ] **Serve Command (for local preview):** `docfx serve docs/docfx/_site` (Once setup).
- [ ] Iteratively build and review.

## 5. GitHub Actions Workflow for Documentation Deployment

- [ ] Create a GitHub Actions workflow (e.g., `.github/workflows/docfx.yml`).
- [ ] **Trigger:** On push to `main` or release.
- [ ] **Steps:** Checkout, Setup .NET, Install DocFX, Build docs, Deploy to GitHub Pages.
    ```yaml
    # .github/workflows/docfx.yml (Conceptual - To be created)
    # name: Deploy Documentation
    # ... (workflow content from plan) ...
    ```

## 6. Final Review
- [ ] Thoroughly review all generated documentation once available.
- [ ] Check links, formatting, code examples.

**Overall Status:** The plan for DocFX setup and documentation generation is clear. However, the actual implementation (DocFX initialization, comprehensive XML comments, conceptual articles, build workflow) is largely pending. Some initial XML comments exist in the code, but generation of the XML documentation file is not yet enabled in the project files.
```
```
