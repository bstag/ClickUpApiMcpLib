using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentRemoveTagsFromTimeEntriesRequest
{
    private List<string>? _timeEntryIds;
    private List<string>? _tags;

    private readonly string _workspaceId;
    private readonly ITimeTrackingService _timeTrackingService;

    public FluentRemoveTagsFromTimeEntriesRequest(string workspaceId, ITimeTrackingService timeTrackingService)
    {
        _workspaceId = workspaceId;
        _timeTrackingService = timeTrackingService;
    }

    public FluentRemoveTagsFromTimeEntriesRequest WithTimeEntryIds(List<string> timeEntryIds)
    {
        _timeEntryIds = timeEntryIds;
        return this;
    }

    public FluentRemoveTagsFromTimeEntriesRequest WithTags(List<string> tags)
    {
        _tags = tags;
        return this;
    }

    public async Task RemoveAsync(CancellationToken cancellationToken = default)
    {
        var removeTagsRequest = new RemoveTagsFromTimeEntriesRequest(
            TimeEntryIds: _timeEntryIds ?? new List<string>(),
            Tags: _tags?.Select(t => new TimeTrackingTagDefinition(Name: t, TagFg: null, TagBg: null)).ToList() ?? new List<TimeTrackingTagDefinition>()
        );

        await _timeTrackingService.RemoveTagsFromTimeEntriesAsync(
            _workspaceId,
            removeTagsRequest,
            cancellationToken
        );
    }
}