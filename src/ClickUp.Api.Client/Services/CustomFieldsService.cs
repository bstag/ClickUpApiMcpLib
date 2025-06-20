using System;
using System.Collections.Generic;
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.CustomFields; // For Field
using ClickUp.Api.Client.Models.RequestModels.CustomFields;
using ClickUp.Api.Client.Models.ResponseModels.CustomFields; // Assuming GetCustomFieldsResponse exists
using System.Linq; // For Enumerable.Empty

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ICustomFieldsService"/> for ClickUp Custom Field operations.
    /// </summary>
    public class CustomFieldsService : ICustomFieldsService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFieldsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public CustomFieldsService(IApiConnection apiConnection)
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
        public async Task<IEnumerable<Field>> GetAccessibleCustomFieldsAsync(
            string listId,
            CancellationToken cancellationToken = default)
        {
            // This endpoint retrieves all fields accessible by a List, including those from parent Folders, Spaces, and the Workspace.
            var endpoint = $"list/{listId}/field";
            var response = await _apiConnection.GetAsync<GetCustomFieldsResponse>(endpoint, cancellationToken); // API returns {"fields": [...]}
            return response?.Fields ?? Enumerable.Empty<Field>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Field>> GetFolderCustomFieldsAsync(
            string folderId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"folder/{folderId}/field";
            var response = await _apiConnection.GetAsync<GetCustomFieldsResponse>(endpoint, cancellationToken); // API returns {"fields": [...]}
            return response?.Fields ?? Enumerable.Empty<Field>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Field>> GetSpaceCustomFieldsAsync(
            string spaceId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"space/{spaceId}/field";
            var response = await _apiConnection.GetAsync<GetCustomFieldsResponse>(endpoint, cancellationToken); // API returns {"fields": [...]}
            return response?.Fields ?? Enumerable.Empty<Field>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Field>> GetWorkspaceCustomFieldsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/field"; // team_id is workspaceId
            var response = await _apiConnection.GetAsync<GetCustomFieldsResponse>(endpoint, cancellationToken); // API returns {"fields": [...]}
            return response?.Fields ?? Enumerable.Empty<Field>();
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task SetCustomFieldValueAsync(
            string taskId,
            string fieldId,
            SetCustomFieldValueRequest setFieldValueRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"task/{taskId}/field/{fieldId}";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            // API returns 200 with an empty object.
            await _apiConnection.PostAsync(endpoint, setFieldValueRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task RemoveCustomFieldValueAsync(
            string taskId,
            string fieldId,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"task/{taskId}/field/{fieldId}";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            // API returns 200 with an empty object.
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }
    }
}
