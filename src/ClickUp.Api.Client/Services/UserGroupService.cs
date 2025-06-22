using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.UserGroups;
using ClickUp.Api.Client.Models.RequestModels.UserGroups;
using ClickUp.Api.Client.Models.ResponseModels.UserGroups; // For GetUserGroupsResponse
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IUserGroupsService"/> for ClickUp User Group (Team) operations.
    /// </summary>
    public class UserGroupsService : IUserGroupsService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<UserGroupsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public UserGroupsService(IApiConnection apiConnection, ILogger<UserGroupsService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<UserGroupsService>.Instance;
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
        public async Task<IEnumerable<UserGroup>> GetUserGroupsAsync(
            string workspaceId,
            List<string>? groupIds = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting user groups for workspace ID: {WorkspaceId}, Group IDs: {GroupIds}", workspaceId, groupIds != null ? string.Join(",", groupIds) : "All");
            var endpoint = $"team/{workspaceId}/group";
            var queryParams = new Dictionary<string, string?>();
            if (groupIds != null && groupIds.Any())
            {
                queryParams["group_ids"] = string.Join(",", groupIds);
            }
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetUserGroupsResponse>(endpoint, cancellationToken);
            return response?.Groups ?? Enumerable.Empty<UserGroup>();
        }

        /// <inheritdoc />
        public async Task<UserGroup> CreateUserGroupAsync(
            string workspaceId,
            CreateUserGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating user group in workspace ID: {WorkspaceId}, Name: {GroupName}", workspaceId, request.Name);
            var endpoint = $"team/{workspaceId}/group";
            // The API for Create Group (POST /team/{team_id}/group) returns the created UserGroup object directly.
            var userGroup = await _apiConnection.PostAsync<CreateUserGroupRequest, UserGroup>(endpoint, request, cancellationToken);
            if (userGroup == null)
            {
                throw new InvalidOperationException($"API connection returned null response when creating user group in workspace {workspaceId}.");
            }
            return userGroup;
        }

        /// <inheritdoc />
        public async Task<UserGroup> UpdateUserGroupAsync(
            string groupId,
            UpdateUserGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating user group ID: {GroupId}, Name: {GroupName}", groupId, request.Name);
            var endpoint = $"group/{groupId}";
            // The API for Update Group (PUT /group/{group_id}) returns the updated UserGroup object directly.
            var userGroup = await _apiConnection.PutAsync<UpdateUserGroupRequest, UserGroup>(endpoint, request, cancellationToken);
            if (userGroup == null)
            {
                throw new InvalidOperationException($"API connection returned null response when updating user group {groupId}.");
            }
            return userGroup;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteUserGroupAsync(
            string groupId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting user group ID: {GroupId}", groupId);
            var endpoint = $"group/{groupId}";
            // API returns an empty JSON object {} on success.
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }
    }
}
