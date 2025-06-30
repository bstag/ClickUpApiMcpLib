using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Views;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.ResponseModels.Views;

public class ViewsFluentApi
{
    private readonly IViewsService _viewsService;

    public ViewsFluentApi(IViewsService viewsService)
    {
        _viewsService = viewsService;
    }

    public async Task<GetViewsResponse> GetWorkspaceViewsAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        return await _viewsService.GetWorkspaceViewsAsync(workspaceId, cancellationToken);
    }

    public async Task<GetViewsResponse> GetSpaceViewsAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        return await _viewsService.GetSpaceViewsAsync(spaceId, cancellationToken);
    }

    public async Task<GetViewsResponse> GetFolderViewsAsync(string folderId, CancellationToken cancellationToken = default)
    {
        return await _viewsService.GetFolderViewsAsync(folderId, cancellationToken);
    }

    public async Task<GetViewsResponse> GetListViewsAsync(string listId, CancellationToken cancellationToken = default)
    {
        return await _viewsService.GetListViewsAsync(listId, cancellationToken);
    }

    public async Task<GetViewResponse> GetViewAsync(string viewId, CancellationToken cancellationToken = default)
    {
        return await _viewsService.GetViewAsync(viewId, cancellationToken);
    }

    public async Task DeleteViewAsync(string viewId, CancellationToken cancellationToken = default)
    {
        await _viewsService.DeleteViewAsync(viewId, cancellationToken);
    }

    public ViewFluentCreateRequest CreateWorkspaceView(string workspaceId)
    {
        return new ViewFluentCreateRequest(workspaceId, _viewsService, ViewFluentCreateRequest.ViewContainerType.Workspace);
    }

    public ViewFluentCreateRequest CreateSpaceView(string spaceId)
    {
        return new ViewFluentCreateRequest(spaceId, _viewsService, ViewFluentCreateRequest.ViewContainerType.Space);
    }

    public ViewFluentCreateRequest CreateFolderView(string folderId)
    {
        return new ViewFluentCreateRequest(folderId, _viewsService, ViewFluentCreateRequest.ViewContainerType.Folder);
    }

    public ViewFluentCreateRequest CreateListView(string listId)
    {
        return new ViewFluentCreateRequest(listId, _viewsService, ViewFluentCreateRequest.ViewContainerType.List);
    }

    public ViewFluentUpdateRequest UpdateView(string viewId)
    {
        return new ViewFluentUpdateRequest(viewId, _viewsService);
    }

    public async Task<GetViewTasksResponse> GetViewTasksAsync(string viewId, int page, CancellationToken cancellationToken = default)
    {
        return await _viewsService.GetViewTasksAsync(viewId, page, cancellationToken);
    }

    /// <summary>
    /// Retrieves all Workspace (Everything level) views asynchronously.
    /// The underlying API is not paginated for listing views.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace (team).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="View"/>.</returns>
    public async IAsyncEnumerable<View> GetWorkspaceViewsAsyncEnumerableAsync(
        string workspaceId,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _viewsService.GetWorkspaceViewsAsync(workspaceId, cancellationToken).ConfigureAwait(false);
        if (response?.Views != null)
        {
            foreach (var view in response.Views)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return view;
            }
        }
    }

    /// <summary>
    /// Retrieves all views for a specific Space asynchronously.
    /// The underlying API is not paginated for listing views.
    /// </summary>
    /// <param name="spaceId">The ID of the Space.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="View"/>.</returns>
    public async IAsyncEnumerable<View> GetSpaceViewsAsyncEnumerableAsync(
        string spaceId,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _viewsService.GetSpaceViewsAsync(spaceId, cancellationToken).ConfigureAwait(false);
        if (response?.Views != null)
        {
            foreach (var view in response.Views)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return view;
            }
        }
    }

    /// <summary>
    /// Retrieves all views for a specific Folder asynchronously.
    /// The underlying API is not paginated for listing views.
    /// </summary>
    /// <param name="folderId">The ID of the Folder.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="View"/>.</returns>
    public async IAsyncEnumerable<View> GetFolderViewsAsyncEnumerableAsync(
        string folderId,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _viewsService.GetFolderViewsAsync(folderId, cancellationToken).ConfigureAwait(false);
        if (response?.Views != null)
        {
            foreach (var view in response.Views)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return view;
            }
        }
    }

    /// <summary>
    /// Retrieves all views for a specific List asynchronously.
    /// The underlying API is not paginated for listing views.
    /// </summary>
    /// <param name="listId">The ID of the List.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="View"/>.</returns>
    public async IAsyncEnumerable<View> GetListViewsAsyncEnumerableAsync(
        string listId,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _viewsService.GetListViewsAsync(listId, cancellationToken).ConfigureAwait(false);
        if (response?.Views != null)
        {
            foreach (var view in response.Views)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return view;
            }
        }
    }
}
