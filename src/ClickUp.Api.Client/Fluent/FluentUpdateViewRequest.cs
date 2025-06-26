using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Views;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentUpdateViewRequest
{
    private string? _name;
    private string? _type;
    private ViewGrouping? _grouping;
    private ViewDivide? _divide;
    private ViewSorting? _sorting;
    private ViewFilters? _filters;
    private ViewColumns? _columns;
    private ViewTeamSidebar? _teamSidebar;
    private ViewSettings? _settings;

    private readonly string _viewId;
    private readonly IViewsService _viewsService;

    public FluentUpdateViewRequest(string viewId, IViewsService viewsService)
    {
        _viewId = viewId;
        _viewsService = viewsService;
    }

    public FluentUpdateViewRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public FluentUpdateViewRequest WithType(string type)
    {
        _type = type;
        return this;
    }

    public FluentUpdateViewRequest WithGrouping(ViewGrouping grouping)
    {
        _grouping = grouping;
        return this;
    }

    public FluentUpdateViewRequest WithDivide(ViewDivide divide)
    {
        _divide = divide;
        return this;
    }

    public FluentUpdateViewRequest WithSorting(ViewSorting sorting)
    {
        _sorting = sorting;
        return this;
    }

    public FluentUpdateViewRequest WithFilters(ViewFilters filters)
    {
        _filters = filters;
        return this;
    }

    public FluentUpdateViewRequest WithColumns(ViewColumns columns)
    {
        _columns = columns;
        return this;
    }

    public FluentUpdateViewRequest WithTeamSidebar(ViewTeamSidebar teamSidebar)
    {
        _teamSidebar = teamSidebar;
        return this;
    }

    public FluentUpdateViewRequest WithSettings(ViewSettings settings)
    {
        _settings = settings;
        return this;
    }

    public async Task<UpdateViewResponse> UpdateAsync(CancellationToken cancellationToken = default)
    {
        var updateViewRequest = new UpdateViewRequest
        {
            Name = _name ?? string.Empty,
            Type = _type ?? string.Empty,
            Grouping = _grouping ?? new ViewGrouping(),
            Divide = _divide ?? new ViewDivide(),
            Sorting = _sorting ?? new ViewSorting(),
            Filters = _filters ?? new ViewFilters(),
            Columns = _columns ?? new ViewColumns(),
            TeamSidebar = _teamSidebar ?? new ViewTeamSidebar(),
            Settings = _settings
        };

        return await _viewsService.UpdateViewAsync(
            _viewId,
            updateViewRequest,
            cancellationToken
        );
    }
}