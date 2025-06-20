using System;
using System.Collections.Generic; // For Dictionary
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.TaskChecklists;
using ClickUp.Api.Client.Models.ResponseModels; // For potential wrapper DTOs

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ITaskChecklistsService"/> for ClickUp Task Checklist operations.
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
        public async Task<Checklist?> CreateChecklistAsync(
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

            // API returns {"checklist": {...}}
            var response = await _apiConnection.PostAsync<CreateChecklistRequest, GetChecklistResponse>(endpoint, createChecklistRequest, cancellationToken);
            return response?.Checklist;
        }

        /// <inheritdoc />
        public async Task<Checklist?> EditChecklistAsync(
            string checklistId,
            UpdateChecklistRequest updateChecklistRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"checklist/{checklistId}";
            // API returns {"checklist": {...}}
            var response = await _apiConnection.PutAsync<UpdateChecklistRequest, GetChecklistResponse>(endpoint, updateChecklistRequest, cancellationToken);
            return response?.Checklist;
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
        public async Task<Checklist?> CreateChecklistItemAsync(
            string checklistId,
            CreateChecklistItemRequest createChecklistItemRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"checklist/{checklistId}/checklist_item";
            // API returns {"checklist": {...}} containing the PARENT checklist
            var response = await _apiConnection.PostAsync<CreateChecklistItemRequest, GetChecklistResponse>(endpoint, createChecklistItemRequest, cancellationToken);
            return response?.Checklist;
        }

        /// <inheritdoc />
        public async Task<Checklist?> EditChecklistItemAsync(
            string checklistId,
            string checklistItemId,
            UpdateChecklistItemRequest updateChecklistItemRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"checklist/{checklistId}/checklist_item/{checklistItemId}";
            // API returns {"checklist": {...}} containing the PARENT checklist
            var response = await _apiConnection.PutAsync<UpdateChecklistItemRequest, GetChecklistResponse>(endpoint, updateChecklistItemRequest, cancellationToken);
            return response?.Checklist;
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
