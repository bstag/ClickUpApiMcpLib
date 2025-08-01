using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines the contract for querying and creating views across different ClickUp contexts.
    /// Implements the Single Responsibility Principle by focusing only on view querying and context-specific creation operations.
    /// </summary>
    public interface IViewQueryService
    {
        /// <summary>
        /// Retrieves all views in a workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the workspace.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the views response.</returns>
        Task<GetViewsResponse> GetWorkspaceViewsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new view in a workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the workspace.</param>
        /// <param name="createViewRequest">The request containing the view data to create.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created view response.</returns>
        Task<CreateTeamViewResponse> CreateWorkspaceViewAsync(
            string workspaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all views in a space.
        /// </summary>
        /// <param name="spaceId">The ID of the space.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the views response.</returns>
        Task<GetViewsResponse> GetSpaceViewsAsync(
            string spaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new view in a space.
        /// </summary>
        /// <param name="spaceId">The ID of the space.</param>
        /// <param name="createViewRequest">The request containing the view data to create.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created view response.</returns>
        Task<CreateSpaceViewResponse> CreateSpaceViewAsync(
            string spaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all views in a folder.
        /// </summary>
        /// <param name="folderId">The ID of the folder.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the views response.</returns>
        Task<GetViewsResponse> GetFolderViewsAsync(
            string folderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new view in a folder.
        /// </summary>
        /// <param name="folderId">The ID of the folder.</param>
        /// <param name="createViewRequest">The request containing the view data to create.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created view response.</returns>
        Task<CreateFolderViewResponse> CreateFolderViewAsync(
            string folderId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all views in a list.
        /// </summary>
        /// <param name="listId">The ID of the list.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the views response.</returns>
        Task<GetViewsResponse> GetListViewsAsync(
            string listId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new view in a list.
        /// </summary>
        /// <param name="listId">The ID of the list.</param>
        /// <param name="createViewRequest">The request containing the view data to create.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created view response.</returns>
        Task<CreateListViewResponse> CreateListViewAsync(
            string listId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default);
    }
}