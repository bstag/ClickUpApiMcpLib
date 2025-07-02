using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TimeEntriesFluentGetRequest
{
    private long? _startDate;
    private long? _endDate;
    private int? _assignee;
    private string? _taskId;
    private bool? _customTaskIds;
    private string? _teamIdForCustomTaskIds;
    private bool? _includeTaskTags;
    private bool? _includeLocationNames;
    private int? _page;

    private readonly string _workspaceId;
    private readonly ITimeTrackingService _timeTrackingService;

    public TimeEntriesFluentGetRequest(string workspaceId, ITimeTrackingService timeTrackingService)
    {
        _workspaceId = workspaceId;
        _timeTrackingService = timeTrackingService;
    }

    public TimeEntriesFluentGetRequest WithStartDate(long startDate)
    {
        _startDate = startDate;
        return this;
    }

    public TimeEntriesFluentGetRequest WithEndDate(long endDate)
    {
        _endDate = endDate;
        return this;
    }

    public TimeEntriesFluentGetRequest WithAssignee(int assignee)
    {
        _assignee = assignee;
        return this;
    }

    public TimeEntriesFluentGetRequest WithTaskId(string taskId)
    {
        _taskId = taskId;
        return this;
    }

    public TimeEntriesFluentGetRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public TimeEntriesFluentGetRequest WithTeamIdForCustomTaskIds(string teamIdForCustomTaskIds)
    {
        _teamIdForCustomTaskIds = teamIdForCustomTaskIds;
        return this;
    }

    public TimeEntriesFluentGetRequest WithIncludeTaskTags(bool includeTaskTags)
    {
        _includeTaskTags = includeTaskTags;
        return this;
    }

    public TimeEntriesFluentGetRequest WithIncludeLocationNames(bool includeLocationNames)
    {
        _includeLocationNames = includeLocationNames;
        return this;
    }

    public TimeEntriesFluentGetRequest WithPage(int pageNumber)
    {
        _page = pageNumber;
        return this;
    }

    public async Task<Models.Common.Pagination.IPagedResult<TimeEntry>> GetAsync(CancellationToken cancellationToken = default)
    {
        var request = new GetTimeEntriesRequest
        {
            StartDate = _startDate.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_startDate.Value) : (System.DateTimeOffset?)null,
            EndDate = _endDate.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_endDate.Value) : (System.DateTimeOffset?)null,
            Assignee = _assignee?.ToString(),
            TaskId = _taskId,
            IncludeTaskTags = _includeTaskTags,
            IncludeLocationNames = _includeLocationNames,
            Page = _page
        };

        return await _timeTrackingService.GetTimeEntriesAsync(
            _workspaceId,
            request,
            cancellationToken
        );
    }

    public IAsyncEnumerable<TimeEntry> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        var request = new GetTimeEntriesRequest
        {
            StartDate = _startDate.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_startDate.Value) : (System.DateTimeOffset?)null,
            EndDate = _endDate.HasValue ? System.DateTimeOffset.FromUnixTimeMilliseconds(_endDate.Value) : (System.DateTimeOffset?)null,
            Assignee = _assignee?.ToString(),
            TaskId = _taskId,
            IncludeTaskTags = _includeTaskTags,
            IncludeLocationNames = _includeLocationNames
        };

        return _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(
            _workspaceId,
            request,
            cancellationToken
        );
    }
}