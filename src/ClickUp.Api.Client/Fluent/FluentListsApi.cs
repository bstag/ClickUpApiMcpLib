using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentListsApi
{
    private readonly IListsService _listsService;

    public FluentListsApi(IListsService listsService)
    {
        _listsService = listsService;
    }

    public async Task<IEnumerable<ClickUpList>> GetListsInFolderAsync(string folderId, bool? archived = null, CancellationToken cancellationToken = default)
    {
        return await _listsService.GetListsInFolderAsync(folderId, archived, cancellationToken);
    }

    public async Task<IEnumerable<ClickUpList>> GetFolderlessListsAsync(string spaceId, bool? archived = null, CancellationToken cancellationToken = default)
    {
        return await _listsService.GetFolderlessListsAsync(spaceId, archived, cancellationToken);
    }

    public async Task<ClickUpList> GetListAsync(string listId, CancellationToken cancellationToken = default)
    {
        return await _listsService.GetListAsync(listId, cancellationToken);
    }

    public async Task DeleteListAsync(string listId, CancellationToken cancellationToken = default)
    {
        await _listsService.DeleteListAsync(listId, cancellationToken);
    }

    public async Task AddTaskToListAsync(string listId, string taskId, CancellationToken cancellationToken = default)
    {
        await _listsService.AddTaskToListAsync(listId, taskId, cancellationToken);
    }

    public async Task RemoveTaskFromListAsync(string listId, string taskId, CancellationToken cancellationToken = default)
    {
        await _listsService.RemoveTaskFromListAsync(listId, taskId, cancellationToken);
    }

    public FluentCreateListRequest CreateListInFolder(string folderId)
    {
        return new FluentCreateListRequest(folderId, _listsService, false);
    }

    public FluentCreateListRequest CreateFolderlessList(string spaceId)
    {
        return new FluentCreateListRequest(spaceId, _listsService, true);
    }

    public FluentUpdateListRequest UpdateList(string listId)
    {
        return new FluentUpdateListRequest(listId, _listsService);
    }

    public FluentCreateListFromTemplateRequest CreateListFromTemplateInFolder(string folderId, string templateId)
    {
        return new FluentCreateListFromTemplateRequest(folderId, templateId, _listsService, true);
    }

    public FluentCreateListFromTemplateRequest CreateListFromTemplateInSpace(string spaceId, string templateId)
    {
        return new FluentCreateListFromTemplateRequest(spaceId, templateId, _listsService, false);
    }
}
