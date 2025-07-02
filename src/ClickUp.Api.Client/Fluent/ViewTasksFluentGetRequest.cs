using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Views;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class ViewTasksFluentGetRequest
{
    private readonly GetViewTasksRequest _requestDto = new();
    private readonly string _viewId;
    private readonly IViewsService _viewsService;

    public ViewTasksFluentGetRequest(string viewId, IViewsService viewsService)
    {
        _viewId = viewId;
        _viewsService = viewsService;
    }

    /// <summary>
    /// Sets the page number for pagination.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (0-indexed).</param>
    /// <returns>The fluent request builder.</returns>
    public ViewTasksFluentGetRequest Page(int pageNumber)
    {
        _requestDto.Page = pageNumber;
        return this;
    }

    /// <summary>
    /// Executes the request to get tasks from the view.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged result of tasks.</returns>
    public async Task<IPagedResult<CuTask>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return await _viewsService.GetViewTasksAsync(_viewId, _requestDto, cancellationToken);
    }
}
