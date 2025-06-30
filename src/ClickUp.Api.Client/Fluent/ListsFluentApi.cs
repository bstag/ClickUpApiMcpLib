using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class ListsFluentApi
{
    private readonly IListsService _listsService;

    public ListsFluentApi(IListsService listsService)
    {
        _listsService = listsService;
    }

    public async Task<IEnumerable<ClickUpList>> GetListsInFolderAsync(string folderId, bool? archived = null, CancellationToken cancellationToken = default)
    {
        return await _listsService.GetListsInFolderAsync(folderId, archived, cancellationToken);
    }

    public async IAsyncEnumerable<ClickUpList> GetListsAsyncEnumerableAsync(string folderId, bool? archived = null, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var lists = await _listsService.GetListsInFolderAsync(folderId, archived, cancellationToken).ConfigureAwait(false);
        foreach (var list in lists)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return list;
        }
    }

    public async Task<IEnumerable<ClickUpList>> GetFolderlessListsAsync(string spaceId, bool? archived = null, CancellationToken cancellationToken = default)
    {
        return await _listsService.GetFolderlessListsAsync(spaceId, archived, cancellationToken);
    }

    public async IAsyncEnumerable<ClickUpList> GetFolderlessListsAsyncEnumerableAsync(string spaceId, bool? archived = null, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var list in _listsService.GetFolderlessListsAsyncEnumerableAsync(spaceId, archived, cancellationToken).ConfigureAwait(false))
        {
            yield return list;
        }
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

    public ListFluentCreateRequest CreateListInFolder(string folderId)
    {
        return new ListFluentCreateRequest(folderId, _listsService, false);
    }

    public ListFluentCreateRequest CreateFolderlessList(string spaceId)
    {
        return new ListFluentCreateRequest(spaceId, _listsService, true);
    }

    public ListFluentUpdateRequest UpdateList(string listId)
    {
        return new ListFluentUpdateRequest(listId, _listsService);
    }

    public TemplateFluentCreateListRequest CreateListFromTemplateInFolder(string folderId, string templateId)
    {
        return new TemplateFluentCreateListRequest(folderId, templateId, _listsService, true);
    }

    public TemplateFluentCreateListRequest CreateListFromTemplateInSpace(string spaceId, string templateId)
    {
        return new TemplateFluentCreateListRequest(spaceId, templateId, _listsService, false);
    }
}
