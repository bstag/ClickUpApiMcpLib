# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Build
```bash
# Build the entire solution
./build_solution.sh

# Or use dotnet directly
dotnet build src/ClickUp.Api.sln --nologo
```

### Test
```bash
# Run unit tests
dotnet test src/ClickUp.Api.Client.Tests

# Run integration tests (requires CLICKUP_TOKEN env var)
dotnet test src/ClickUp.Api.Client.IntegrationTests
```

### Examples
```bash
# Run console example
cd examples/ClickUp.Api.Client.Console
dotnet run

# Run fluent API example
cd examples/ClickUp.Api.Client.Fluent.Console
dotnet run
```

## Architecture Overview

This is a .NET 9 SDK for the ClickUp REST API with a layered architecture:

### Core Components

**Service Layer Pattern**
- **Abstractions**: `src/ClickUp.Api.Client.Abstractions/Services/` - Interface definitions (ITasksService, IUsersService, etc.)
- **Implementations**: `src/ClickUp.Api.Client/Services/` - Concrete service implementations
- Each service handles a specific ClickUp domain (tasks, users, spaces, etc.)

**HTTP Communication**
- **IApiConnection**: Abstract HTTP operations (GET, POST, PUT, DELETE)
- **ApiConnection**: Implementation using HttpClient with JSON serialization
- **Polly Integration**: Exponential backoff, circuit breaker, and rate limit handling

**Fluent API**
- **ClickUpClient**: Main entry point with fluent interface
- **Fluent APIs**: Builder pattern for request construction (`TasksFluentApi`, `SpacesFluentApi`, etc.)
- Usage: `client.Tasks.GetTask("123").WithCustomTaskIds().ExecuteAsync()`

**Exception Handling**
- **Hierarchical Model**: Base `ClickUpApiException` with specific subclasses
- **Factory Pattern**: `ClickUpApiExceptionFactory` maps HTTP status codes to appropriate exceptions
- **ClickUp-specific**: Handles ClickUp's error format (`err`, `ECODE`) and validation details

**Models**
- **Entities**: Core domain objects (Task, User, Space, etc.)
- **Requests**: Request DTOs for API calls
- **Responses**: Response DTOs from API calls
- **Pagination**: `IPagedResult<T>` with cursor and count-based pagination

### Key Patterns

1. **Interface Segregation**: Each service has a focused interface
2. **Dependency Injection**: Constructor injection throughout
3. **Factory Pattern**: Exception creation and HTTP client configuration
4. **Repository Pattern**: Services act as repositories for ClickUp entities
5. **Builder Pattern**: Fluent API request construction
6. **Async/Await**: Comprehensive async support with cancellation tokens

### Data Flow
```
Client Code → Fluent API → Service Interface → Service Implementation → HTTP Layer → ClickUp API
```

## Working with the Codebase

### Adding New Services
1. Create interface in `src/ClickUp.Api.Client.Abstractions/Services/`
2. Implement in `src/ClickUp.Api.Client/Services/`
3. Add to DI container in `DependencyInjection.cs`
4. Create fluent API wrapper in `src/ClickUp.Api.Client/Fluent/`

### Adding New Models
- **Entities**: Domain objects in `src/ClickUp.Api.Client.Models/Entities/`
- **Requests**: Request DTOs in `src/ClickUp.Api.Client.Models/RequestModels/`
- **Responses**: Response DTOs in `src/ClickUp.Api.Client.Models/ResponseModels/`

### Testing Strategy
- **Unit Tests**: Mock `IApiConnection` and test service logic
- **Integration Tests**: Use real API with recorded responses where possible
- **Examples**: Demonstrate real-world usage patterns

### Key Files to Understand
- `src/ClickUp.Api.Client/Http/ApiConnection.cs` - HTTP layer implementation
- `src/ClickUp.Api.Client/Http/ClickUpApiExceptionFactory.cs` - Exception handling
- `src/ClickUp.Api.Client/Fluent/ClickUpClient.cs` - Main fluent API entry point
- `src/ClickUp.Api.Client/DependencyInjection.cs` - DI configuration
- `src/ClickUp.Api.Client/Http/Handlers/HttpPolicyBuilders.cs` - Polly resilience policies

## Project Structure

```
src/
├── ClickUp.Api.Client.Abstractions/     # Service interfaces
├── ClickUp.Api.Client.Models/           # DTOs, entities, exceptions
├── ClickUp.Api.Client/                  # Main implementation
│   ├── Services/                        # Service implementations
│   ├── Http/                           # HTTP layer
│   ├── Fluent/                         # Fluent API
│   └── Helpers/                        # Utility classes
├── ClickUp.Api.Client.Tests/            # Unit tests
└── ClickUp.Api.Client.IntegrationTests/ # Integration tests

examples/                                # Example applications
docs/                                   # Documentation
```

## Environment Setup

For integration tests and examples, set the `CLICKUP_TOKEN` environment variable:
```bash
export CLICKUP_TOKEN=your_token_here
```