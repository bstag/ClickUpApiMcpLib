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
using ClickUp.Api.Client.Models.RequestModels.Users;
using ClickUp.Api.Client.Models.ResponseModels.Users; // Assuming GetUserResponse exists

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IUsersService"/> for ClickUp User operations within a Workspace.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IApiConnection _apiConnection;
        private const string BaseWorkspaceEndpoint = "team"; // ClickUp v2 uses "team/{team_id}" for workspace context

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public UsersService(IApiConnection apiConnection)
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
        public async Task<User?> GetUserFromWorkspaceAsync(
            string workspaceId,
            string userId,
            bool? includeShared = null, // This parameter is not standard for GET user. Might be specific to certain ClickUp contexts or older versions.
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/user/{userId}";
            var queryParams = new Dictionary<string, string?>();
            // The 'include_shared' parameter is not listed in the standard GET /team/{team_id}/user/{user_id} endpoint.
            // If it's valid for a specific use case, it would be added here.
            // if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            // API usually returns {"user": {...}}
            var response = await _apiConnection.GetAsync<GetUserResponse>(endpoint, cancellationToken);
            return response?.User;
        }

        /// <inheritdoc />
        public async Task<User?> EditUserOnWorkspaceAsync(
            string workspaceId,
            string userId,
            EditUserOnWorkspaceRequest editUserRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/user/{userId}";
            // API usually returns {"user": {...}}
            var response = await _apiConnection.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(endpoint, editUserRequest, cancellationToken);
            return response?.User;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task RemoveUserFromWorkspaceAsync(
            string workspaceId,
            string userId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/user/{userId}";
            // API returns a 'team' object, but interface is void.
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }
    }
}
