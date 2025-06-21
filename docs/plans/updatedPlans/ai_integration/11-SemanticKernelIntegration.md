# Detailed Plan: AI Integration (Semantic Kernel)

This document outlines the plan for integrating the ClickUp API SDK with Microsoft Semantic Kernel, enabling AI agents to interact with ClickUp functionalities.

**Source Documents:**
*   `docs/plans/NEW_OVERALL_PLAN.md` (Phase 4, Steps 14 & 15)

**Location in Codebase:**
*   New Project: `src/ClickUp.Api.Client.SemanticKernel/` (or similar name)
*   This project will contain Semantic Kernel plugins that wrap the SDK's services.

## 1. Goals for Semantic Kernel Integration

*   **Natural Language Interaction:** Allow AI agents (built with Semantic Kernel) to perform ClickUp operations based on natural language commands.
*   **Simplified AI Access:** Provide pre-built plugins that abstract the direct SDK calls into semantic functions understandable by AI.
*   **Extensibility:** Design plugins in a way that new functionalities can be added easily.
*   **(Future) MCP Integration:** Lay the groundwork for potentially exposing these plugins via an MCP (Model Context Protocol) Server.

## 2. Semantic Kernel Plugin Design

1.  **Plugin Structure:**
    *   Each plugin will typically correspond to a major ClickUp entity or a group of related functionalities (e.g., `TaskPlugin`, `ListPlugin`, `CommentPlugin`).
    *   Plugins will be C# classes.
    *   Methods within these classes will be decorated with `[KernelFunction]` and `[Description]` attributes to make them discoverable and usable by Semantic Kernel.

2.  **Native Functions:**
    *   Plugin methods will be "native functions" in Semantic Kernel terminology, meaning they are C# methods that directly call the ClickUp API SDK services.
    *   Each native function will:
        *   Accept parameters that are easy for an AI to understand and populate (e.g., simple strings, numbers).
        *   Internally, map these parameters to the appropriate SDK service method calls and DTOs.
        *   Return a string or a simple object that Semantic Kernel can easily process and present to the AI or user. This might be a summary, a confirmation, or a JSON string of the result.

3.  **Key SDK Services to Wrap:**
    *   Prioritize wrapping services and methods that are most likely to be useful for AI-driven automation:
        *   **`ITasksService`:**
            *   `GetTaskAsync(string taskId, ...)`
            *   `CreateTaskAsync(CreateTaskRequestDto request, ...)`
            *   `UpdateTaskAsync(string taskId, UpdateTaskRequestDto request, ...)`
            *   `DeleteTaskAsync(string taskId, ...)`
            *   `GetTasksAsync(string listId, ...)` (for querying tasks)
        *   **`IListsService`:**
            *   `GetListAsync(string listId, ...)`
            *   `CreateListAsync(string folderId, CreateListRequestDto request, ...)`
            *   `GetListsAsync(string folderId, ...)`
        *   **`ICommentsService`:**
            *   `CreateTaskCommentAsync(string taskId, CreateCommentRequestDto request, ...)`
            *   `GetTaskCommentsAsync(string taskId, ...)`
        *   **`ISpacesService`, `IFoldersService`:** For navigation and context.
        *   Others as deemed high-value for AI interaction.

4.  **Parameter Handling and Descriptions:**
    *   `[Description]` attribute on methods and parameters is crucial. Descriptions should be clear, concise, and explain what the function does and what each parameter means in natural language.
    *   Parameter types should be simple where possible (string, int, bool). For complex inputs (like creating a task with many fields), the function might accept a JSON string representing the SDK's request DTO, or have multiple parameters for common fields.
    *   **Example Semantic Function:**
        ```csharp
        // In src/ClickUp.Api.Client.SemanticKernel/Plugins/TaskPlugin.cs
        public class TaskPlugin
        {
            private readonly ITasksService _tasksService;
            private readonly ILogger<TaskPlugin> _logger;

            public TaskPlugin(ITasksService tasksService, ILogger<TaskPlugin> logger)
            {
                _tasksService = tasksService;
                _logger = logger;
            }

            [KernelFunction, Description("Creates a new task in a specified list.")]
            public async Task<string> CreateTaskAsync(
                [Description("The ID of the list where the task will be created.")] string listId,
                [Description("The name of the new task.")] string taskName,
                [Description("Optional: The description for the task.")] string? description = null,
                [Description("Optional: The ID of the user to assign the task to.")] int? assigneeId = null)
            {
                try
                {
                    var request = new CreateTaskRequestDto
                    {
                        Name = taskName,
                        Description = description,
                        // Assignees might need a more complex structure if multiple or teams
                        Assignees = assigneeId.HasValue ? new List<int> { assigneeId.Value } : null
                    };
                    var createdTask = await _tasksService.CreateTaskAsync(listId, request); // Assuming CreateTaskAsync is on ITasksService
                    return $"Task '{createdTask.Name}' (ID: {createdTask.Id}) created successfully.";
                }
                catch (ClickUpApiException ex)
                {
                    _logger.LogError(ex, "Failed to create task in list {ListId} with name {TaskName}", listId, taskName);
                    return $"Error creating task: {ex.Message} (Status: {ex.HttpStatus}, Code: {ex.ApiErrorCode})";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error creating task in list {ListId}", listId);
                    return $"An unexpected error occurred: {ex.Message}";
                }
            }

            [KernelFunction, Description("Gets details of a specific task by its ID.")]
            public async Task<string> GetTaskDetailsAsync(
                [Description("The ID of the task to retrieve.")] string taskId)
            {
                try
                {
                    var task = await _tasksService.GetTaskAsync(taskId);
                    // Serialize to JSON or return a formatted string
                    return JsonSerializer.Serialize(task, new JsonSerializerOptions { WriteIndented = true });
                }
                catch (ClickUpApiNotFoundException ex)
                {
                    return $"Task with ID '{taskId}' not found. {ex.Message}";
                }
                catch (ClickUpApiException ex)
                {
                     _logger.LogError(ex, "API error getting task {TaskId}", taskId);
                    return $"Error getting task: {ex.Message}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error getting task {TaskId}", taskId);
                    return $"An unexpected error occurred: {ex.Message}";
                }
            }
        }
        ```

5.  **Return Values:**
    *   Functions should return strings that are meaningful to an AI/user. This could be:
        *   A simple success/failure message.
        *   A summary of the created/updated resource.
        *   A JSON string of the resource (if the AI needs to parse structured data).
    *   Consider providing options or separate functions for different levels of detail in returns.

## 3. Project Setup (`src/ClickUp.Api.Client.SemanticKernel/`)

1.  **Create a new .NET Standard or .NET Core library project.**
2.  **Package References:**
    *   `Microsoft.SemanticKernel.Core` (or the main `Microsoft.SemanticKernel` package)
    *   `ClickUp.Api.Client` (project reference to the main SDK)
    *   `Microsoft.Extensions.Logging.Abstractions`

3.  **DI and Plugin Registration:**
    *   Provide extension methods or guidance on how to register these plugins with the Semantic Kernel's `KernelBuilder` or `IServiceCollection` in a consuming application.
    *   Example:
        ```csharp
        // In consuming application's DI setup
        // services.AddClickUpApiClient(options => ...); // From main SDK
        // services.AddScoped<TaskPlugin>(); // Register plugin

        // Kernel setup
        // var kernel = Kernel.CreateBuilder()
        //     .AddOpenAIChatCompletion(...) // Add AI service
        //     .Build();
        // kernel.ImportPluginFromObject(serviceProvider.GetRequiredService<TaskPlugin>(), "ClickUpTask");
        ```

## 4. Semantic Kernel Planners and Prompts

*   While the SDK provides the plugins (tools), the consuming application using Semantic Kernel will be responsible for:
    *   Creating prompts that leverage these functions.
    *   Using Semantic Kernel planners (e.g., `HandlebarsPlanner`, `SequentialPlanner`) to orchestrate calls to these functions based on user intent.
*   The SDK documentation can provide examples of how these plugins *could* be used with a planner, but the full AI application logic is outside the SDK's scope.

## 5. (Future) MCP (Model Context Protocol) Integration - Conceptual

*   **MCP Server:** If this integration were to be pursued, an MCP Server application would need to be built. This server would host the Semantic Kernel and expose the ClickUp plugins over the MCP.
*   **MCP Plugin Definition:** The existing Semantic Kernel plugins would likely be directly usable or require minimal adaptation to be exposed via MCP. MCP typically works with OpenAPI specifications for describing plugin capabilities. DocFX or other tools might be used to generate an OpenAPI spec for the Semantic Kernel plugins.
*   **Scope for this Plan:** This detailed plan focuses on creating the Semantic Kernel plugins. The MCP server implementation is a separate, larger effort and is only noted here as a future possibility as per `NEW_OVERALL_PLAN.md`.

## 6. Documentation

*   **`articles/semantic-kernel-integration.md`:**
    *   Introduction to using the Semantic Kernel plugins.
    *   How to add the `ClickUp.Api.Client.SemanticKernel` NuGet package.
    *   How to register the plugins with Semantic Kernel in a .NET application.
    *   List of available plugins and their key functions (with simplified descriptions).
    *   Basic examples of invoking a plugin function directly.
    *   A conceptual example of how a planner might use these functions (e.g., a user saying "Create a task in my 'Work' list called 'Follow up with client'").
    *   Link to official Semantic Kernel documentation for more advanced usage.

## 7. Testing

*   **Unit Tests for Plugins:**
    *   For each plugin function:
        *   Mock the underlying SDK service (e.g., `ITasksService`).
        *   Call the plugin function with various inputs.
        *   Verify that the correct SDK service method is called with correctly mapped parameters.
        *   Verify that the function returns the expected string/object based on mocked SDK responses (success and error cases).
        *   Test parameter validation or transformation logic within the plugin function.

## Plan Output

*   This document `11-SemanticKernelIntegration.md` contains the detailed plan.
*   It specifies the structure and design approach for Semantic Kernel plugins.
*   It identifies initial SDK services and methods to prioritize for wrapping.
*   It outlines the setup for the new Semantic Kernel integration project.
*   It provides guidance for documentation and testing of these plugins.
```
