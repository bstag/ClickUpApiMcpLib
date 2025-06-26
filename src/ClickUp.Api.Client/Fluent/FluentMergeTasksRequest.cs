using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentMergeTasksRequest
{
    private List<string>? _sourceTaskIds;

    private readonly string _targetTaskId;
    private readonly ITasksService _tasksService;

    public FluentMergeTasksRequest(string targetTaskId, ITasksService tasksService)
    {
        _targetTaskId = targetTaskId;
        _tasksService = tasksService;
    }

    public FluentMergeTasksRequest WithSourceTaskIds(List<string> sourceTaskIds)
    {
        _sourceTaskIds = sourceTaskIds;
        return this;
    }

    public async Task<CuTask> MergeAsync(CancellationToken cancellationToken = default)
    {
        var mergeTasksRequest = new MergeTasksRequest
        {
            SourceTaskIds = _sourceTaskIds ?? new List<string>()
        };

        return await _tasksService.MergeTasksAsync(
            _targetTaskId,
            mergeTasksRequest,
            cancellationToken
        );
    }
}