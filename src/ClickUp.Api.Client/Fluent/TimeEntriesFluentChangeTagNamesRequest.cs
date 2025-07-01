using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TimeEntriesFluentChangeTagNamesRequest
{
    private string? _name; // This will be the old name
    private string? _newName;
    private string? _newColor; // This will be used for both TagBg and TagFg

    private readonly string _workspaceId;
    private readonly ITimeTrackingService _timeTrackingService;

    public TimeEntriesFluentChangeTagNamesRequest(string workspaceId, ITimeTrackingService timeTrackingService)
    {
        _workspaceId = workspaceId;
        _timeTrackingService = timeTrackingService;
    }

    public TimeEntriesFluentChangeTagNamesRequest WithOldName(string oldName)
    {
        _name = oldName;
        return this;
    }

    public TimeEntriesFluentChangeTagNamesRequest WithNewName(string newName)
    {
        _newName = newName;
        return this;
    }

    public TimeEntriesFluentChangeTagNamesRequest WithNewColor(string newColor)
    {
        _newColor = newColor;
        return this;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var changeTagNamesRequest = new ChangeTagNamesFromTimeEntriesRequest(
            Name: _name ?? string.Empty,
            NewName: _newName ?? string.Empty,
            TagBg: _newColor ?? string.Empty,
            TagFg: _newColor ?? string.Empty
        );

        await _timeTrackingService.ChangeTimeEntryTagNameAsync(
            _workspaceId,
            changeTagNamesRequest,
            cancellationToken
        );
    }
}