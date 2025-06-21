using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Users; // Assuming Guest DTO is here
using ClickUp.Api.Client.Models.RequestModels.Guests; // Assuming Request DTOs are here
using ClickUp.Api.Client.Models.ResponseModels.Guests; // For InviteGuestToWorkspaceResponse

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Guests operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - POST /v2/team/{team_id}/guest
    /// - GET /v2/team/{team_id}/guest/{guest_id}
    /// - PUT /v2/team/{team_id}/guest/{guest_id}
    /// - DELETE /v2/team/{team_id}/guest/{guest_id}
    /// - POST /v2/task/{task_id}/guest/{guest_id}
    /// - etc.
    /// </remarks>
    public interface IGuestsService
    {
        /// <summary>
        /// Invites a guest to join a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="inviteGuestRequest">Details for inviting the guest.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the invited guest, typically including the user/guest object and their role via <see cref="InviteGuestToWorkspaceResponse"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="inviteGuestRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the workspace with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiValidationException">Thrown if the invitation request is invalid (e.g., invalid email or permissions).</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to invite guests to this workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<InviteGuestToWorkspaceResponse> InviteGuestToWorkspaceAsync(
            string workspaceId,
            InviteGuestToWorkspaceRequest inviteGuestRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves information about a specific guest in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="GetGuestResponse"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="guestId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the workspace or guest with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this guest's information.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<GetGuestResponse> GetGuestAsync(
            string workspaceId,
            string guestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Configures options or updates details for a guest in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="updateGuestRequest">Details for editing the guest's permissions or properties using <see cref="EditGuestOnWorkspaceRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Guest"/> details.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="guestId"/>, or <paramref name="updateGuestRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the workspace or guest with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiValidationException">Thrown if the update request is invalid.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to edit this guest.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Guest> EditGuestOnWorkspaceAsync(
            string workspaceId,
            string guestId,
            EditGuestOnWorkspaceRequest updateGuestRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a guest's access to a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="guestId">The ID of the guest to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <remarks>Original note mentioned API returns a 'team' object. Returning CuTask for simplicity unless team DTO is essential for client.</remarks>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="guestId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the workspace or guest with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove this guest from the workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task RemoveGuestFromWorkspaceAsync(
            string workspaceId,
            string guestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shares a task with a guest.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="addGuestToItemRequest">Permission level and other details for adding the guest to the task.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the <see cref="Guest"/> and their shared items.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/>, <paramref name="guestId"/>, or <paramref name="addGuestToItemRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or guest with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiValidationException">Thrown if the request to add the guest is invalid (e.g., invalid permissions).</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to add this guest to the task.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Guest> AddGuestToTaskAsync(
            string taskId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a guest's access to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the <see cref="Guest"/> and their remaining shared items.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="guestId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or guest with the specified IDs are not found, or the guest is not associated with the task.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove this guest from the task.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Guest> RemoveGuestFromTaskAsync(
            string taskId,
            string guestId,
            bool? includeShared = null,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shares a List with a guest.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="addGuestToItemRequest">Permission level for the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the <see cref="Guest"/> and their shared items.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/>, <paramref name="guestId"/>, or <paramref name="addGuestToItemRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the list or guest with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiValidationException">Thrown if the request to add the guest is invalid.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to add this guest to the list.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Guest> AddGuestToListAsync(
            string listId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a guest's access to a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the <see cref="Guest"/> and their remaining shared items.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="guestId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the list or guest with the specified IDs are not found, or the guest is not associated with the list.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove this guest from the list.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Guest> RemoveGuestFromListAsync(
            string listId,
            string guestId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shares a Folder with a guest.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="addGuestToItemRequest">Permission level for the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the <see cref="Guest"/> and their shared items.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/>, <paramref name="guestId"/>, or <paramref name="addGuestToItemRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the folder or guest with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiValidationException">Thrown if the request to add the guest is invalid.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to add this guest to the folder.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Guest> AddGuestToFolderAsync(
            string folderId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a guest's access to a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the <see cref="Guest"/> and their remaining shared items.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> or <paramref name="guestId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the folder or guest with the specified IDs are not found, or the guest is not associated with the folder.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove this guest from the folder.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Guest> RemoveGuestFromFolderAsync(
            string folderId,
            string guestId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);
    }
}
