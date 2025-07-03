using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common.ValueObjects;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Parameters; // Added for GetTimeEntriesRequestParameters

namespace ClickUp.Api.Client.Fluent;

public class TimeEntriesFluentGetRequest
{
    private readonly string _workspaceId;
    private readonly ITimeTrackingService _timeTrackingService;
    private readonly GetTimeEntriesRequestParameters _parameters = new();

    public TimeEntriesFluentGetRequest(string workspaceId, ITimeTrackingService timeTrackingService)
    {
        _workspaceId = workspaceId;
        _timeTrackingService = timeTrackingService;
    }

    public TimeEntriesFluentGetRequest WithTimeRange(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        _parameters.TimeRange = new TimeRange(startDate, endDate);
        return this;
    }

    public TimeEntriesFluentGetRequest WithAssignee(long assigneeUserId)
    {
        _parameters.AssigneeUserId = assigneeUserId;
        return this;
    }

    public TimeEntriesFluentGetRequest ForTask(string taskId)
    {
        _parameters.TaskId = taskId;
        _parameters.ListId = null;
        _parameters.FolderId = null;
        _parameters.SpaceId = null; // Ensure exclusivity
        return this;
    }

    public TimeEntriesFluentGetRequest ForList(string listId)
    {
        _parameters.ListId = listId;
        _parameters.TaskId = null;
        _parameters.FolderId = null;
        _parameters.SpaceId = null; // Ensure exclusivity
        return this;
    }

    public TimeEntriesFluentGetRequest ForFolder(string folderId)
    {
        _parameters.FolderId = folderId;
        _parameters.TaskId = null;
        _parameters.ListId = null;
        _parameters.SpaceId = null; // Ensure exclusivity
        return this;
    }

    public TimeEntriesFluentGetRequest ForSpace(string spaceId)
    {
        _parameters.SpaceId = spaceId;
        _parameters.TaskId = null;
        _parameters.ListId = null;
        _parameters.FolderId = null; // Ensure exclusivity
        return this;
    }

    public TimeEntriesFluentGetRequest WithIncludeTaskTags(bool includeTaskTags = true)
    {
        _parameters.IncludeTaskTags = includeTaskTags;
        return this;
    }

    public TimeEntriesFluentGetRequest WithIncludeLocationNames(bool includeLocationNames = true)
    {
        _parameters.IncludeLocationNames = includeLocationNames;
        return this;
    }

    public TimeEntriesFluentGetRequest WithPage(int pageNumber)
    {
        _parameters.Page = pageNumber;
        return this;
    }

    public async Task<IPagedResult<TimeEntry>> GetAsync(CancellationToken cancellationToken = default)
    {
        // Option 1: Pass parameters directly if service accepts it
        // return await _timeTrackingService.GetTimeEntriesAsync(_workspaceId, _parameters, cancellationToken);

        // Option 2: Configure using Action delegate
        return await _timeTrackingService.GetTimeEntriesAsync(_workspaceId, config =>
        {
            config.TimeRange = _parameters.TimeRange;
            config.AssigneeUserId = _parameters.AssigneeUserId;
            config.TaskId = _parameters.TaskId;
            config.ListId = _parameters.ListId;
            config.FolderId = _parameters.FolderId;
            config.SpaceId = _parameters.SpaceId;
            config.IncludeTaskTags = _parameters.IncludeTaskTags;
            config.IncludeLocationNames = _parameters.IncludeLocationNames;
            config.Page = _parameters.Page;
        }, cancellationToken);
    }

    public IAsyncEnumerable<TimeEntry> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        // The service method GetTimeEntriesAsyncEnumerableAsync was updated to take GetTimeEntriesRequestParameters directly.
        // We need to ensure the _parameters instance is correctly populated.
        // The 'Page' property within _parameters will be ignored by the service layer for async enumerable.
        return _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(
            _workspaceId,
            _parameters,
            cancellationToken
        );
    }
}