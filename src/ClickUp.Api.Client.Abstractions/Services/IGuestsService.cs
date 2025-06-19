using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{

    // Represents the Guests operations in the ClickUp API
    // Based on endpoints like:
    // - POST /v2/team/{team_id}/guest
    // - GET /v2/team/{team_id}/guest/{guest_id}
    // - PUT /v2/team/{team_id}/guest/{guest_id}
    // - DELETE /v2/team/{team_id}/guest/{guest_id}
    // - POST /v2/task/{task_id}/guest/{guest_id}
    // - DELETE /v2/task/{task_id}/guest/{guest_id}
    // - POST /v2/list/{list_id}/guest/{guest_id}
    // - DELETE /v2/list/{list_id}/guest/{guest_id}
    // - POST /v2/folder/{folder_id}/guest/{guest_id}
    // - DELETE /v2/folder/{folder_id}/guest/{guest_id}

    public interface IGuestsService
    {
        /// <summary>
        /// Invites a guest to join a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="inviteGuestRequest">Details for inviting the guest.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the invited guest within the team structure.</returns>
        Task<object> InviteGuestToWorkspaceAsync(double workspaceId, object inviteGuestRequest);
        // Note: inviteGuestRequest should be InviteGuestRequest. Return type should be a DTO representing the 'team' structure with member details from the response.

        /// <summary>
        /// Retrieves information about a specific guest in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <returns>Details of the guest.</returns>
        Task<object> GetGuestAsync(double workspaceId, double guestId);
        // Note: Return type should be GuestDetailsDto. The API returns an empty object for this, which seems unusual. Verify actual response structure. If empty, Task might be more appropriate or a custom success/failure result.

        /// <summary>
        /// Configures options for a guest in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="editGuestRequest">Details for editing the guest's permissions.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated guest details.</returns>
        Task<object> EditGuestOnWorkspaceAsync(double workspaceId, double guestId, object editGuestRequest);
        // Note: editGuestRequest should be EditGuestOnWorkspaceRequest. Return type should be a DTO representing the 'guest' structure from the response.

        /// <summary>
        /// Revokes a guest's access to a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="guestId">The ID of the guest to remove.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task<object> RemoveGuestFromWorkspaceAsync(double workspaceId, double guestId);
        // Note: API returns a 'team' object. Consider if Task is sufficient or if this DTO is needed.

        /// <summary>
        /// Shares a task with a guest.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="addGuestToItemRequest">Permission level for the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their shared items.</returns>
        Task<object> AddGuestToTaskAsync(string taskId, double guestId, object addGuestToItemRequest, bool? includeShared = null, bool? customTaskIds = null, double? teamId = null);
        // Note: addGuestToItemRequest should be AddGuestToItemRequest. Return type should be a DTO representing the 'guest' with 'shared' items structure.

        /// <summary>
        /// Revokes a guest's access to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their remaining shared items.</returns>
        Task<object> RemoveGuestFromTaskAsync(string taskId, double guestId, bool? includeShared = null, bool? customTaskIds = null, double? teamId = null);
        // Note: Return type should be a DTO representing the 'guest' with 'shared' items structure.

        /// <summary>
        /// Shares a List with a guest.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="addGuestToItemRequest">Permission level for the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their shared items.</returns>
        Task<object> AddGuestToListAsync(double listId, double guestId, object addGuestToItemRequest, bool? includeShared = null);
        // Note: addGuestToItemRequest should be AddGuestToItemRequest. Return type should be a DTO representing the 'guest' with 'shared' items structure.

        /// <summary>
        /// Revokes a guest's access to a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their remaining shared items.</returns>
        Task<object> RemoveGuestFromListAsync(double listId, double guestId, bool? includeShared = null);
        // Note: Return type should be a DTO representing the 'guest' with 'shared' items structure.

        /// <summary>
        /// Shares a Folder with a guest.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="addGuestToItemRequest">Permission level for the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their shared items.</returns>
        Task<object> AddGuestToFolderAsync(double folderId, double guestId, object addGuestToItemRequest, bool? includeShared = null);
        // Note: addGuestToItemRequest should be AddGuestToItemRequest. Return type should be a DTO representing the 'guest' with 'shared' items structure.

        /// <summary>
        /// Revokes a guest's access to a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="guestId">The ID of the guest.</param>
        /// <param name="includeShared">Optional. Exclude details of items shared with the guest by setting to false.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains details of the guest and their remaining shared items.</returns>
        Task<object> RemoveGuestFromFolderAsync(double folderId, double guestId, bool? includeShared = null);
        // Note: Return type should be a DTO representing the 'guest' with 'shared' items structure.
    }
}
