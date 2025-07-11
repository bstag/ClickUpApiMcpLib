# ClickUp .NET SDK for ClickUp REST API

[![NuGet](https://img.shields.io/nuget/v/Stagware.ClickUp.Api.Client.svg)](https://www.nuget.org/packages/Stagware.ClickUp.Api.Client)
[![Build](https://github.com/your-org/ClickUpApiMcpLib/actions/workflows/build.yml/badge.svg)](https://github.com/your-org/ClickUpApiMcpLib/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/your-org/ClickUpApiMcpLib.svg)](LICENSE)

Typed, resilient, and dependency-injection-ready .NET 9 library for the full ClickUp public REST API.

> **Note:** This library is currently in pre-release (0.x versions) while the API stabilizes. Breaking changes may occur between minor versions until 1.0.0 is released.

---

## ✨ Features

* Task management (CRUD, checklists, tags, relationships, attachments)
* Comments & Chat messages
* Time tracking entries & summaries
* Spaces, Folders, Lists, and Goals
* Members, Guests, Roles, User Groups
* Custom Fields & Views
* Docs, Templates, Webhooks, Shared Hierarchy and more
* Polly-powered retries and `HttpClientFactory` integration
* 100% nullable-annotated models with XML docs

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

## 🧰 Service Map

| ClickUp Feature | Service Interface | Implementation |
|-----------------|-------------------|----------------|
| Tasks           | `ITaskService`            | `TaskService` |
| Task Checklists | `ITaskChecklistsService`  | `TaskChecklistsService` |
| Time Tracking   | `ITimeTrackingService`    | `TimeTrackingService` |
| Users           | `IUsersService`           | `UsersService` |
| Spaces          | `ISpacesService`          | `SpacesService` |
| … and many more | –                         | See `Services/` folder |

Retrieve any service via DI: `var svc = provider.GetRequiredService<ITaskService>();`

---

## 📦 Examples

The [`/examples`](examples/) directory contains small console apps and scripts:

| Example | What it shows |
|---------|---------------|
| `ConsoleDemo` | End-to-end: list spaces, create a task, upload attachment |
| `RetryPolicyDemo` | Plugging a custom Polly circuit-breaker |
| `WebhookListener` | Minimal ASP.NET Core webhook receiver |

Run an example:
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

## 🗺 Roadmap

* gRPC transport (when ClickUp publishes proto)  
* Strongly typed webhooks  
* Auto-generated docs site via DocFX + GitHub Pages

---

## 📄 License

This library is released under the MIT License. See the [LICENSE](LICENSE) file for details.