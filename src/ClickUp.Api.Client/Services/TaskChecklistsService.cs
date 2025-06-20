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

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ITaskChecklistsService"/> for ClickUp CuTask Checklist operations.
    /// </summary>
    public class TaskChecklistsService : ITaskChecklistsService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskChecklistsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public TaskChecklistsService(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        }

        private string BuildQueryString(Dictionary<string, string?> queryParams)
        {
            if (queryParams == null || !queryParams.Any(kvp => kvp.Value != null))
            {
                return string.Empty;
            }

            var sb = new StringBuilder("?");
            foreach (var kvp in queryParams)
            {
                if (kvp.Value != null)
                {
                    if (sb.Length > 1) sb.Append('&');
                    sb.Append($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
            }
            return sb.ToString();
        }

        /// <inheritdoc />
        public async Task<CreateChecklistResponse> CreateChecklistAsync(
            string taskId,
            CreateChecklistRequest createChecklistRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"task/{taskId}/checklist";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

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
            var endpoint = $"checklist/{checklistId}";
            await _apiConnection.PutAsync(endpoint, editChecklistRequest, cancellationToken); // Interface returns void
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteChecklistAsync(
            string checklistId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"checklist/{checklistId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CreateChecklistItemResponse> CreateChecklistItemAsync(
            string checklistId,
            CreateChecklistItemRequest createChecklistItemRequest,
            CancellationToken cancellationToken = default)
        {
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
            var endpoint = $"checklist/{checklistId}/checklist_item/{checklistItemId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }
    }
}
