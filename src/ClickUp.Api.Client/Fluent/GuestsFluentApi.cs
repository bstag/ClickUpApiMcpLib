using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.ResponseModels.Guests;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class GuestsFluentApi
{
    private readonly IGuestsService _guestsService;

    public GuestsFluentApi(IGuestsService guestsService)
    {
        _guestsService = guestsService;
    }

    public WorkspaceFluentInviteGuestRequest InviteGuestToWorkspace(string workspaceId)
    {
        return new WorkspaceFluentInviteGuestRequest(workspaceId, _guestsService);
    }

    public async Task<GetGuestResponse> GetGuestAsync(string workspaceId, string guestId, CancellationToken cancellationToken = default)
    {
        return await _guestsService.GetGuestAsync(workspaceId, guestId, cancellationToken);
    }

    public WorkspaceFluentEditGuestRequest EditGuestOnWorkspace(string workspaceId, string guestId)
    {
        return new WorkspaceFluentEditGuestRequest(workspaceId, guestId, _guestsService);
    }

    public async Task RemoveGuestFromWorkspaceAsync(string workspaceId, string guestId, CancellationToken cancellationToken = default)
    {
        await _guestsService.RemoveGuestFromWorkspaceAsync(workspaceId, guestId, cancellationToken);
    }

    public ItemFluentAddGuestRequest AddGuestToTask(string taskId, string guestId)
    {
        return new ItemFluentAddGuestRequest(taskId, guestId, _guestsService, ItemFluentAddGuestRequest.ItemType.Task);
    }

    public ItemFluentAddGuestRequest AddGuestToListAsync(string listId, string guestId)
    {
        return new ItemFluentAddGuestRequest(listId, guestId, _guestsService, ItemFluentAddGuestRequest.ItemType.List);
    }

    public ItemFluentAddGuestRequest AddGuestToFolder(string folderId, string guestId)
    {
        return new ItemFluentAddGuestRequest(folderId, guestId, _guestsService, ItemFluentAddGuestRequest.ItemType.Folder);
    }

    public async Task<Guest> RemoveGuestFromTaskAsync(string taskId, string guestId, bool? includeShared = null, bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        return await _guestsService.RemoveGuestFromTaskAsync(taskId, guestId, includeShared, customTaskIds, teamId, cancellationToken);
    }

    public async Task<Guest> RemoveGuestFromListAsync(string listId, string guestId, bool? includeShared = null, CancellationToken cancellationToken = default)
    {
        return await _guestsService.RemoveGuestFromListAsync(listId, guestId, includeShared, cancellationToken);
    }

    public async Task<Guest> RemoveGuestFromFolderAsync(string folderId, string guestId, bool? includeShared = null, CancellationToken cancellationToken = default)
    {
        return await _guestsService.RemoveGuestFromFolderAsync(folderId, guestId, includeShared, cancellationToken);
    }
}