# ClickUp .NET SDK Project Overview

## Project Purpose
ClickUpApiMcpLib is a comprehensive .NET 9 SDK for the ClickUp REST API. It provides type-safe, resilient access to all ClickUp features including:
- Task management (CRUD, checklists, tags, relationships, attachments)
- Comments & Chat messages
- Time tracking entries & summaries
- Spaces, Folders, Lists, and Goals
- Members, Guests, Roles, User Groups
- Custom Fields & Views
- Docs, Templates, Webhooks, and more

## Tech Stack
- **.NET 9** - Target framework
- **C# with nullable reference types enabled**
- **Microsoft.Extensions.DependencyInjection** - Dependency injection
- **System.Text.Json** - JSON serialization
- **HttpClient with HttpClientFactory** - HTTP communication
- **Polly** - Resilience policies (retries, circuit breaker, rate limiting)
- **NuGet packaging** with GitVersion for versioning

## Key Architecture Components

### Service Layer Pattern
- **Abstractions**: Interface definitions in `src/ClickUp.Api.Client.Abstractions/Services/`
- **Implementations**: Concrete services in `src/ClickUp.Api.Client/Services/`
- **Decomposed Services**: Follows Single Responsibility Principle (e.g., `ITasksService` has specialized interfaces like `ITaskCrudService`, `ITaskQueryService`)

### HTTP Communication
- **IApiConnection**: Abstract HTTP operations
- **ApiConnection**: HttpClient implementation with JSON serialization
- **Polly Integration**: Exponential backoff, circuit breaker, rate limit handling

### Fluent API
- **ClickUpClient**: Main entry point with fluent interface
- **Builder Patterns**: URL builders, configuration builders, templates
- **Validation Pipelines**: Type-safe request construction

### Exception Handling
- **Hierarchical Model**: Base `ClickUpApiException` with specific subclasses
- **Factory Pattern**: Maps HTTP status codes to appropriate exceptions
- **ClickUp-specific**: Handles ClickUp error format (`err`, `ECODE`)

### Plugin System
- **Extensible Architecture**: Sample plugins for logging, rate limiting, caching
- **IClickUpPlugin Interface**: For custom functionality

## Project Structure
```
src/
├── ClickUp.Api.Client.Abstractions/     # Service interfaces
├── ClickUp.Api.Client.Models/           # DTOs, entities, exceptions
├── ClickUp.Api.Client/                  # Main implementation
│   ├── Services/                        # Service implementations
│   ├── Http/                           # HTTP layer
│   ├── Fluent/                         # Fluent API
│   ├── Infrastructure/                 # Abstraction implementations
│   ├── Plugins/                        # Plugin system
│   └── Extensions/                     # DI configuration
├── ClickUp.Api.Client.Tests/           # Unit tests
└── ClickUp.Api.Client.IntegrationTests/ # Integration tests

examples/                               # Example applications
├── ClickUp.Api.Client.CLI/            # Full-featured CLI tool
├── ClickUp.Api.Client.Console/        # Console demo
├── ClickUp.Api.Client.Fluent.Console/ # Fluent API demo
└── ClickUp.Api.Client.Worker/         # Background service demo
```

## Key Features
- **100% nullable-annotated models** with XML documentation
- **Type-safe fluent API** with validation
- **Comprehensive error handling** and logging
- **Production-ready CLI tool** with 26 command modules
- **Plugin architecture** for extensibility  
- **Multiple output formats** (JSON, CSV, Table)
- **GitVersion integration** for semantic versioning