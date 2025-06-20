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
using ClickUp.Api.Client.Models.RequestModels.Guests;
using ClickUp.Api.Client.Models.ResponseModels.Guests; // Assuming GetGuestResponse etc.

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IGuestsService"/> for ClickUp Guest operations.
    /// </summary>
    public class GuestsService : IGuestsService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuestsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public GuestsService(IApiConnection apiConnection)
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
        public async Task<TeamMember?> InviteGuestToWorkspaceAsync(
            string workspaceId,
            InviteGuestToWorkspaceRequest inviteGuestRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/guest"; // team_id is workspaceId
            // API returns {"team": {"members": [{"user": ..., "invited_by": ...}]}} structure or similar.
            // Assuming TeamMember is the DTO for the relevant part of this response.
            // Or a more specific InviteGuestResponse might wrap this.
            // For now, let's assume a wrapper that gives us the TeamMember.
            var response = await _apiConnection.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestResponse>(endpoint, inviteGuestRequest, cancellationToken);
            return response?.InvitedMember; // Assuming InviteGuestResponse has an InvitedMember property of type TeamMember.
        }

        /// <inheritdoc />
        public async Task<Guest?> GetGuestAsync(
            string workspaceId,
            string guestId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/guest/{guestId}";
            // API for Get Guest on Team might return { "guest": {...} } or just the guest object.
            // Based on other similar GETs, let's assume it returns the Guest object directly or wrapped.
            // The original note stated "API returns an empty object for this", which is odd.
            // Assuming it returns the Guest DTO, possibly wrapped.
            var response = await _apiConnection.GetAsync<GetGuestResponse>(endpoint, cancellationToken);
            return response?.Guest;
        }

        /// <inheritdoc />
        public async Task<Guest?> EditGuestOnWorkspaceAsync(
            string workspaceId,
            string guestId,
            UpdateGuestRequest updateGuestRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/guest/{guestId}";
            // API returns {"guest": {...}}
            var response = await _apiConnection.PutAsync<UpdateGuestRequest, GetGuestResponse>(endpoint, updateGuestRequest, cancellationToken);
            return response?.Guest;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task RemoveGuestFromWorkspaceAsync(
            string workspaceId,
            string guestId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/guest/{guestId}";
            // API returns a 'team' object, but for a DELETE, we often don't need it.
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Guest?> AddGuestToTaskAsync(
            string taskId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            bool? customTaskIds = null,
            string? teamId = null, // This is workspaceId for the task context
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"task/{taskId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            // API returns {"guest": {...}, "shared": {...}}
            var response = await _apiConnection.PostAsync<AddGuestToItemRequest, GetGuestResponse>(endpoint, addGuestToItemRequest, cancellationToken);
            return response?.Guest; // Assuming GetGuestResponse contains the Guest and their shared items
        }

        /// <inheritdoc />
        public async Task<Guest?> RemoveGuestFromTaskAsync(
            string taskId,
            string guestId,
            bool? includeShared = null,
            bool? customTaskIds = null,
            string? teamId = null, // This is workspaceId for the task context
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"task/{taskId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            // API returns {"guest": {...}, "shared": {...}} or just confirms.
            // Using DeleteAsync<TResponse> as it might return the guest object.
            var response = await _apiConnection.DeleteAsync<GetGuestResponse>(endpoint, cancellationToken);
            return response?.Guest;
        }

        /// <inheritdoc />
        public async Task<Guest?> AddGuestToListAsync(
            string listId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"list/{listId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.PostAsync<AddGuestToItemRequest, GetGuestResponse>(endpoint, addGuestToItemRequest, cancellationToken);
            return response?.Guest;
        }

        /// <inheritdoc />
        public async Task<Guest?> RemoveGuestFromListAsync(
            string listId,
            string guestId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"list/{listId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.DeleteAsync<GetGuestResponse>(endpoint, cancellationToken);
            return response?.Guest;
        }

        /// <inheritdoc />
        public async Task<Guest?> AddGuestToFolderAsync(
            string folderId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"folder/{folderId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.PostAsync<AddGuestToItemRequest, GetGuestResponse>(endpoint, addGuestToItemRequest, cancellationToken);
            return response?.Guest;
        }

        /// <inheritdoc />
        public async Task<Guest?> RemoveGuestFromFolderAsync(
            string folderId,
            string guestId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"folder/{folderId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.DeleteAsync<GetGuestResponse>(endpoint, cancellationToken);
            return response?.Guest;
        }
    }
}
