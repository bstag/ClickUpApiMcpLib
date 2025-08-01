# Fluent API

This document describes the enhanced fluent API features in the ClickUp API Client SDK, including URL builders, templates, validation pipelines, and configuration builders.

## Overview

The ClickUp SDK provides a comprehensive fluent API that makes it easy to construct complex requests, configure services, and build URLs in a readable and maintainable way. The fluent API follows the builder pattern and provides method chaining for improved developer experience.

## URL Builders

The SDK includes enhanced URL building capabilities that provide a fluent interface for constructing API endpoints with parameters, query strings, and path segments.

### FluentUrlBuilder

```csharp
// Basic URL construction
var url = FluentUrlBuilder.Create()
    .WithBaseUrl("https://api.clickup.com/api/v2")
    .WithPath("team")
    .WithPath(teamId)
    .WithPath("task")
    .Build();

// URL with query parameters
var urlWithQuery = FluentUrlBuilder.Create()
    .WithBaseUrl("https://api.clickup.com/api/v2")
    .WithPath("list")
    .WithPath(listId)
    .WithPath("task")
    .WithQueryParameter("archived", "false")
    .WithQueryParameter("page", "0")
    .WithQueryParameter("order_by", "created")
    .WithQueryParameter("reverse", "true")
    .Build();

// Conditional URL building
var conditionalUrl = FluentUrlBuilder.Create()
    .WithBaseUrl(baseUrl)
    .WithPath("task")
    .WithPath(taskId)
    .WithQueryParameterIf(!string.IsNullOrEmpty(customTaskIds), "custom_task_ids", "true")
    .WithQueryParameterIf(includeSubtasks, "include_subtasks", "true")
    .Build();
```

### UrlBuilderHelper

Utility methods for common URL construction patterns:

```csharp
// Create a new builder instance
var builder = UrlBuilderHelper.CreateBuilder();

// Build query strings from objects
var queryString = UrlBuilderHelper.BuildQueryString(new
{
    archived = false,
    page = 0,
    order_by = "created",
    reverse = true
});

// Build query strings from dictionaries
var parameters = new Dictionary<string, object>
{
    ["status"] = "open",
    ["assignees"] = new[] { "123", "456" },
    ["due_date_gt"] = DateTime.UtcNow.AddDays(-7)
};
var queryFromDict = UrlBuilderHelper.BuildQueryString(parameters);
```

## Configuration Builders

Fluent configuration builders provide an intuitive way to set up the SDK with various options and settings.

### ClickUpClientBuilder

```csharp
// Basic client configuration
var client = ClickUpClientBuilder.Create()
    .WithApiToken(apiToken)
    .WithBaseUrl("https://api.clickup.com/api/v2")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .Build();

// Advanced client configuration
var advancedClient = ClickUpClientBuilder.Create()
    .WithApiToken(apiToken)
    .WithBaseUrl(customBaseUrl)
    .WithHttpClientFactory(httpClientFactory)
    .WithRetryPolicy(retryPolicy)
    .WithLogging(loggerFactory)
    .WithRateLimiting(rateLimitOptions)
    .WithCaching(cacheOptions)
    .Build();

// Configuration with plugins
var clientWithPlugins = ClickUpClientBuilder.Create()
    .WithApiToken(apiToken)
    .WithPlugin<LoggingPlugin>()
    .WithPlugin<RateLimitingPlugin>(options => 
    {
        options.RequestsPerMinute = 100;
        options.BurstLimit = 10;
    })
    .WithPlugin<CachingPlugin>(options =>
    {
        options.DefaultExpiration = TimeSpan.FromMinutes(5);
        options.MaxCacheSize = 1000;
    })
    .Build();
```

### ServiceCollectionExtensions

Fluent service registration for dependency injection:

```csharp
// Basic service registration
services.AddClickUpClient()
    .WithApiToken(configuration["ClickUp:ApiToken"])
    .WithBaseUrl(configuration["ClickUp:BaseUrl"])
    .WithTimeout(TimeSpan.FromSeconds(30));

// Advanced service registration
services.AddClickUpClient()
    .WithConfiguration(configuration.GetSection("ClickUp"))
    .WithHttpClientFactory()
    .WithRetryPolicy(policy => policy
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
    .WithLogging()
    .WithPlugins(plugins => plugins
        .Add<LoggingPlugin>()
        .Add<RateLimitingPlugin>()
        .Add<CachingPlugin>());
```

## Request Templates

The SDK provides request templates that encapsulate common request patterns and can be reused across different operations.

### TaskRequestTemplate

```csharp
// Create a reusable task request template
var taskTemplate = TaskRequestTemplate.Create()
    .WithName("Task from template")
    .WithDescription("This task was created from a template")
    .WithStatus("to do")
    .WithPriority(3)
    .WithAssignees("123", "456")
    .WithTags("template", "automated")
    .WithDueDate(DateTime.UtcNow.AddDays(7))
    .WithCustomFields(fields => fields
        .Add("Department", "Engineering")
        .Add("Estimated Hours", 8));

// Use the template to create multiple tasks
var task1 = await tasksService.CreateTaskAsync(listId, taskTemplate
    .WithName("Specific Task 1")
    .Build());

var task2 = await tasksService.CreateTaskAsync(listId, taskTemplate
    .WithName("Specific Task 2")
    .WithAssignees("789")
    .Build());
```

### QueryTemplate

```csharp
// Create a reusable query template
var queryTemplate = QueryTemplate.Create()
    .WithPageSize(50)
    .WithOrderBy("created")
    .WithOrderDirection("desc")
    .WithDateRange(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow)
    .WithStatuses("open", "in progress")
    .WithArchived(false);

// Use the template for different queries
var recentTasks = await taskQueryService.GetTasksAsync(listId, queryTemplate
    .WithAssignees(currentUserId)
    .Build());

var teamTasks = await taskQueryService.GetTasksAsync(listId, queryTemplate
    .WithAssignees(teamMemberIds)
    .WithPageSize(100)
    .Build());
```

## Validation Pipelines

The fluent API includes built-in validation pipelines that ensure requests are properly formed before being sent to the API.

### Request Validation

```csharp
// Validation is automatically applied to fluent builders
var validatedRequest = TaskCreateRequestBuilder.Create()
    .WithName("New Task") // Required field validation
    .WithListId(listId)   // Required field validation
    .WithDescription(description) // Length validation
    .WithPriority(priority) // Range validation (1-4)
    .WithDueDate(dueDate) // Date validation (not in past)
    .WithAssignees(assigneeIds) // User existence validation
    .Validate() // Explicit validation call
    .Build();

// Custom validation rules
var customValidatedRequest = TaskUpdateRequestBuilder.Create()
    .WithTaskId(taskId)
    .WithName(name)
    .WithValidationRule(request => 
        !string.IsNullOrWhiteSpace(request.Name), 
        "Task name cannot be empty")
    .WithValidationRule(request => 
        request.Name.Length <= 100, 
        "Task name cannot exceed 100 characters")
    .WithAsyncValidationRule(async request => 
        await IsValidAssignee(request.AssigneeId), 
        "Invalid assignee ID")
    .ValidateAsync()
    .Build();
```

### Validation Pipeline Configuration

```csharp
// Configure global validation settings
services.AddClickUpClient()
    .WithValidation(options =>
    {
        options.EnableStrictValidation = true;
        options.ValidateOnBuild = true;
        options.ThrowOnValidationFailure = true;
        options.CustomValidators.Add<TaskNameValidator>();
        options.CustomValidators.Add<DateRangeValidator>();
    });

// Disable validation for specific scenarios
var quickRequest = TaskCreateRequestBuilder.Create()
    .WithName("Quick Task")
    .WithListId(listId)
    .WithValidation(false) // Disable validation for this request
    .Build();
```

## Advanced Fluent Patterns

### Conditional Building

```csharp
// Conditional method chaining
var request = TaskCreateRequestBuilder.Create()
    .WithName(taskName)
    .WithListId(listId)
    .If(hasDescription, builder => builder.WithDescription(description))
    .If(hasDueDate, builder => builder.WithDueDate(dueDate))
    .If(hasAssignees, builder => builder.WithAssignees(assigneeIds))
    .If(isHighPriority, builder => builder.WithPriority(4))
    .Build();
```

### Batch Operations

```csharp
// Fluent batch operations
var batchResult = await BatchOperationBuilder.Create()
    .AddTask(task => task
        .WithName("Task 1")
        .WithListId(listId)
        .WithAssignees("123"))
    .AddTask(task => task
        .WithName("Task 2")
        .WithListId(listId)
        .WithAssignees("456"))
    .AddTask(task => task
        .WithName("Task 3")
        .WithListId(listId)
        .WithPriority(4))
    .WithConcurrency(3)
    .WithErrorHandling(ErrorHandlingStrategy.ContinueOnError)
    .ExecuteAsync();
```

### Pipeline Composition

```csharp
// Compose multiple operations in a pipeline
var pipelineResult = await OperationPipeline.Create()
    .Step("Create Task", async () => await tasksService.CreateTaskAsync(listId, taskRequest))
    .Step("Add Comment", async (task) => await commentsService.CreateTaskCommentAsync(task.Id, commentRequest))
    .Step("Update Status", async (task) => await tasksService.UpdateTaskAsync(task.Id, statusUpdate))
    .Step("Notify Assignees", async (task) => await notificationService.NotifyAssigneesAsync(task.Id))
    .WithErrorHandling(step => step.RetryOnFailure(3))
    .WithLogging()
    .ExecuteAsync();
```

## Benefits of the Fluent API

### Improved Readability
- Method chaining creates self-documenting code
- Natural language-like syntax
- Clear intent and flow of operations

### Enhanced Developer Experience
- IntelliSense support for discovering available options
- Compile-time validation of method chains
- Reduced boilerplate code

### Flexibility and Extensibility
- Easy to add new builder methods
- Conditional building patterns
- Composable and reusable templates

### Type Safety
- Strong typing throughout the fluent chain
- Compile-time checking of parameter types
- Generic constraints for type safety

## Best Practices

### Builder Reuse
```csharp
// Create base builders for common scenarios
var baseTaskBuilder = TaskCreateRequestBuilder.Create()
    .WithListId(defaultListId)
    .WithStatus("to do")
    .WithTags("automated");

// Extend base builders for specific use cases
var bugTaskBuilder = baseTaskBuilder.Clone()
    .WithPriority(4)
    .WithTags("bug", "urgent");

var featureTaskBuilder = baseTaskBuilder.Clone()
    .WithPriority(2)
    .WithTags("feature", "enhancement");
```

### Error Handling
```csharp
// Graceful error handling in fluent chains
var result = await TaskOperationBuilder.Create()
    .WithTask(taskId)
    .UpdateStatus("in progress")
    .AddComment("Starting work on this task")
    .AssignTo(currentUserId)
    .OnError(error => logger.LogError(error, "Task operation failed"))
    .OnSuccess(task => logger.LogInformation("Task {TaskId} updated successfully", task.Id))
    .ExecuteAsync();
```

### Performance Optimization
```csharp
// Use builders efficiently
var builder = TaskQueryBuilder.Create(); // Reuse builder instance

foreach (var listId in listIds)
{
    var tasks = await builder
        .Reset() // Clear previous state
        .WithListId(listId)
        .WithPageSize(100)
        .WithCaching(TimeSpan.FromMinutes(5))
        .ExecuteAsync();
    
    // Process tasks...
}
```