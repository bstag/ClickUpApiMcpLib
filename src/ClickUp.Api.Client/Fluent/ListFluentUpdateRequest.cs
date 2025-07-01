using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.RequestModels.Lists;

using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class ListFluentUpdateRequest
{
    private string? _name;
    private string? _content;
    private string? _markdownContent;
    private string? _assignee;
    private System.DateTimeOffset? _dueDate;
    private bool? _dueDateTime;
    private int? _priority;
    private string? _status;
    private bool? _unsetStatus;

    private readonly string _listId;
    private readonly IListsService _listsService;

    public ListFluentUpdateRequest(string listId, IListsService listsService)
    {
        _listId = listId;
        _listsService = listsService;
    }

    public ListFluentUpdateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public ListFluentUpdateRequest WithContent(string content)
    {
        _content = content;
        return this;
    }

    public ListFluentUpdateRequest WithMarkdownContent(string markdownContent)
    {
        _markdownContent = markdownContent;
        return this;
    }

    public ListFluentUpdateRequest WithAssignee(string assignee)
    {
        _assignee = assignee;
        return this;
    }

    public ListFluentUpdateRequest WithDueDate(System.DateTimeOffset dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public ListFluentUpdateRequest WithDueDateTime(bool dueDateTime)
    {
        _dueDateTime = dueDateTime;
        return this;
    }

    public ListFluentUpdateRequest WithPriority(int priority)
    {
        _priority = priority;
        return this;
    }

    public ListFluentUpdateRequest WithStatus(string status)
    {
        _status = status;
        return this;
    }

    public ListFluentUpdateRequest WithUnsetStatus(bool unsetStatus)
    {
        _unsetStatus = unsetStatus;
        return this;
    }

    public async Task<ClickUpList> UpdateAsync(CancellationToken cancellationToken = default)
    {
        var updateListRequest = new UpdateListRequest(
            Name: _name ?? string.Empty,
            Content: _content,
            MarkdownContent: _markdownContent,
            DueDate: _dueDate,
            DueDateTime: _dueDateTime,
            Priority: _priority,
            Assignee: _assignee,
            Status: _status,
            UnsetStatus: _unsetStatus
        );

        return await _listsService.UpdateListAsync(
            _listId,
            updateListRequest,
            cancellationToken
        );
    }
}