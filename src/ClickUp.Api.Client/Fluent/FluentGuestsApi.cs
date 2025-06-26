using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.ResponseModels.Guests;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentGuestsApi
{
    private readonly IGuestsService _guestsService;

    public FluentGuestsApi(IGuestsService guestsService)
    {
        _guestsService = guestsService;
    }

    public FluentInviteGuestToWorkspaceRequest InviteGuestToWorkspace(string workspaceId)
    {
        return new FluentInviteGuestToWorkspaceRequest(workspaceId, _guestsService);
    }

    public async Task<GetGuestResponse> GetGuestAsync(string workspaceId, string guestId, CancellationToken cancellationToken = default)
    {
        return await _guestsService.GetGuestAsync(workspaceId, guestId, cancellationToken);
    }

    public FluentEditGuestOnWorkspaceRequest EditGuestOnWorkspace(string workspaceId, string guestId)
    {
        return new FluentEditGuestOnWorkspaceRequest(workspaceId, guestId, _guestsService);
    }

    public async Task RemoveGuestFromWorkspaceAsync(string workspaceId, string guestId, CancellationToken cancellationToken = default)
    {
        await _guestsService.RemoveGuestFromWorkspaceAsync(workspaceId, guestId, cancellationToken);
    }

    public FluentAddGuestToItemRequest AddGuestToTask(string taskId, string guestId)
    {
        return new FluentAddGuestToItemRequest(taskId, guestId, _guestsService, FluentAddGuestToItemRequest.ItemType.Task);
    }

    public FluentAddGuestToItemRequest AddGuestToListAsync(string listId, string guestId)
    {
        return new FluentAddGuestToItemRequest(listId, guestId, _guestsService, FluentAddGuestToItemRequest.ItemType.List);
    }

    public FluentAddGuestToItemRequest AddGuestToFolder(string folderId, string guestId)
    {
        return new FluentAddGuestToItemRequest(folderId, guestId, _guestsService, FluentAddGuestToItemRequest.ItemType.Folder);
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