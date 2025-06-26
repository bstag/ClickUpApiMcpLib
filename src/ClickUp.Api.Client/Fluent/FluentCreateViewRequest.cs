using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Views;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentCreateViewRequest
{
    private readonly CreateViewRequest _request = new();

    private readonly string _containerId;
    private readonly IViewsService _viewsService;
    private readonly ViewContainerType _containerType;

    public enum ViewContainerType
    {
        Workspace,
        Space,
        Folder,
        List
    }

    public FluentCreateViewRequest(string containerId, IViewsService viewsService, ViewContainerType containerType)
    {
        _containerId = containerId;
        _viewsService = viewsService;
        _containerType = containerType;
        _request.Settings = new ViewSettings(); // Initialize settings
    }

    public FluentCreateViewRequest WithName(string name)
    {
        _request.Name = name;
        return this;
    }

    public FluentCreateViewRequest WithType(string type)
    {
        _request.Type = type;
        return this;
    }

    public FluentCreateViewRequest WithPrivate(bool @private)
    {
        _request.Settings = _request.Settings with { Sharing = @private ? "private" : "public" };
        return this;
    }

    public FluentCreateViewRequest WithParent(string parent)
    {
        // Parent is not directly in CreateViewRequest, it's part of the URL
        // This method might be removed or handled differently based on API usage
        return this;
    }

    public FluentCreateViewRequest WithGrouping(ViewGrouping grouping)
    {
        _request.Grouping = grouping;
        return this;
    }

    public FluentCreateViewRequest WithDivide(ViewDivide divide)
    {
        _request.Divide = divide;
        return this;
    }

    public FluentCreateViewRequest WithSorting(ViewSorting sorting)
    {
        _request.Sorting = sorting;
        return this;
    }

    public FluentCreateViewRequest WithFilters(ViewFilters filters)
    {
        _request.Filters = filters;
        return this;
    }

    public FluentCreateViewRequest WithColumns(ViewColumns columns)
    {
        _request.Columns = columns;
        return this;
    }

    public FluentCreateViewRequest WithTeamSidebar(ViewTeamSidebar teamSidebar)
    {
        _request.TeamSidebar = teamSidebar;
        return this;
    }

    public FluentCreateViewRequest WithSettings(ViewSettings settings)
    {
        _request.Settings = settings;
        return this;
    }

    public async Task<object> CreateAsync(CancellationToken cancellationToken = default)
    {
        return _containerType switch
        {
            ViewContainerType.Workspace => await _viewsService.CreateWorkspaceViewAsync(_containerId, _request, cancellationToken),
            ViewContainerType.Space => await _viewsService.CreateSpaceViewAsync(_containerId, _request, cancellationToken),
            ViewContainerType.Folder => await _viewsService.CreateFolderViewAsync(_containerId, _request, cancellationToken),
            ViewContainerType.List => await _viewsService.CreateListViewAsync(_containerId, _request, cancellationToken),
            _ => throw new System.ArgumentOutOfRangeException()
        };
    }
}