using System; // For DateTimeOffset
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common.ValueObjects; // For TimeRange
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.Common.Pagination; // For IPagedResult

namespace ClickUp.Api.Client.Fluent;

public class TimeEntriesFluentGetRequest
{
    private TimeRange? _timeRange;
    private long? _assigneeUserId; // Changed from int? _assignee to long? _assigneeUserId
    private string? _taskId;
    // CustomTaskIds and TeamIdForCustomTaskIds are not general filters for GetTimeEntries,
    // they are usually for specific create/update operations if a task ID is involved.
    // We'll remove them here as they are not on the refactored GetTimeEntriesAsync service method.
    // private bool? _customTaskIds;
    // private string? _teamIdForCustomTaskIds;
    private bool? _includeTaskTags;
    private bool? _includeLocationNames;
    private int? _page;
    private string? _spaceId;
    private string? _folderId;
    private string? _listId;

    private readonly string _workspaceId;
    private readonly ITimeTrackingService _timeTrackingService;

    public TimeEntriesFluentGetRequest(string workspaceId, ITimeTrackingService timeTrackingService)
    {
        _workspaceId = workspaceId;
        _timeTrackingService = timeTrackingService;
    }

    public TimeEntriesFluentGetRequest WithTimeRange(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        _timeRange = new TimeRange(startDate, endDate);
        return this;
    }

    public TimeEntriesFluentGetRequest WithAssignee(long assigneeUserId) // Changed from int to long
    {
        _assigneeUserId = assigneeUserId;
        return this;
    }

    public TimeEntriesFluentGetRequest ForTask(string taskId)
    {
        _taskId = taskId;
        _listId = null; _folderId = null; _spaceId = null; // Ensure exclusivity
        return this;
    }

    public TimeEntriesFluentGetRequest ForList(string listId)
    {
        _listId = listId;
        _taskId = null; _folderId = null; _spaceId = null; // Ensure exclusivity
        return this;
    }

    public TimeEntriesFluentGetRequest ForFolder(string folderId)
    {
        _folderId = folderId;
        _taskId = null; _listId = null; _spaceId = null; // Ensure exclusivity
        return this;
    }

    public TimeEntriesFluentGetRequest ForSpace(string spaceId)
    {
        _spaceId = spaceId;
        _taskId = null; _listId = null; _folderId = null; // Ensure exclusivity
        return this;
    }

    public TimeEntriesFluentGetRequest WithIncludeTaskTags(bool includeTaskTags = true)
    {
        _includeTaskTags = includeTaskTags;
        return this;
    }

    public TimeEntriesFluentGetRequest WithIncludeLocationNames(bool includeLocationNames = true)
    {
        _includeLocationNames = includeLocationNames;
        return this;
    }

    public TimeEntriesFluentGetRequest WithPage(int pageNumber)
    {
        _page = pageNumber;
        return this;
    }

    public async Task<IPagedResult<TimeEntry>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _timeTrackingService.GetTimeEntriesAsync(
            _workspaceId,
            _timeRange,
            _assigneeUserId,
            _taskId,
            _listId,
            _folderId,
            _spaceId,
            _includeTaskTags,
            _includeLocationNames,
            _page,
            cancellationToken
        );
    }

    public IAsyncEnumerable<TimeEntry> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        // The service method GetTimeEntriesAsyncEnumerableAsync was also updated to take these parameters.
        return _timeTrackingService.GetTimeEntriesAsyncEnumerableAsync(
            _workspaceId,
            _timeRange,
            _assigneeUserId,
            _taskId,
            _listId,
            _folderId,
            _spaceId,
            _includeTaskTags,
            _includeLocationNames,
            // Page is handled by the enumerable itself
            cancellationToken
        );
    }
}