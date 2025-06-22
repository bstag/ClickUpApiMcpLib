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
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Guests;
using ClickUp.Api.Client.Models.ResponseModels.Guests; // Assuming GetGuestResponse etc.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IGuestsService"/> for ClickUp Guest operations.
    /// </summary>
    public class GuestsService : IGuestsService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<GuestsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuestsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public GuestsService(IApiConnection apiConnection, ILogger<GuestsService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<GuestsService>.Instance;
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
        public async Task<InviteGuestToWorkspaceResponse> InviteGuestToWorkspaceAsync(
            string workspaceId,
            InviteGuestToWorkspaceRequest inviteGuestRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Inviting guest to workspace ID: {WorkspaceId}, Email: {GuestEmail}", workspaceId, inviteGuestRequest.Email);
            var endpoint = $"team/{workspaceId}/guest"; // team_id is workspaceId
            var response = await _apiConnection.PostAsync<InviteGuestToWorkspaceRequest, InviteGuestToWorkspaceResponse>(endpoint, inviteGuestRequest, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when inviting guest to workspace {workspaceId}.");
            }
            // Assuming InviteGuestToWorkspaceResponse directly models the {"team": {...}} structure.
            // If response.Team is null or response.Team.Members is what's needed, further checks might be required here
            // depending on the exact structure of InviteGuestToWorkspaceResponse and what the caller expects.
            // For now, returning the whole response as per interface.
            return response;
        }

        /// <inheritdoc />
        public async Task<GetGuestResponse> GetGuestAsync(
            string workspaceId,
            string guestId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting guest ID: {GuestId} for workspace ID: {WorkspaceId}", guestId, workspaceId);
            var endpoint = $"team/{workspaceId}/guest/{guestId}";
            var response = await _apiConnection.GetAsync<GetGuestResponse>(endpoint, cancellationToken);
            if (response == null) // The interface expects GetGuestResponse, which wraps Guest.
            {
                throw new InvalidOperationException($"API connection returned null response when getting guest {guestId} for workspace {workspaceId}.");
            }
            if (response.Guest == null) // Additional check if GetGuestResponse can exist with a null Guest
            {
                 throw new InvalidOperationException($"API response for guest {guestId} did not contain guest data.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<Guest> EditGuestOnWorkspaceAsync(
            string workspaceId,
            string guestId,
            EditGuestOnWorkspaceRequest updateGuestRequest, // Changed from UpdateGuestRequest
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Editing guest ID: {GuestId} on workspace ID: {WorkspaceId}", guestId, workspaceId);
            var endpoint = $"team/{workspaceId}/guest/{guestId}";
            var responseWrapper = await _apiConnection.PutAsync<EditGuestOnWorkspaceRequest, GetGuestResponse>(endpoint, updateGuestRequest, cancellationToken);
            if (responseWrapper?.Guest == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty Guest object when editing guest {guestId} for workspace {workspaceId}.");
            }
            return responseWrapper.Guest;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task RemoveGuestFromWorkspaceAsync(
            string workspaceId,
            string guestId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing guest ID: {GuestId} from workspace ID: {WorkspaceId}", guestId, workspaceId);
            var endpoint = $"team/{workspaceId}/guest/{guestId}";
            // API returns a 'team' object, but for a DELETE, we often don't need it.
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Guest> AddGuestToTaskAsync(
            string taskId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            bool? customTaskIds = null,
            string? teamId = null, // This is workspaceId for the task context
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding guest ID: {GuestId} to task ID: {TaskId}", guestId, taskId);
            var endpoint = $"task/{taskId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.PostAsync<AddGuestToItemRequest, GetGuestResponse>(endpoint, addGuestToItemRequest, cancellationToken);
            if (responseWrapper?.Guest == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty Guest object when adding guest {guestId} to task {taskId}.");
            }
            return responseWrapper.Guest;
        }

        /// <inheritdoc />
        public async Task<Guest> RemoveGuestFromTaskAsync(
            string taskId,
            string guestId,
            bool? includeShared = null,
            bool? customTaskIds = null,
            string? teamId = null, // This is workspaceId for the task context
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing guest ID: {GuestId} from task ID: {TaskId}", guestId, taskId);
            var endpoint = $"task/{taskId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.DeleteAsync<GetGuestResponse>(endpoint, cancellationToken);
            if (responseWrapper?.Guest == null)
            {
                // Note: DELETE operations might return empty or just a success status.
                // If API guarantees to return the Guest object on successful removal, this exception is fine.
                // Otherwise, if an empty success is possible, the interface might need to be Task or Task<bool>.
                throw new InvalidOperationException($"API connection returned null or empty Guest object when removing guest {guestId} from task {taskId}.");
            }
            return responseWrapper.Guest;
        }

        /// <inheritdoc />
        public async Task<Guest> AddGuestToListAsync(
            string listId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding guest ID: {GuestId} to list ID: {ListId}", guestId, listId);
            var endpoint = $"list/{listId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.PostAsync<AddGuestToItemRequest, GetGuestResponse>(endpoint, addGuestToItemRequest, cancellationToken);
            if (responseWrapper?.Guest == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty Guest object when adding guest {guestId} to list {listId}.");
            }
            return responseWrapper.Guest;
        }

        /// <inheritdoc />
        public async Task<Guest> RemoveGuestFromListAsync(
            string listId,
            string guestId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing guest ID: {GuestId} from list ID: {ListId}", guestId, listId);
            var endpoint = $"list/{listId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.DeleteAsync<GetGuestResponse>(endpoint, cancellationToken);
            if (responseWrapper?.Guest == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty Guest object when removing guest {guestId} from list {listId}.");
            }
            return responseWrapper.Guest;
        }

        /// <inheritdoc />
        public async Task<Guest> AddGuestToFolderAsync(
            string folderId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding guest ID: {GuestId} to folder ID: {FolderId}", guestId, folderId);
            var endpoint = $"folder/{folderId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.PostAsync<AddGuestToItemRequest, GetGuestResponse>(endpoint, addGuestToItemRequest, cancellationToken);
            if (responseWrapper?.Guest == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty Guest object when adding guest {guestId} to folder {folderId}.");
            }
            return responseWrapper.Guest;
        }

        /// <inheritdoc />
        public async Task<Guest> RemoveGuestFromFolderAsync(
            string folderId,
            string guestId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing guest ID: {GuestId} from folder ID: {FolderId}", guestId, folderId);
            var endpoint = $"folder/{folderId}/guest/{guestId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeShared.HasValue) queryParams["include_shared"] = includeShared.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.DeleteAsync<GetGuestResponse>(endpoint, cancellationToken);
            if (responseWrapper?.Guest == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty Guest object when removing guest {guestId} from folder {folderId}.");
            }
            return responseWrapper.Guest;
        }
    }
}
