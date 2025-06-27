using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TimeInStatusFluentGetBulkTasksRequest
{
    private bool? _customTaskIds;
    private string? _teamId;

    private readonly IEnumerable<string> _taskIds;
    private readonly ITasksService _tasksService;

    public TimeInStatusFluentGetBulkTasksRequest(IEnumerable<string> taskIds, ITasksService tasksService)
    {
        _taskIds = taskIds;
        _tasksService = tasksService;
    }

    public TimeInStatusFluentGetBulkTasksRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public TimeInStatusFluentGetBulkTasksRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    public async Task<GetBulkTasksTimeInStatusResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetBulkTasksTimeInStatusAsync(
            _taskIds,
            _customTaskIds,
            _teamId,
            cancellationToken
        );
    }
}