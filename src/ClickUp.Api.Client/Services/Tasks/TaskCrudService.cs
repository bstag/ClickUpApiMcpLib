using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Plugins;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Plugins;
using ClickUp.Api.Client.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services.Tasks
{
    /// <summary>
    /// Handles basic CRUD operations for ClickUp tasks.
    /// Implements the Single Responsibility Principle by focusing only on task creation, reading, updating, and deletion.
    /// </summary>
    public class TaskCrudService : ITaskCrudService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<TaskCrudService> _logger;
        private readonly IPluginManager? _pluginManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCrudService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <param name="pluginManager">The plugin manager for executing plugins (optional).</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public TaskCrudService(IApiConnection apiConnection, ILogger<TaskCrudService> logger, IPluginManager? pluginManager = null)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<TaskCrudService>.Instance;
            _pluginManager = pluginManager;
        }

        /// <inheritdoc />
        public async Task<CuTask> CreateTaskAsync(
            string listId,
            CreateTaskRequest createTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(listId, nameof(listId));
            if (createTaskRequest == null) throw new ArgumentNullException(nameof(createTaskRequest));
            ValidationHelper.ValidateRequiredString(createTaskRequest.Name, nameof(createTaskRequest.Name));
            if (!string.IsNullOrEmpty(teamId)) ValidationHelper.ValidateId(teamId, nameof(teamId));

            await ExecuteBeforePluginsAsync("CreateTask", new { ListId = listId, Request = createTaskRequest }, cancellationToken);
            
            _logger.LogInformation("Creating task in list ID: {ListId} with name: {TaskName}", listId, createTaskRequest.Name);
            
            var endpoint = $"list/{listId}/task";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            var task = await _apiConnection.PostAsync<CreateTaskRequest, CuTask>(endpoint, createTaskRequest, cancellationToken);
            if (task == null)
            {
                throw new InvalidOperationException($"API connection returned null response when creating task in list {listId}.");
            }
            
            await ExecuteAfterPluginsAsync("CreateTask", new { ListId = listId, Request = createTaskRequest }, task, cancellationToken);
            
            _logger.LogInformation("Successfully created task {TaskId} in list {ListId}", task.Id, listId);
            return task;
        }

        /// <inheritdoc />
        public async Task<CuTask> GetTaskAsync(
            string taskId,
            GetTaskRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(taskId, nameof(taskId));
            if (requestModel == null) throw new ArgumentNullException(nameof(requestModel));

            _logger.LogInformation("Getting task with ID: {TaskId}", taskId);
            
            var endpoint = $"task/{taskId}";
            var queryParams = new Dictionary<string, string?>();
            if (requestModel.CustomTaskIds.HasValue) queryParams["custom_task_ids"] = requestModel.CustomTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParams["team_id"] = requestModel.TeamId;
            if (requestModel.IncludeSubtasks.HasValue) queryParams["include_subtasks"] = requestModel.IncludeSubtasks.Value.ToString().ToLower();
            if (requestModel.IncludeMarkdownDescription.HasValue) queryParams["include_markdown_description"] = requestModel.IncludeMarkdownDescription.Value.ToString().ToLower();
            if (requestModel.Page.HasValue) queryParams["page"] = requestModel.Page.Value.ToString();
            endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            var task = await _apiConnection.GetAsync<CuTask>(endpoint, cancellationToken);
            if (task == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting task {taskId}.");
            }
            
            _logger.LogDebug("Successfully retrieved task {TaskId}", taskId);
            return task;
        }

        /// <inheritdoc />
        public async Task<CuTask> UpdateTaskAsync(
            string taskId,
            UpdateTaskRequest updateTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(taskId, nameof(taskId));
            if (updateTaskRequest == null) throw new ArgumentNullException(nameof(updateTaskRequest));
            if (!string.IsNullOrEmpty(teamId)) ValidationHelper.ValidateId(teamId, nameof(teamId));

            _logger.LogInformation("Updating task with ID: {TaskId}", taskId);
            
            var endpoint = $"task/{taskId}";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            var task = await _apiConnection.PutAsync<UpdateTaskRequest, CuTask>(endpoint, updateTaskRequest, cancellationToken);
            if (task == null)
            {
                throw new InvalidOperationException($"API connection returned null response when updating task {taskId}.");
            }
            
            _logger.LogInformation("Successfully updated task {TaskId}", taskId);
            return task;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteTaskAsync(
            string taskId,
            DeleteTaskRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(taskId, nameof(taskId));
            if (requestModel == null) throw new ArgumentNullException(nameof(requestModel));
            if (!string.IsNullOrEmpty(requestModel.TeamId)) ValidationHelper.ValidateId(requestModel.TeamId, nameof(requestModel.TeamId));

            _logger.LogInformation("Deleting task with ID: {TaskId}", taskId);
            
            var endpoint = $"task/{taskId}";
            var queryParams = new Dictionary<string, string?>();
            if (requestModel.CustomTaskIds.HasValue) queryParams["custom_task_ids"] = requestModel.CustomTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParams["team_id"] = requestModel.TeamId;
            endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
            _logger.LogInformation("Successfully deleted task {TaskId}", taskId);
        }

        /// <inheritdoc />
        public async Task<CuTask> CreateTaskFromTemplateAsync(
            string listId,
            string templateId,
            CreateTaskFromTemplateRequest createTaskFromTemplateRequest,
            CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(listId, nameof(listId));
            ValidationHelper.ValidateId(templateId, nameof(templateId));
            if (createTaskFromTemplateRequest == null) throw new ArgumentNullException(nameof(createTaskFromTemplateRequest));

            _logger.LogInformation("Creating task in list ID: {ListId} from template ID: {TemplateId}", listId, templateId);
            
            var endpoint = $"list/{listId}/taskTemplate/{templateId}";

            var task = await _apiConnection.PostAsync<CreateTaskFromTemplateRequest, CuTask>(endpoint, createTaskFromTemplateRequest, cancellationToken);
            if (task == null)
            {
                throw new InvalidOperationException($"API connection returned null response when creating task from template {templateId} in list {listId}.");
            }
            
            _logger.LogInformation("Successfully created task {TaskId} from template {TemplateId} in list {ListId}", task.Id, templateId, listId);
            return task;
        }

        /// <summary>
        /// Executes plugins before an API operation.
        /// </summary>
        /// <param name="operationType">The type of operation being performed.</param>
        /// <param name="requestData">The request data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async System.Threading.Tasks.Task ExecuteBeforePluginsAsync(string operationType, object? requestData = null, CancellationToken cancellationToken = default)
        {
            if (_pluginManager == null) return;

            try
            {
                var requestDict = requestData != null ? 
                    new Dictionary<string, object> { ["data"] = requestData } : 
                    new Dictionary<string, object>();
                    
                var context = new PluginContext(
                    _apiConnection,
                    operationType,
                    nameof(TaskCrudService),
                    requestDict);

                await _pluginManager.ExecutePluginsAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error executing before plugins for operation: {OperationType}", operationType);
            }
        }

        /// <summary>
        /// Executes plugins after an API operation.
        /// </summary>
        /// <param name="operationType">The type of operation being performed.</param>
        /// <param name="requestData">The request data.</param>
        /// <param name="responseData">The response data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async System.Threading.Tasks.Task ExecuteAfterPluginsAsync(string operationType, object? requestData = null, object? responseData = null, CancellationToken cancellationToken = default)
        {
            if (_pluginManager == null) return;

            try
            {
                var requestDict = requestData != null ? 
                    new Dictionary<string, object> { ["data"] = requestData } : 
                    new Dictionary<string, object>();
                    
                var responseDict = responseData != null ? 
                    new Dictionary<string, object> { ["data"] = responseData } : 
                    new Dictionary<string, object>();
                    
                var context = new PluginContext(
                    _apiConnection,
                    operationType,
                    nameof(TaskCrudService),
                    requestDict,
                    responseDict);

                await _pluginManager.ExecutePluginsAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error executing after plugins for operation: {OperationType}", operationType);
            }
        }
    }
}