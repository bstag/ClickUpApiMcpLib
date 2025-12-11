# ClickUp .NET SDK for ClickUp REST API

[![NuGet](https://img.shields.io/nuget/v/Stagware.ClickUp.Api.Client.svg)](https://www.nuget.org/packages/Stagware.ClickUp.Api.Client)
[![Build](https://github.com/your-org/ClickUpApiMcpLib/actions/workflows/build.yml/badge.svg)](https://github.com/your-org/ClickUpApiMcpLib/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/your-org/ClickUpApiMcpLib.svg)](LICENSE)

Typed, resilient, and dependency-injection-ready .NET 9 library for the full ClickUp public REST API.

> **Note:** This library is currently in pre-release (0.x versions) while the API stabilizes. Breaking changes may occur between minor versions until 1.0.0 is released.

---

## ✨ Features

### Core API Coverage
* Task management (CRUD, checklists, tags, relationships, attachments)
* Comments & Chat messages
* Time tracking entries & summaries
* Spaces, Folders, Lists, and Goals
* Members, Guests, Roles, User Groups
* Custom Fields & Views
* Docs, Templates, Webhooks, Shared Hierarchy and more

### Architecture & Design
* **🏗️ Service Decomposition**: Task and view services decomposed into smaller, focused interfaces following Single Responsibility Principle
* **🔧 Infrastructure Abstraction**: Abstraction layer with implementations for file system, date/time providers, HTTP client factory, and configuration
* **⚡ Enhanced Fluent API**: URL builders, templates, validation pipelines, and configuration builders for improved developer experience
* **🔌 Extensible Plugin System**: Sample plugins for logging, rate limiting, and caching with extensible architecture
* **🆕 Modular CLI Tool**: Production-ready command-line interface with 26 organized modules
* **🆕 Enhanced Architecture**: Clean separation of concerns with focused command modules

### Developer Experience
* Polly-powered retries and `HttpClientFactory` integration
* 100% nullable-annotated models with XML docs
* Type-safe fluent API with validation
* Comprehensive error handling and logging

> Want the nitty-gritty? Browse the XML-generated docs in the [`/docs`](docs/) folder.

---

## 🚀 Quickstart

### 1. Install via NuGet
```powershell
> dotnet add package Stagware.ClickUp.Api.Client
```

### 2. Configure DI & Make Your First Call
```csharp
using Microsoft.Extensions.DependencyInjection;
using ClickUp.Api.Client;

var services = new ServiceCollection();

services.AddClickUpClient(options =>
{
    options.ApiToken = Environment.GetEnvironmentVariable("CLICKUP_TOKEN");
});

var provider = services.BuildServiceProvider();
var userService = provider.GetRequiredService<IUsersService>();

var me = await userService.GetAuthorizedUserAsync();
Console.WriteLine($"Hello {me.Username} 👋");
```

That's it! You now have fully typed access to every ClickUp endpoint.

## 📚 Documentation

Comprehensive documentation is available at: **[https://your-username.github.io/ClickUpApiMcpLib/](https://your-username.github.io/ClickUpApiMcpLib/)**

The documentation includes:
- **[Getting Started Guide](https://your-username.github.io/ClickUpApiMcpLib/articles/getting-started.html)** - Installation and setup
- **[API Reference](https://your-username.github.io/ClickUpApiMcpLib/api/)** - Complete API documentation
- **[Authentication Guide](https://your-username.github.io/ClickUpApiMcpLib/articles/authentication.html)** - OAuth and Personal Access Tokens
- **[Error Handling](https://your-username.github.io/ClickUpApiMcpLib/articles/error-handling.html)** - Exception handling patterns
- **[Pagination](https://your-username.github.io/ClickUpApiMcpLib/articles/pagination.html)** - Working with paginated responses

---

## ⚙️ Configuration

| Option                | Description                                               | Default |
|-----------------------|-----------------------------------------------------------|---------|
| `ApiToken`            | **Required.** Personal or OAuth token                     | –       |
| `BaseUrl`             | Override API root (for mock/testing)                      | `https://api.clickup.com/api` |
| `RetryPolicy`         | Polly `IAsyncPolicy<HttpResponseMessage>` for resiliency  | Exponential-backoff 3 tries |
| `UserAgent`           | Custom `User-Agent` header                                | `ClickUpDotNetSdk/{version}` |

Configure via lambda in `AddClickUpClient`, environment variables, or `IConfiguration` binding.

---

## 🧰 Service Architecture

### Decomposed Service Interfaces
Following the Single Responsibility Principle, services are decomposed into focused interfaces:

| ClickUp Feature | Primary Interface | Specialized Interfaces |
|-----------------|-------------------|------------------------|
| Tasks           | `ITasksService`   | `ITaskCrudService`, `ITaskQueryService`, `ITaskRelationshipService`, `ITaskTimeTrackingService` |
| Views           | `IViewsService`   | `IViewCrudService`, `IViewQueryService` |
| Task Checklists | `ITaskChecklistsService`  | `TaskChecklistsService` |
| Time Tracking   | `ITimeTrackingService`    | `TimeTrackingService` |
| Users           | `IUsersService`           | `UsersService` |
| Spaces          | `ISpacesService`          | `SpacesService` |
| … and many more | –                         | See `Services/` folder |

### Infrastructure Abstraction Layer
The SDK includes abstraction implementations for:
- **File System**: `IFileSystemProvider` with local and cloud implementations
- **Date/Time**: `IDateTimeProvider` for testable time operations
- **HTTP Client Factory**: `IHttpClientFactory` integration with custom configurations
- **Configuration**: `IConfigurationProvider` with multiple source support

Retrieve any service via DI: `var svc = provider.GetRequiredService<ITasksService>();`

### Enhanced Fluent API
```csharp
// URL Builder with validation
var endpoint = UrlBuilder.Create()
    .WithPathSegments("task", taskId)
    .WithQueryParameter("include_subtasks", true)
    .ValidateAndBuild();

// Configuration Builder
var config = ClickUpConfigurationBuilder.Create()
    .WithApiToken(token)
    .WithRetryPolicy(retryPolicy)
    .WithValidation()
    .Build();

// Template-based requests
var taskTemplate = TaskTemplate.Create()
    .WithName("{{taskName}}")
    .WithDescription("{{description}}")
    .WithAssignee("{{assigneeId}}");
```

### Plugin System
Extensible plugin architecture with sample implementations:

```csharp
// Register plugins during configuration
services.AddClickUpClient(options =>
{
    options.ApiToken = "your-token";
})
.AddPlugin<LoggingPlugin>()
.AddPlugin<RateLimitingPlugin>(config => 
{
    config.RequestsPerMinute = 100;
})
.AddPlugin<CachingPlugin>(config =>
{
    config.DefaultExpiration = TimeSpan.FromMinutes(5);
});
```

**Available Plugins:**
- **LoggingPlugin**: Comprehensive request/response logging
- **RateLimitingPlugin**: Configurable rate limiting with backoff
- **CachingPlugin**: Response caching with TTL and invalidation
- **Custom Plugins**: Implement `IClickUpPlugin` for custom functionality

---

## 📦 Examples

The [`/examples`](examples/) directory contains practical applications and demos:

| Example | What it shows |
|---------|---------------|
| **`ClickUp.Api.Client.CLI`** | **Full-featured command-line interface demonstrating comprehensive SDK usage** |
| `ConsoleDemo` | End-to-end: list spaces, create a task, upload attachment |
| `RetryPolicyDemo` | Plugging a custom Polly circuit-breaker |
| `WebhookListener` | Minimal ASP.NET Core webhook receiver |

### 🖥️ CLI Tool - Real-World SDK Usage

The **ClickUp CLI** is a production-ready command-line tool that showcases the SDK's full capabilities with a modular architecture featuring 26 organized command modules:

```bash
# Setup authentication
cd examples/ClickUp.Api.Client.CLI
dotnet run -- config set-api-key YOUR_API_KEY

# Authentication and user management
dotnet run -- auth user get
dotnet run -- auth workspaces list
dotnet run -- user list WORKSPACE_ID

# Project structure operations
dotnet run -- space list WORKSPACE_ID
dotnet run -- folder list SPACE_ID
dotnet run -- list list FOLDER_ID

# Task management
dotnet run -- task list LIST_ID --output-format table
dotnet run -- task get TASK_ID --include-subtasks
dotnet run -- task-checklist list TASK_ID

# Advanced features
dotnet run -- goal list WORKSPACE_ID
dotnet run -- docs search WORKSPACE_ID --query "project"
dotnet run -- webhook list WORKSPACE_ID
```

**Key Features:**
- 🔐 **Authentication**: Personal access tokens with secure config management
- 📊 **Multiple Output Formats**: JSON, CSV, Table, Properties
- 🔍 **Advanced Filtering**: Property selection, pagination, search
- 🐛 **Debug Mode**: HTTP request/response logging with `--debug`
- ⚡ **Performance**: Efficient API usage with proper error handling
- 🏗️ **Modular Architecture**: 26 focused command modules for better organization

**Recent Improvements:**
- **Complete API Coverage**: All major ClickUp API endpoints are now accessible
- **Enhanced Organization**: Commands are logically grouped by functionality
- **Better Maintainability**: Each module handles a specific domain area

The CLI demonstrates real-world patterns for:
- Dependency injection setup
- Configuration management
- Error handling and user feedback
- Data transformation and formatting
- Hierarchical data navigation (Workspaces → Spaces → Folders → Lists → Tasks)

See the [CLI README](examples/ClickUp.Api.Client.CLI/README.md) for complete documentation.

### Quick Start with Other Examples

```bash
export CLICKUP_TOKEN=your_token_here
cd examples/ConsoleDemo
dotnet run
```

---

## 🛠 Testing & Contributing

* **Unit tests:** `dotnet test src/ClickUp.Api.Client.Tests`
* **Integration tests:** require `CLICKUP_TOKEN` env var and hit the real API.  
  `dotnet test src/ClickUp.Api.Client.IntegrationTests`
* Contributions are welcome! Please open an issue or PR following our [contributing guide](CONTRIBUTING.md) and use Conventional Commits.

---

## 🗺️ Roadmap

### ✅ Completed
- Core API client with all major endpoints
- Comprehensive response models
- Authentication and configuration
- Error handling and logging
- **CLI tool with modular architecture (26 command modules)**
- **Complete API coverage in CLI**
- **Refactored command structure for better maintainability**
- Unit and integration tests
- Documentation and examples
- Multiple output formats (JSON, Table, CSV)
- Advanced CLI features (filtering, pagination, bulk operations)

### 🚧 In Progress
- Performance optimizations
- Enhanced error handling
- Additional integration tests

### 📋 Planned
- Rate limiting improvements
- Caching mechanisms
- Additional SDK language bindings
- Enhanced webhook support
- Real-time API features
- gRPC transport (when ClickUp publishes proto)
- Strongly typed webhooks
- Auto-generated docs site via DocFX + GitHub Pages

---

## 📄 License

This library is released under the MIT License. See the [LICENSE](LICENSE) file for details.