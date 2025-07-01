# Contributing to ClickUpApiMcpLib

First off, thank you for taking the time to contribute! This project is community-driven and we appreciate your help improving the SDK.

The sections below cover the essential information you need to submit issues and pull-requests. If you are new to open-source contributions, feel free to open a draft PR early and ask questions.

## Table of Contents

1. [Code of Conduct](#code-of-conduct)
2. [Getting Started](#getting-started)
3. [Coding Guidelines](#coding-guidelines)
4. [Commit & PR Process](#commit--pr-process)
5. [SDK Method Parameter Conventions](#sdk-method-parameter-conventions)
6. [DTO Naming Conventions](#dto-naming-conventions)

---

## Code of Conduct

We follow the [Contributor Covenant](https://www.contributor-covenant.org/version/2/1/code_of_conduct/) to foster an open and welcoming environment.

## Getting Started

1. Fork the repository and clone your fork.
2. Install the .NET 9 SDK (or the version specified in `global.json`).
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Run the full test suite:
   ```bash
   dotnet test -c Release
   ```
5. Create a new branch from `main` for your work:
   ```bash
   git checkout -b <feature|bugfix>/<short-description>
   ```

## Coding Guidelines

* Follow **.editorconfig** rules – the CI build treats warnings as errors.
* Run `dotnet format` before committing.
* New code **must** include unit tests. Integration tests are required when touching API-interactive code.
* Public APIs require XML-doc comments.
* Prefer immutable objects (`record`/`init`) where possible.

## Commit & PR Process

1. Keep commits focused and logically grouped.
2. Use conventional commit messages (`feat:`, `fix:`, `docs:`…).
3. Link the corresponding ClickUp task in the PR description when applicable.
4. Ensure the PR checks ✅ (build, analyzers, tests) pass.
5. Once approved, a maintainer will squash-merge your PR.

---

## SDK Method Parameter Conventions

To reduce cognitive load and prevent usage errors, **all public SDK methods MUST follow the canonical identifier order below**.

| Scope | Parameter Name |
|-------|----------------|
| Workspace | `workspaceId` *(alias `teamId`)* |
| Space | `spaceId` |
| Folder | `folderId` |
| List | `listId` |
| Task | `taskId` |
| Sub-entity | `entityId` *(comments, checklist items, etc.)* |

Rules:

1. List the identifiers exactly in the order above — **never rearrange or omit a broader scope that is required for context**.
2. After identifiers, place other **required** parameters.
3. Finally, append **optional** parameters (nullable reference types or those with default values).
4. The last parameter of every `async` method MUST be `CancellationToken cancellationToken = default`.

### Example

```csharp
public Task<CommentResponse> CreateTaskCommentAsync(
    string workspaceId,
    string spaceId,
    string folderId,
    string listId,
    string taskId,
    string entityId, // comment id when applicable
    CreateCommentRequest request,
    CancellationToken cancellationToken = default);
```

If a specific identifier is implicitly contained in a lower-level ID (e.g., `taskId` uniquely identifies the list), you may omit the higher-level IDs **only if** the ClickUp API endpoint does not require them.

---

## DTO Naming Conventions

To ensure consistency and improve discoverability across the SDK, Data Transfer Objects (DTOs) should follow a specific naming scheme:

1.  **Request DTOs**:
    *   Classes used as request bodies for API operations (typically for `POST` or `PUT` methods) MUST be suffixed with `Request`.
    *   The convention is `ServiceNameOperationRequest`. For example, a DTO to create a task would be named `CreateTaskRequest`. If the service is implicitly known (e.g., `TasksService`), it might be shortened, but the operation and `Request` suffix are key.
    *   Example: `CreateTaskRequest`, `UpdateListRequest`.

2.  **Response DTOs**:
    *   Classes that model the direct response from an API operation (typically for `GET`, `POST`, `PUT`, or `DELETE` methods that return data) MUST be suffixed with `Response`.
    *   The convention is `ServiceNameOperationResponse`. For example, the response from an operation that retrieves a specific task would be `GetTaskResponse`.
    *   Example: `GetTaskResponse`, `CreateFolderResponse`.

3.  **Nested/Component Models**:
    *   Models that are part of a larger request or response DTO, but are not top-level request/response DTOs themselves, should have descriptive names but generally AVOID the `Request` or `Response` suffix unless they themselves could also function as standalone request/response DTOs for other specific operations.
    *   Suffixes like `Dto`, `Details`, `Info`, `Model`, `Body`, `Payload` should generally be avoided for new models in favor of more descriptive names or by adhering to the `Request`/`Response` suffix rule if they are indeed top-level DTOs. Existing models with these suffixes are being progressively refactored.

This convention helps in quickly identifying the purpose of a model (request, response, or general component) and its associated API operation.

---

Thank you for helping make the ClickUp SDK better! Feel free to reach out by opening an issue if anything here is unclear.
