using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities; // Assuming View and Task DTOs are here
using ClickUp.Api.Client.Models.RequestModels.Views; // Assuming Request DTOs are here
using ClickUp.Api.Client.Models.ResponseModels.Views; // Assuming GetViewTasksResponse is here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Views operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/team/{team_id}/view
    /// - POST /v2/team/{team_id}/view
    /// - GET /v2/space/{space_id}/view
    /// - POST /v2/space/{space_id}/view
    /// - GET /v2/folder/{folder_id}/view
    /// - POST /v2/folder/{folder_id}/view
    /// - GET /v2/list/{list_id}/view
    /// - POST /v2/list/{list_id}/view
    /// - GET /v2/view/{view_id}
    /// - PUT /v2/view/{view_id}
    /// - DELETE /v2/view/{view_id}
    /// - GET /v2/view/{view_id}/task
    /// </remarks>
    public interface IViewsService
    {
        /// <summary>
        /// Retrieves Views at the Workspace (Everything) level.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="View"/> objects at the Workspace level.</returns>
        Task<IEnumerable<View>> GetWorkspaceViewsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new View at the Workspace (Everything) level.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="createViewRequest">Details of the View to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="View"/>.</returns>
        Task<View> CreateWorkspaceViewAsync(
            string workspaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves Views for a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="View"/> objects in the Space.</returns>
        Task<IEnumerable<View>> GetSpaceViewsAsync(
            string spaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new View in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="createViewRequest">Details of the View to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="View"/>.</returns>
        Task<View> CreateSpaceViewAsync(
            string spaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves Views for a specific Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="View"/> objects in the Folder.</returns>
        Task<IEnumerable<View>> GetFolderViewsAsync(
            string folderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new View in a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="createViewRequest">Details of the View to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="View"/>.</returns>
        Task<View> CreateFolderViewAsync(
            string folderId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves Views for a specific List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="View"/> objects in the List.</returns>
        Task<IEnumerable<View>> GetListViewsAsync(
            string listId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new View in a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="createViewRequest">Details of the View to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="View"/>.</returns>
        Task<View> CreateListViewAsync(
            string listId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves details of a specific View.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="View"/>.</returns>
        Task<View> GetViewAsync(
            string viewId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a View.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <param name="updateViewRequest">Details for updating the View.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="View"/>.</returns>
        Task<View> UpdateViewAsync(
            string viewId,
            UpdateViewRequest updateViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a View.
        /// </summary>
        /// <param name="viewId">The ID of the View to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task DeleteViewAsync(
            string viewId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves tasks visible in a specific View.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <param name="page">The page number to retrieve (0-indexed).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetViewTasksResponse"/> object containing a list of tasks and pagination details.</returns>
        Task<GetViewTasksResponse> GetViewTasksAsync(
            string viewId,
            int page,
            CancellationToken cancellationToken = default);
    }
}
