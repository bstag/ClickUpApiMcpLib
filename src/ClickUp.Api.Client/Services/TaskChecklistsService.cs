using System;
using System.Collections.Generic; // For Dictionary
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Checklists;
using ClickUp.Api.Client.Models.RequestModels.Checklists;
using ClickUp.Api.Client.Models.ResponseModels.Checklists; // For specific response DTOs
using System.Linq; // For Enumerable.Empty (though not used in this specific refactor but good practice)
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUp.Api.Client.Helpers;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ITaskChecklistsService"/> for ClickUp CuTask Checklist operations.
    /// </summary>
    public class TaskChecklistsService : ITaskChecklistsService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<TaskChecklistsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskChecklistsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public TaskChecklistsService(IApiConnection apiConnection, ILogger<TaskChecklistsService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<TaskChecklistsService>.Instance;
        }


        /// <inheritdoc />
        public async Task<CreateChecklistResponse> CreateChecklistAsync(
            string taskId,
            CreateChecklistRequest createChecklistRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating checklist for task ID: {TaskId}, Name: {ChecklistName}", taskId, createChecklistRequest.Name);
            var endpoint = $"task/{taskId}/checklist";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            var response = await _apiConnection.PostAsync<CreateChecklistRequest, CreateChecklistResponse>(endpoint, createChecklistRequest, cancellationToken);
            if (response?.Checklist == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty Checklist data when creating checklist for task {taskId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task EditChecklistAsync(
            string checklistId,
            EditChecklistRequest editChecklistRequest, // Changed from UpdateChecklistRequest
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Editing checklist ID: {ChecklistId}, Name: {ChecklistName}", checklistId, editChecklistRequest.Name);
            var endpoint = $"checklist/{checklistId}";
            await _apiConnection.PutAsync(endpoint, editChecklistRequest, cancellationToken); // Interface returns void
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteChecklistAsync(
            string checklistId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting checklist ID: {ChecklistId}", checklistId);
            var endpoint = $"checklist/{checklistId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CreateChecklistItemResponse> CreateChecklistItemAsync(
            string checklistId,
            CreateChecklistItemRequest createChecklistItemRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating checklist item in checklist ID: {ChecklistId}, Name: {ItemName}", checklistId, createChecklistItemRequest.Name);
            var endpoint = $"checklist/{checklistId}/checklist_item";
            var response = await _apiConnection.PostAsync<CreateChecklistItemRequest, CreateChecklistItemResponse>(endpoint, createChecklistItemRequest, cancellationToken);
            if (response?.Checklist == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty Checklist data when creating checklist item for checklist {checklistId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<EditChecklistItemResponse> EditChecklistItemAsync(
            string checklistId,
            string checklistItemId,
            EditChecklistItemRequest editChecklistItemRequest, // Changed from UpdateChecklistItemRequest
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Editing checklist item ID: {ChecklistItemId} in checklist ID: {ChecklistId}, Name: {ItemName}", checklistItemId, checklistId, editChecklistItemRequest.Name);
            var endpoint = $"checklist/{checklistId}/checklist_item/{checklistItemId}";
            var response = await _apiConnection.PutAsync<EditChecklistItemRequest, EditChecklistItemResponse>(endpoint, editChecklistItemRequest, cancellationToken);
            if (response?.Checklist == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty Checklist data when editing checklist item {checklistItemId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteChecklistItemAsync(
            string checklistId,
            string checklistItemId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting checklist item ID: {ChecklistItemId} from checklist ID: {ChecklistId}", checklistItemId, checklistId);
            var endpoint = $"checklist/{checklistId}/checklist_item/{checklistItemId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }
    }
}
