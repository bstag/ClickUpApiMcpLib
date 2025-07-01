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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ICustomFieldsService"/> for ClickUp Custom Field operations.
    /// </summary>
    public class CustomFieldsService : ICustomFieldsService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<CustomFieldsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFieldsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public CustomFieldsService(IApiConnection apiConnection, ILogger<CustomFieldsService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<CustomFieldsService>.Instance;
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
        public async Task<IEnumerable<CustomFieldDefinition>> GetAccessibleCustomFieldsAsync(
            string listId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting accessible custom fields for list ID: {ListId}", listId);
            // This endpoint retrieves all fields accessible by a List, including those from parent Folders, Spaces, and the Workspace.
            var endpoint = $"list/{listId}/field";
            var response = await _apiConnection.GetAsync<GetAccessibleCustomFieldsResponse>(endpoint, cancellationToken); // API returns {"fields": [...]}
            return response?.Fields ?? Enumerable.Empty<CustomFieldDefinition>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CustomFieldDefinition>> GetFolderCustomFieldsAsync(
            string folderId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting custom fields for folder ID: {FolderId}", folderId);
            var endpoint = $"folder/{folderId}/field";
            var response = await _apiConnection.GetAsync<GetAccessibleCustomFieldsResponse>(endpoint, cancellationToken); // API returns {"fields": [...]}
            return response?.Fields ?? Enumerable.Empty<CustomFieldDefinition>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CustomFieldDefinition>> GetSpaceCustomFieldsAsync(
            string spaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting custom fields for space ID: {SpaceId}", spaceId);
            var endpoint = $"space/{spaceId}/field";
            var response = await _apiConnection.GetAsync<GetAccessibleCustomFieldsResponse>(endpoint, cancellationToken); // API returns {"fields": [...]}
            return response?.Fields ?? Enumerable.Empty<CustomFieldDefinition>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CustomFieldDefinition>> GetWorkspaceCustomFieldsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting custom fields for workspace ID: {WorkspaceId}", workspaceId);
            var endpoint = $"team/{workspaceId}/field"; // team_id is workspaceId
            var response = await _apiConnection.GetAsync<GetAccessibleCustomFieldsResponse>(endpoint, cancellationToken); // API returns {"fields": [...]}
            return response?.Fields ?? Enumerable.Empty<CustomFieldDefinition>();
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
            _logger.LogInformation("Setting custom field value for task ID: {TaskId}, Field ID: {FieldId}", taskId, fieldId);
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
            _logger.LogInformation("Removing custom field value for task ID: {TaskId}, Field ID: {FieldId}", taskId, fieldId);
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
