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
        Task<Guest> RemoveGuestFromFolderAsync(
            string folderId,
            string guestId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default);
    }
}
