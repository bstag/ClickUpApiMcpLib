using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;
using Microsoft.Extensions.Logging;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IViewsService"/> for ClickUp View operations using the composite pattern.
    /// Delegates to specialized services following the Single Responsibility Principle.
    /// </summary>
    public class ViewsService : IViewsService
    {
        private readonly IViewCrudService _viewCrudService;
        private readonly IViewQueryService _viewQueryService;
        private readonly IViewTaskService _viewTaskService;
        private readonly ILogger<ViewsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewsService"/> class.
        /// </summary>
        /// <param name="viewCrudService">The service for view CRUD operations.</param>
        /// <param name="viewQueryService">The service for view querying and context-specific creation operations.</param>
        /// <param name="viewTaskService">The service for view task operations.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ViewsService(
            IViewCrudService viewCrudService,
            IViewQueryService viewQueryService,
            IViewTaskService viewTaskService,
            ILogger<ViewsService> logger)
        {
            _viewCrudService = viewCrudService ?? throw new ArgumentNullException(nameof(viewCrudService));
            _viewQueryService = viewQueryService ?? throw new ArgumentNullException(nameof(viewQueryService));
            _viewTaskService = viewTaskService ?? throw new ArgumentNullException(nameof(viewTaskService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <inheritdoc />
        public async Task<GetViewsResponse> GetWorkspaceViewsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetWorkspaceViewsAsync to ViewQueryService for workspace ID: {WorkspaceId}", workspaceId);
            return await _viewQueryService.GetWorkspaceViewsAsync(workspaceId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CreateTeamViewResponse> CreateWorkspaceViewAsync(
            string workspaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating CreateWorkspaceViewAsync to ViewQueryService for workspace ID: {WorkspaceId}", workspaceId);
            return await _viewQueryService.CreateWorkspaceViewAsync(workspaceId, createViewRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetViewsResponse> GetSpaceViewsAsync(
            string spaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetSpaceViewsAsync to ViewQueryService for space ID: {SpaceId}", spaceId);
            return await _viewQueryService.GetSpaceViewsAsync(spaceId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CreateSpaceViewResponse> CreateSpaceViewAsync(
            string spaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating CreateSpaceViewAsync to ViewQueryService for space ID: {SpaceId}", spaceId);
            return await _viewQueryService.CreateSpaceViewAsync(spaceId, createViewRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetViewsResponse> GetFolderViewsAsync(
            string folderId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetFolderViewsAsync to ViewQueryService for folder ID: {FolderId}", folderId);
            return await _viewQueryService.GetFolderViewsAsync(folderId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CreateFolderViewResponse> CreateFolderViewAsync(
            string folderId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating CreateFolderViewAsync to ViewQueryService for folder ID: {FolderId}", folderId);
            return await _viewQueryService.CreateFolderViewAsync(folderId, createViewRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetViewsResponse> GetListViewsAsync(
            string listId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetListViewsAsync to ViewQueryService for list ID: {ListId}", listId);
            return await _viewQueryService.GetListViewsAsync(listId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CreateListViewResponse> CreateListViewAsync(
            string listId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating CreateListViewAsync to ViewQueryService for list ID: {ListId}", listId);
            return await _viewQueryService.CreateListViewAsync(listId, createViewRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetViewResponse> GetViewAsync(
            string viewId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetViewAsync to ViewCrudService for view ID: {ViewId}", viewId);
            return await _viewCrudService.GetViewAsync(viewId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<UpdateViewResponse> UpdateViewAsync(
            string viewId,
            UpdateViewRequest updateViewRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating UpdateViewAsync to ViewCrudService for view ID: {ViewId}", viewId);
            return await _viewCrudService.UpdateViewAsync(viewId, updateViewRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteViewAsync(
            string viewId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating DeleteViewAsync to ViewCrudService for view ID: {ViewId}", viewId);
            await _viewCrudService.DeleteViewAsync(viewId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IPagedResult<CuTask>> GetViewTasksAsync(
            string viewId,
            GetViewTasksRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetViewTasksAsync to ViewTaskService for view ID: {ViewId}", viewId);
            return await _viewTaskService.GetViewTasksAsync(viewId, request, cancellationToken);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CuTask> GetViewTasksAsyncEnumerableAsync(
            string viewId,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetViewTasksAsyncEnumerableAsync to ViewTaskService for view ID: {ViewId}", viewId);
            await foreach (var task in _viewTaskService.GetViewTasksAsyncEnumerableAsync(viewId, cancellationToken))
            {
                yield return task;
            }
        }
    }
}
