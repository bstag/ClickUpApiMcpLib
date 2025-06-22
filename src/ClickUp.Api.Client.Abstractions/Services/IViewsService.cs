using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Views; // Assuming View and CuTask DTOs are here
using ClickUp.Api.Client.Models.RequestModels.Views; // Assuming Request DTOs are here
using ClickUp.Api.Client.Models.ResponseModels.Views; // Assuming GetViewTasksResponse is here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Views operations in the ClickUp API.
    /// This service allows for creating, retrieving, updating, and deleting views,
    /// as well as fetching tasks within a specific view.
    /// </summary>
    /// <remarks>
    /// Views can be associated with Workspaces (Teams), Spaces, Folders, or Lists.
    /// Based on ClickUp API endpoints like:
    /// <list type="bullet">
    /// <item><description>GET /v2/team/{team_id}/view</description></item>
    /// <item><description>POST /v2/team/{team_id}/view</description></item>
    /// <item><description>GET /v2/space/{space_id}/view</description></item>
    /// <item><description>POST /v2/space/{space_id}/view</description></item>
    /// <item><description>GET /v2/folder/{folder_id}/view</description></item>
    /// <item><description>POST /v2/folder/{folder_id}/view</description></item>
    /// <item><description>GET /v2/list/{list_id}/view</description></item>
    /// <item><description>POST /v2/list/{list_id}/view</description></item>
    /// <item><description>GET /v2/view/{view_id}</description></item>
    /// <item><description>PUT /v2/view/{view_id}</description></item>
    /// <item><description>DELETE /v2/view/{view_id}</description></item>
    /// <item><description>GET /v2/view/{view_id}/task</description></item>
    /// </list>
    /// </remarks>
    public interface IViewsService
    {
        /// <summary>
        /// Retrieves all Views at the Workspace (Team/Everything) level.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetViewsResponse"/> object with a list of views.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<GetViewsResponse> GetWorkspaceViewsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new View at the Workspace (Team/Everything) level.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="createViewRequest">A <see cref="CreateViewRequest"/> object containing details for the new view, such as its name, type, and settings.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateTeamViewResponse"/> object with the created view's details.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createViewRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<CreateTeamViewResponse> CreateWorkspaceViewAsync(
            string workspaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all Views for a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetViewsResponse"/> object with a list of views in the Space.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<GetViewsResponse> GetSpaceViewsAsync(
            string spaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new View within a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="createViewRequest">A <see cref="CreateViewRequest"/> object containing details for the new view.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateSpaceViewResponse"/> object with the created view's details.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="createViewRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<CreateSpaceViewResponse> CreateSpaceViewAsync(
            string spaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all Views for a specific Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetViewsResponse"/> object with a list of views in the Folder.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="folderId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<GetViewsResponse> GetFolderViewsAsync(
            string folderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new View within a specific Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="createViewRequest">A <see cref="CreateViewRequest"/> object containing details for the new view.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateFolderViewResponse"/> object with the created view's details.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="folderId"/> or <paramref name="createViewRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<CreateFolderViewResponse> CreateFolderViewAsync(
            string folderId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all Views for a specific List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetViewsResponse"/> object with a list of views in the List.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<GetViewsResponse> GetListViewsAsync(
            string listId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new View within a specific List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="createViewRequest">A <see cref="CreateViewRequest"/> object containing details for the new view.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CreateListViewResponse"/> object with the created view's details.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="createViewRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<CreateListViewResponse> CreateListViewAsync(
            string listId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves details of a specific View by its ID.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetViewResponse"/> object with the view's details.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="viewId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as view not found.</exception>
        Task<GetViewResponse> GetViewAsync(
            string viewId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing View.
        /// </summary>
        /// <param name="viewId">The ID of the View to update.</param>
        /// <param name="updateViewRequest">An <see cref="UpdateViewRequest"/> object containing the updated details for the view.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="UpdateViewResponse"/> object with the updated view's details.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="viewId"/> or <paramref name="updateViewRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<UpdateViewResponse> UpdateViewAsync(
            string viewId,
            UpdateViewRequest updateViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a View.
        /// </summary>
        /// <param name="viewId">The ID of the View to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="viewId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        System.Threading.Tasks.Task DeleteViewAsync(
            string viewId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves tasks that are visible within a specific View, with pagination.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <param name="page">The page number of results to retrieve (0-indexed). The ClickUp API requires this parameter for this endpoint.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetViewTasksResponse"/> object with a list of tasks and pagination information.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="viewId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<GetViewTasksResponse> GetViewTasksAsync(
            string viewId,
            int page,
            CancellationToken cancellationToken = default);
    }
}
