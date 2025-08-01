using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;

namespace ClickUp.Api.Client.Abstractions.Services.Views
{
    /// <summary>
    /// Service interface for ClickUp View operations across different hierarchy levels.
    /// Handles view operations at workspace, space, folder, and list levels.
    /// </summary>
    /// <remarks>
    /// This interface follows the Interface Segregation Principle by focusing solely on view operations
    /// across different hierarchy levels (workspace, space, folder, list).
    /// Covered API Endpoints:
    /// - Workspace Views: `GET /v2/team/{team_id}/view`, `POST /v2/team/{team_id}/view`
    /// - Space Views: `GET /v2/space/{space_id}/view`, `POST /v2/space/{space_id}/view`
    /// - Folder Views: `GET /v2/folder/{folder_id}/view`, `POST /v2/folder/{folder_id}/view`
    /// - List Views: `GET /v2/list/{list_id}/view`, `POST /v2/list/{list_id}/view`
    /// </remarks>
    public interface IViewHierarchyService
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
    }
}