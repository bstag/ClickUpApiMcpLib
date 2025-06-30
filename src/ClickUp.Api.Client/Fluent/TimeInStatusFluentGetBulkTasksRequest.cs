using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

using ClickUp.Api.Client.Models.RequestModels.Tasks; // Added for GetBulkTasksTimeInStatusRequest

public class TimeInStatusFluentGetBulkTasksRequest
{
    private readonly GetBulkTasksTimeInStatusRequest _requestDto; // Use the DTO
    private readonly ITasksService _tasksService;

    public TimeInStatusFluentGetBulkTasksRequest(IEnumerable<string> taskIds, ITasksService tasksService)
    {
        _requestDto = new GetBulkTasksTimeInStatusRequest(taskIds); // Initialize DTO with taskIds
        _tasksService = tasksService;
    }

    public TimeInStatusFluentGetBulkTasksRequest WithCustomTaskIds(bool customTaskIds)
    {
        _requestDto.CustomTaskIds = customTaskIds;
        return this;
    }

    public TimeInStatusFluentGetBulkTasksRequest WithTeamId(string teamId)
    {
        _requestDto.TeamId = teamId;
        return this;
    }

    public async Task<GetBulkTasksTimeInStatusResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _tasksService.GetBulkTasksTimeInStatusAsync(
            _requestDto, // Pass the DTO
            cancellationToken
        );
    }
}