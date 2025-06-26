using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.RequestModels.Lists;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentCreateListRequest
{
    private string? _name;
    private string? _content;
    private string? _markdownContent;
    private int? _assignee;
    private System.DateTimeOffset? _dueDate;
    private bool? _dueDateTime;
    private int? _priority;
    private string? _status;

    private readonly string _containerId; // Can be folderId or spaceId
    private readonly IListsService _listsService;
    private readonly bool _isFolderless; // True if creating a folderless list

    public FluentCreateListRequest(string containerId, IListsService listsService, bool isFolderless)
    {
        _containerId = containerId;
        _listsService = listsService;
        _isFolderless = isFolderless;
    }

    public FluentCreateListRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    public FluentCreateListRequest WithContent(string content)
    {
        _content = content;
        return this;
    }

    public FluentCreateListRequest WithMarkdownContent(string markdownContent)
    {
        _markdownContent = markdownContent;
        return this;
    }

    public FluentCreateListRequest WithAssignee(int assignee)
    {
        _assignee = assignee;
        return this;
    }

    public FluentCreateListRequest WithDueDate(System.DateTimeOffset dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public FluentCreateListRequest WithDueDateTime(bool dueDateTime)
    {
        _dueDateTime = dueDateTime;
        return this;
    }

    public FluentCreateListRequest WithPriority(int priority)
    {
        _priority = priority;
        return this;
    }

    public FluentCreateListRequest WithStatus(string status)
    {
        _status = status;
        return this;
    }

    public async Task<ClickUpList> CreateAsync(CancellationToken cancellationToken = default)
    {
        var createListRequest = new CreateListRequest(
            Name: _name ?? string.Empty,
            Content: _content,
            MarkdownContent: _markdownContent,
            DueDate: _dueDate,
            DueDateTime: _dueDateTime,
            Priority: _priority,
            Assignee: _assignee,
            Status: _status
        );

        if (_isFolderless)
        {
            return await _listsService.CreateFolderlessListAsync(
                _containerId,
                createListRequest,
                cancellationToken
            );
        }
        else
        {
            return await _listsService.CreateListInFolderAsync(
                _containerId,
                createListRequest,
                cancellationToken
            );
        }
    }
}