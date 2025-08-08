# Coding Style and Conventions

## General C# Conventions

### Code Formatting
- **Indentation**: 4 spaces (configured in `.editorconfig`)
- **Line Endings**: LF (Unix-style) with final newline
- **File Encoding**: UTF-8
- **Max Line Length**: Follow standard C# conventions

### Language Features
- **Nullable Reference Types**: Enabled for all projects (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Target Framework**: .NET 9 (`<TargetFramework>net9.0</TargetFramework>`)
- **Prefer `record` types** for immutable data models
- **Use `init` accessors** where possible for immutability

### Documentation
- **XML Documentation**: Required for all public APIs (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`)
- **Warning Suppression**: XML doc warnings (1591) are suppressed but documentation is still required
- **Treat Warnings as Errors**: Enabled in CI/CD but configurable

## SDK-Specific Conventions

### Method Parameter Order
**All public SDK methods MUST follow this canonical identifier order:**

1. `workspaceId` (alias `teamId`)
2. `spaceId`
3. `folderId` 
4. `listId`
5. `taskId`
6. `entityId` (for sub-entities like comments, checklist items)
7. Other required parameters
8. Optional parameters (nullable or with defaults)
9. `CancellationToken cancellationToken = default` (always last for async methods)

**Example:**
```csharp
public Task<CommentResponse> CreateTaskCommentAsync(
    string workspaceId,
    string spaceId,
    string folderId,
    string listId,
    string taskId,
    string entityId,
    CreateCommentRequest request,
    CancellationToken cancellationToken = default);
```

### DTO Naming Conventions

#### Request DTOs
- **Suffix**: Always end with `Request`
- **Pattern**: `{Operation}{Entity}Request`
- **Examples**: `CreateTaskRequest`, `UpdateListRequest`

#### Response DTOs  
- **Suffix**: Always end with `Response`
- **Pattern**: `{Operation}{Entity}Response` or `Get{Entity}Response`
- **Examples**: `GetTaskResponse`, `CreateFolderResponse`

#### Nested/Component Models
- **Avoid generic suffixes**: No `Dto`, `Details`, `Info`, `Model`, `Body`, `Payload`
- **Use descriptive names** or follow Request/Response pattern if they're top-level DTOs
- **Examples**: `TaskAssignee`, `SpaceSettings`, `ListPermissions`

## Service Architecture Patterns

### Interface Segregation
- **Primary Interfaces**: `ITasksService`, `IUsersService`, etc.
- **Specialized Interfaces**: `ITaskCrudService`, `ITaskQueryService`, `ITaskRelationshipService`
- **Implementation**: Single class implements multiple focused interfaces

### Dependency Injection
- **Constructor Injection**: All dependencies injected via constructor
- **Service Registration**: Use `AddClickUpClient()` extension method
- **Scoped Lifetime**: Services registered as scoped by default

### Error Handling
- **Custom Exceptions**: Inherit from `ClickUpApiException`
- **Factory Pattern**: Use `ClickUpApiExceptionFactory` for HTTP status mapping
- **Structured Errors**: Support ClickUp's `err` and `ECODE` format

## Testing Conventions

### Unit Tests
- **Framework**: xUnit
- **Mocking**: Mock `IApiConnection` for HTTP calls
- **Naming**: `{MethodName}_{Scenario}_{ExpectedResult}`
- **Location**: `src/ClickUp.Api.Client.Tests/`

### Integration Tests
- **Real API**: Require `CLICKUP_TOKEN` environment variable
- **Setup**: Use `IntegrationTestBase` base class
- **Location**: `src/ClickUp.Api.Client.IntegrationTests/`

## Development Workflow
1. **Format Code**: Run `dotnet format` before committing
2. **Analyzer Compliance**: Fix all analyzer warnings (treated as errors in CI)
3. **Tests Required**: New code must include unit tests
4. **XML Documentation**: Required for all public APIs
5. **Conventional Commits**: Use `feat:`, `fix:`, `docs:` prefixes