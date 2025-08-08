# Task Completion Checklist

## Code Quality Verification

### Pre-Commit Checks
1. **Format Code**: Run `dotnet format src/ClickUp.Api.sln` to ensure consistent formatting
2. **Build Verification**: Run `dotnet build src/ClickUp.Api.sln --nologo` to ensure compilation success
3. **Unit Tests**: Run `dotnet test src/ClickUp.Api.Client.Tests` to verify unit tests pass
4. **Integration Tests**: Run `dotnet test src/ClickUp.Api.Client.IntegrationTests` (requires `CLICKUP_TOKEN` environment variable)

### Code Analysis
1. **Analyzer Warnings**: Fix all analyzer warnings (treated as errors in CI)
2. **Nullable Reference Types**: Ensure proper null handling throughout
3. **XML Documentation**: Add XML documentation for all public APIs
4. **Exception Handling**: Use proper ClickUp exception hierarchy

## Testing Requirements

### Unit Tests
- [ ] New public methods have corresponding unit tests
- [ ] Edge cases and error conditions are tested
- [ ] Mock `IApiConnection` for HTTP interactions
- [ ] Follow naming convention: `{MethodName}_{Scenario}_{ExpectedResult}`

### Integration Tests  
- [ ] API-interactive code includes integration tests
- [ ] Tests use real ClickUp API with `CLICKUP_TOKEN`
- [ ] Inherit from `IntegrationTestBase` for setup consistency

## Documentation Requirements

### Code Documentation
- [ ] All public APIs have XML documentation comments
- [ ] Complex logic includes inline comments explaining the "why"
- [ ] README updates if public API surface changes
- [ ] RELEASENOTES.md updated for significant changes

### Examples
- [ ] Update relevant examples if API changes affect them
- [ ] CLI tool commands work with any API modifications
- [ ] Console examples demonstrate new features

## Architecture Compliance

### Service Layer
- [ ] Follow canonical parameter order (workspace → space → folder → list → task → entity)
- [ ] Implement appropriate service interfaces
- [ ] Register new services in DI container via `ServiceCollectionExtensions`
- [ ] Use proper exception handling patterns

### DTO Conventions
- [ ] Request DTOs end with `Request` suffix
- [ ] Response DTOs end with `Response` suffix  
- [ ] Avoid generic suffixes (`Dto`, `Info`, `Model`) for new models
- [ ] Use descriptive names for nested models

### HTTP Layer
- [ ] Use `IApiConnection` abstraction for HTTP calls
- [ ] Implement proper error mapping via `ClickUpApiExceptionFactory`
- [ ] Include appropriate retry policies and resilience patterns
- [ ] Support cancellation tokens in async methods

## Final Verification

### Local Testing
1. **Build Solution**: `dotnet build src/ClickUp.Api.sln --configuration Release`
2. **Run All Tests**: `dotnet test src/ClickUp.Api.sln --configuration Release`
3. **Package Creation**: `.\build-packages.ps1` (verify packages build successfully)
4. **Example Applications**: Test relevant examples work correctly

### Environment Testing
- [ ] Test with `CLICKUP_TOKEN` environment variable set
- [ ] Verify examples work with real ClickUp workspace
- [ ] CLI tool functions correctly with new changes
- [ ] Integration tests pass against live API

### Git Workflow
- [ ] Use conventional commit messages (`feat:`, `fix:`, `docs:`, etc.)
- [ ] Branch from `main` with descriptive name (`feature/`, `bugfix/`)
- [ ] Ensure clean commit history
- [ ] PR description links to relevant ClickUp task if applicable

## Performance Considerations
- [ ] HTTP requests use appropriate timeouts
- [ ] Bulk operations use pagination where supported
- [ ] Memory usage is reasonable for large response sets
- [ ] Async/await patterns used consistently

## Security Checklist
- [ ] API tokens handled securely (no logging/exposure)
- [ ] Input validation on user-provided data
- [ ] Proper URL encoding for query parameters
- [ ] No hardcoded secrets in code or tests