using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.RequestModels.Lists;
using ClickUp.Api.Client.Models.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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
    private readonly List<string> _validationErrors = new List<string>();

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

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_listId))
        {
            _validationErrors.Add("ListId is required.");
        }
        // For an update, at least one field should ideally be provided.
        // However, the API might allow an empty update request (noop).
        // For now, we'll assume the API handles this.
        if (string.IsNullOrWhiteSpace(_name) &&
            string.IsNullOrWhiteSpace(_content) &&
            string.IsNullOrWhiteSpace(_markdownContent) &&
            string.IsNullOrWhiteSpace(_assignee) &&
            !_dueDate.HasValue &&
            !_dueDateTime.HasValue &&
            !_priority.HasValue &&
            string.IsNullOrWhiteSpace(_status) &&
            !_unsetStatus.HasValue)
        {
            _validationErrors.Add("At least one property must be set for updating a List.");
        }


        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<ClickUpList> UpdateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
        var updateListRequest = new UpdateListRequest(
            Name: _name ?? string.Empty, // API might require name, or handle empty if other fields are set.
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