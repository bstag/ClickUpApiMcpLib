using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Guests;
using ClickUp.Api.Client.Models.ResponseModels.Guests;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Guest management operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for inviting guests to Workspaces, managing their access to specific items (Tasks, Lists, Folders),
    /// retrieving guest information, and removing guests.
    /// Covered API Endpoints:
    /// - Workspace Guests: `POST /team/{team_id}/guest`, `GET /team/{team_id}/guest/{guest_id}`, `PUT /team/{team_id}/guest/{guest_id}`, `DELETE /team/{team_id}/guest/{guest_id}`
    /// - Task Guests: `POST /task/{task_id}/guest/{guest_id}`, `DELETE /task/{task_id}/guest/{guest_id}`
    /// - List Guests: `POST /list/{list_id}/guest/{guest_id}`, `DELETE /list/{list_id}/guest/{guest_id}`
    /// - Folder Guests: `POST /folder/{folder_id}/guest/{guest_id}`, `DELETE /folder/{folder_id}/guest/{guest_id}`
    /// </remarks>
    public interface IGuestsService
    {
        /// <summary>
        /// Invites a new guest to join a specific Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) to which the guest will be invited.</param>
        /// <param name="inviteGuestRequest">An object containing details for inviting the guest, such as their email and desired permission level.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="InviteGuestToWorkspaceResponse"/> object with details of the invited guest and their role.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="inviteGuestRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the invitation request is invalid (e.g., invalid email format, or permissions issues).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to invite guests to this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<InviteGuestToWorkspaceResponse> InviteGuestToWorkspaceAsync(
            string workspaceId,
            InviteGuestToWorkspaceRequest inviteGuestRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves information about a specific guest within a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="guestId">The unique identifier of the guest to retrieve.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetGuestResponse"/> object with the guest's details.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="guestId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace or guest with the specified IDs does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this guest's information.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetGuestResponse> GetGuestAsync(
            string workspaceId,
            string guestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the configuration or details for an existing guest within a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="guestId">The unique identifier of the guest to update.</param>
        /// <param name="updateGuestRequest">An object (<see cref="EditGuestOnWorkspaceRequest"/>) containing the properties to update for the guest, such as their permissions or name.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Guest"/> object with the new details.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="guestId"/>, or <paramref name="updateGuestRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace or guest with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the update request contains invalid data.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to edit this guest's details.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Guest> EditGuestOnWorkspaceAsync(
            string workspaceId,
            string guestId,
            EditGuestOnWorkspaceRequest updateGuestRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a guest's access to a specific Workspace (Team), effectively removing them from it.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) from which the guest will be removed.</param>
        /// <param name="guestId">The unique identifier of the guest to remove.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="guestId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace or guest with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove this guest from the Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task RemoveGuestFromWorkspaceAsync(
            string workspaceId,
            string guestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shares a specific task with a guest, granting them access with defined permissions.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task to share.</param>
        /// <param name="guestId">The unique identifier of the guest with whom to share the task.</param>
        /// <param name="addGuestToItemRequest">An object specifying the permission level and other details for adding the guest to the task.</param>
        /// <param name="includeShared">Optional. If set to <c>false</c>, excludes details of other items shared with the guest in the response. Defaults to <c>true</c>.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Guest"/> object, potentially including information about shared items.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/>, <paramref name="guestId"/>, or <paramref name="addGuestToItemRequest"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or guest with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the request to add the guest is invalid (e.g., invalid permission level).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to share this task with the guest.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Guest> AddGuestToTaskAsync(
            string taskId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a guest's access to a specific task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task from which the guest's access will be revoked.</param>
        /// <param name="guestId">The unique identifier of the guest whose access will be revoked.</param>
        /// <param name="includeShared">Optional. If set to <c>false</c>, excludes details of other items shared with the guest in the response. Defaults to <c>true</c>.</param>
        /// <param name="customTaskIds">Optional. If set to <c>true</c>, the <paramref name="taskId"/> is treated as a custom task ID. Defaults to <c>false</c>.</param>
        /// <param name="teamId">Optional. The Workspace ID (formerly team_id). This is required if <paramref name="customTaskIds"/> is <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Guest"/> object, potentially including information about their remaining shared items.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> or <paramref name="guestId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="customTaskIds"/> is true but <paramref name="teamId"/> is not provided.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the task or guest with the specified IDs does not exist, or if the guest is not currently associated with the task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove this guest from the task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Guest> RemoveGuestFromTaskAsync(
            string taskId,
            string guestId,
            bool? includeShared = null,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shares a specific List with a guest, granting them access with defined permissions.
        /// </summary>
        /// <param name="listId">The unique identifier of the List to share.</param>
        /// <param name="guestId">The unique identifier of the guest with whom to share the List.</param>
        /// <param name="addGuestToItemRequest">An object specifying the permission level for the guest on this List.</param>
        /// <param name="includeShared">Optional. If set to <c>false</c>, excludes details of other items shared with the guest in the response. Defaults to <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Guest"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/>, <paramref name="guestId"/>, or <paramref name="addGuestToItemRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List or guest with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the request to add the guest is invalid.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to share this List with the guest.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Guest> AddGuestToListAsync(
            string listId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a guest's access to a specific List.
        /// </summary>
        /// <param name="listId">The unique identifier of the List from which the guest's access will be revoked.</param>
        /// <param name="guestId">The unique identifier of the guest whose access will be revoked.</param>
        /// <param name="includeShared">Optional. If set to <c>false</c>, excludes details of other items shared with the guest in the response. Defaults to <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Guest"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="guestId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List or guest with the specified IDs does not exist, or if the guest is not associated with the List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove this guest from the List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Guest> RemoveGuestFromListAsync(
            string listId,
            string guestId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Shares a specific Folder with a guest, granting them access with defined permissions.
        /// </summary>
        /// <param name="folderId">The unique identifier of the Folder to share.</param>
        /// <param name="guestId">The unique identifier of the guest with whom to share the Folder.</param>
        /// <param name="addGuestToItemRequest">An object specifying the permission level for the guest on this Folder.</param>
        /// <param name="includeShared">Optional. If set to <c>false</c>, excludes details of other items shared with the guest in the response. Defaults to <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Guest"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/>, <paramref name="guestId"/>, or <paramref name="addGuestToItemRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Folder or guest with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the request to add the guest is invalid.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to share this Folder with the guest.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Guest> AddGuestToFolderAsync(
            string folderId,
            string guestId,
            AddGuestToItemRequest addGuestToItemRequest,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a guest's access to a specific Folder.
        /// </summary>
        /// <param name="folderId">The unique identifier of the Folder from which the guest's access will be revoked.</param>
        /// <param name="guestId">The unique identifier of the guest whose access will be revoked.</param>
        /// <param name="includeShared">Optional. If set to <c>false</c>, excludes details of other items shared with the guest in the response. Defaults to <c>true</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Guest"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> or <paramref name="guestId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Folder or guest with the specified IDs does not exist, or if the guest is not associated with the Folder.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to remove this guest from the Folder.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Guest> RemoveGuestFromFolderAsync(
            string folderId,
            string guestId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);
    }
}
