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

    public TimeEntriesFluentGetRequest WithIncludeTimers(bool includeTimers = true)
    {
        _parameters.IncludeTimers = includeTimers;
        return this;
    }

    public TimeEntriesFluentGetRequest WithPage(int pageNumber)
    {
        _parameters.Page = pageNumber;
        return this;
    }

    public TimeEntriesFluentGetRequest WithCustomTaskIds(bool customTaskIds = true)
    {
        _parameters.CustomTaskIds = customTaskIds;
        return this;
    }

    public TimeEntriesFluentGetRequest WithTeamIdForCustomTaskIds(string teamId)
    {
        _parameters.TeamIdForCustomTaskIds = teamId;
        return this;
    }

    public async Task<IPagedResult<TimeEntry>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.GetTimeEntriesAsync(_workspaceId, CopyParametersFrom(_parameters), cancellationToken);
    }

    public IAsyncEnumerable<TimeEntry> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        return _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(_workspaceId, _parameters, cancellationToken);
    }

    private static Action<GetTimeEntriesRequestParameters> CopyParametersFrom(GetTimeEntriesRequestParameters source)
    {
        return target =>
        {
            target.TimeRange = source.TimeRange;
            target.AssigneeUserId = source.AssigneeUserId;
            target.TaskId = source.TaskId;
            target.ListId = source.ListId;
            target.FolderId = source.FolderId;
            target.SpaceId = source.SpaceId;
            target.IncludeTaskTags = source.IncludeTaskTags;
            target.IncludeLocationNames = source.IncludeLocationNames;
            target.Page = source.Page;
            target.IncludeTimers = source.IncludeTimers;
            target.CustomTaskIds = source.CustomTaskIds;
            target.TeamIdForCustomTaskIds = source.TeamIdForCustomTaskIds;
        };
    }
}