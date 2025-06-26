using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Guests;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentAddGuestToItemRequest
{
    private readonly AddGuestToItemRequest _request = new();
    private readonly string _itemId;
    private readonly string _guestId;
    private readonly IGuestsService _guestsService;
    private readonly ItemType _itemType;

    private bool? _includeShared;
    private bool? _customTaskIds;
    private string? _teamId;

    public enum ItemType
    {
        Task,
        List,
        Folder
    }

    public FluentAddGuestToItemRequest(string itemId, string guestId, IGuestsService guestsService, ItemType itemType)
    {
        _itemId = itemId;
        _guestId = guestId;
        _guestsService = guestsService;
        _itemType = itemType;
    }

    public FluentAddGuestToItemRequest WithPermissionLevel(int permissionLevel)
    {
        _request.PermissionLevel = permissionLevel;
        return this;
    }

    public FluentAddGuestToItemRequest WithIncludeShared(bool includeShared)
    {
        _includeShared = includeShared;
        return this;
    }

    public FluentAddGuestToItemRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public FluentAddGuestToItemRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    public async Task<Guest> AddAsync(CancellationToken cancellationToken = default)
    {
        return _itemType switch
        {
            ItemType.Task => await _guestsService.AddGuestToTaskAsync(
                _itemId,
                _guestId,
                _request,
                _includeShared,
                _customTaskIds,
                _teamId,
                cancellationToken
            ),
            ItemType.List => await _guestsService.AddGuestToListAsync(
                _itemId,
                _guestId,
                _request,
                _includeShared,
                cancellationToken
            ),
            ItemType.Folder => await _guestsService.AddGuestToFolderAsync(
                _itemId,
                _guestId,
                _request,
                _includeShared,
                cancellationToken
            ),
            _ => throw new System.ArgumentOutOfRangeException()
        };
    }
}