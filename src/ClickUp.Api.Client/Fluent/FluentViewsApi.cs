using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Views;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.ResponseModels.Views;

public class FluentViewsApi
{
    private readonly IViewsService _viewsService;

    public FluentViewsApi(IViewsService viewsService)
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

    public FluentCreateViewRequest CreateWorkspaceView(string workspaceId)
    {
        return new FluentCreateViewRequest(workspaceId, _viewsService, FluentCreateViewRequest.ViewContainerType.Workspace);
    }

    public FluentCreateViewRequest CreateSpaceView(string spaceId)
    {
        return new FluentCreateViewRequest(spaceId, _viewsService, FluentCreateViewRequest.ViewContainerType.Space);
    }

    public FluentCreateViewRequest CreateFolderView(string folderId)
    {
        return new FluentCreateViewRequest(folderId, _viewsService, FluentCreateViewRequest.ViewContainerType.Folder);
    }

    public FluentCreateViewRequest CreateListView(string listId)
    {
        return new FluentCreateViewRequest(listId, _viewsService, FluentCreateViewRequest.ViewContainerType.List);
    }

    public FluentUpdateViewRequest UpdateView(string viewId)
    {
        return new FluentUpdateViewRequest(viewId, _viewsService);
    }

    public async Task<GetViewTasksResponse> GetViewTasksAsync(string viewId, int page, CancellationToken cancellationToken = default)
    {
        return await _viewsService.GetViewTasksAsync(viewId, page, cancellationToken);
    }
}
