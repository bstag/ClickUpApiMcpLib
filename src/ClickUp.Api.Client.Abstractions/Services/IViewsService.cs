using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{

    // Represents the Views operations in the ClickUp API
    // Based on endpoints like:
    // - GET /v2/team/{team_id}/view
    // - POST /v2/team/{team_id}/view
    // - GET /v2/space/{space_id}/view
    // - POST /v2/space/{space_id}/view
    // - GET /v2/folder/{folder_id}/view
    // - POST /v2/folder/{folder_id}/view
    // - GET /v2/list/{list_id}/view
    // - POST /v2/list/{list_id}/view
    // - GET /v2/view/{view_id}
    // - PUT /v2/view/{view_id}
    // - DELETE /v2/view/{view_id}
    // - GET /v2/view/{view_id}/task

    public interface IViewsService
    {
        /// <summary>
        /// Retrieves Views at the Workspace (Everything) level.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <returns>A list of Views at the Workspace level.</returns>
        Task<IEnumerable<object>> GetWorkspaceViewsAsync(double workspaceId);
        // Note: Return type should be IEnumerable<ViewDto>.

        /// <summary>
        /// Creates a new View at the Workspace (Everything) level.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="createViewRequest">Details of the View to create.</param>
        /// <returns>The created View.</returns>
        Task<object> CreateWorkspaceViewAsync(double workspaceId, object createViewRequest);
        // Note: createViewRequest should be CreateViewRequest, return type should be ViewDto.

        /// <summary>
        /// Retrieves Views for a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <returns>A list of Views in the Space.</returns>
        Task<IEnumerable<object>> GetSpaceViewsAsync(double spaceId);
        // Note: Return type should be IEnumerable<ViewDto>.

        /// <summary>
        /// Creates a new View in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="createViewRequest">Details of the View to create.</param>
        /// <returns>The created View.</returns>
        Task<object> CreateSpaceViewAsync(double spaceId, object createViewRequest);
        // Note: createViewRequest should be CreateViewRequest, return type should be ViewDto.

        /// <summary>
        /// Retrieves Views for a specific Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <returns>A list of Views in the Folder.</returns>
        Task<IEnumerable<object>> GetFolderViewsAsync(double folderId);
        // Note: Return type should be IEnumerable<ViewDto>.

        /// <summary>
        /// Creates a new View in a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="createViewRequest">Details of the View to create.</param>
        /// <returns>The created View.</returns>
        Task<object> CreateFolderViewAsync(double folderId, object createViewRequest);
        // Note: createViewRequest should be CreateViewRequest, return type should be ViewDto.

        /// <summary>
        /// Retrieves Views for a specific List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <returns>A list of Views in the List.</returns>
        Task<IEnumerable<object>> GetListViewsAsync(double listId);
        // Note: Return type should be IEnumerable<ViewDto>.

        /// <summary>
        /// Creates a new View in a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="createViewRequest">Details of the View to create.</param>
        /// <returns>The created View.</returns>
        Task<object> CreateListViewAsync(double listId, object createViewRequest);
        // Note: createViewRequest should be CreateViewRequest, return type should be ViewDto.

        /// <summary>
        /// Retrieves details of a specific View.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <returns>Details of the View.</returns>
        Task<object> GetViewAsync(string viewId);
        // Note: Return type should be ViewDto.

        /// <summary>
        /// Updates a View.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <param name="updateViewRequest">Details for updating the View.</param>
        /// <returns>The updated View.</returns>
        Task<object> UpdateViewAsync(string viewId, object updateViewRequest);
        // Note: updateViewRequest should be UpdateViewRequestDto, return type should be ViewDto.

        /// <summary>
        /// Deletes a View.
        /// </summary>
        /// <param name="viewId">The ID of the View to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteViewAsync(string viewId);
        // Note: API returns 200 with an empty object.

        /// <summary>
        /// Retrieves tasks visible in a specific View.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <param name="page">The page number to retrieve (0-indexed).</param>
        /// <returns>A list of tasks in the View.</returns>
        Task<IEnumerable<object>> GetViewTasksAsync(string viewId, int page);
        // Note: Return type should be a DTO that includes 'tasks' and 'last_page' (e.g. GetViewTasksResponseDto). Individual task objects should be ViewTaskDto/TaskDto.
    }
}
