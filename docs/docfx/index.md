# ClickUp .NET SDK Documentation

Welcome to the comprehensive documentation for the **ClickUp .NET SDK** - a modern, type-safe, and resilient .NET library for integrating with the ClickUp API.

## Quick Start

```bash
dotnet add package ClickUp.Api.Client
```

```csharp
using ClickUp.Api.Client.Extensions;

// Configure services
services.AddClickUpClient(options =>
{
    options.PersonalAccessToken = "your_token_here";
});

// Use the service
var tasksService = serviceProvider.GetRequiredService<ITasksService>();
var tasks = await tasksService.GetTasksAsync("list_id");
```

## Documentation Sections

### Getting Started
- **[Installation & Setup](articles/getting-started.md)** - Install the SDK and configure your first client
- **[Authentication](articles/authentication.md)** - Learn about Personal Access Tokens and OAuth 2.0
- **[Your First API Call](articles/getting-started.md#first-api-call)** - Make your first request to the ClickUp API

### Core Features
- **[Error Handling](articles/error-handling.md)** - Robust error handling and custom exceptions
- **[Pagination](articles/pagination.md)** - Handle paginated responses efficiently
- **[Rate Limiting & Resilience](articles/rate-limiting.md)** - Built-in retry policies and circuit breakers
- **[Webhooks](articles/webhooks.md)** - Handle ClickUp webhook events

### Advanced Topics
- **[Semantic Kernel Integration](articles/semantic-kernel-integration.md)** - AI-powered integrations
- **[Documentation Workflow](articles/workflow.md)** - Contributing to documentation

### API Reference
- **[Complete API Reference](api/index.md)** - Detailed documentation for all classes, methods, and models

## Key Features

- **Type-Safe** - Fully typed models with nullable reference types
- **Resilient** - Built-in retry policies and circuit breakers using Polly
- **Async/Await** - Modern async programming patterns throughout
- **Dependency Injection** - First-class support for Microsoft.Extensions.DependencyInjection
- **Well-Documented** - Comprehensive XML documentation and examples
- **Thoroughly Tested** - Extensive unit and integration test coverage

## Supported ClickUp Features

| Feature | Status | Documentation |
|---------|--------|---------------|
| Tasks | Complete | [ITasksService](api/ClickUp.Api.Client.Abstractions.Services.ITasksService.html) |
| Lists | Complete | [IListsService](api/ClickUp.Api.Client.Abstractions.Services.IListsService.html) |
| Spaces | Complete | [ISpacesService](api/ClickUp.Api.Client.Abstractions.Services.ISpacesService.html) |
| Comments | Complete | [ICommentsService](api/ClickUp.Api.Client.Abstractions.Services.ICommentsService.html) |
| Time Tracking | Complete | [ITimeTrackingService](api/ClickUp.Api.Client.Abstractions.Services.ITimeTrackingService.html) |
| Webhooks | Complete | [IWebhooksService](api/ClickUp.Api.Client.Abstractions.Services.IWebhooksService.html) |
| Custom Fields | Complete | [ICustomFieldsService](api/ClickUp.Api.Client.Abstractions.Services.ICustomFieldsService.html) |
| Goals | Complete | [IGoalsService](api/ClickUp.Api.Client.Abstractions.Services.IGoalsService.html) |

## Contributing

We welcome contributions! Please see our [Contributing Guide](https://github.com/your-org/ClickUpApiMcpLib/blob/main/CONTRIBUTING.md) for details.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/your-org/ClickUpApiMcpLib/blob/main/LICENSE) file for details.