using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Guests;
using ClickUp.Api.Client.Models.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Fluent;

public class ItemFluentAddGuestRequest
{
    private readonly AddGuestToItemRequest _request = new();
    private readonly string _itemId;
    private readonly string _guestId;
    private readonly IGuestsService _guestsService;
    private readonly ItemType _itemType;
    private readonly List<string> _validationErrors = new List<string>();

    private bool? _includeShared;
    private bool? _customTaskIds;
    private string? _teamId;

    public enum ItemType
    {
        Task,
        List,
        Folder
    }

    public ItemFluentAddGuestRequest(string itemId, string guestId, IGuestsService guestsService, ItemType itemType)
    {
        _itemId = itemId;
        _guestId = guestId;
        _guestsService = guestsService;
        _itemType = itemType;
    }

    public ItemFluentAddGuestRequest WithPermissionLevel(int permissionLevel)
    {
        _request.PermissionLevel = permissionLevel;
        return this;
    }

    public ItemFluentAddGuestRequest WithIncludeShared(bool includeShared)
    {
        _includeShared = includeShared;
        return this;
    }

    public ItemFluentAddGuestRequest WithCustomTaskIds(bool customTaskIds)
    {
        _customTaskIds = customTaskIds;
        return this;
    }

    public ItemFluentAddGuestRequest WithTeamId(string teamId)
    {
        _teamId = teamId;
        return this;
    }

    public void Validate()
    {
        _validationErrors.Clear();
        if (string.IsNullOrWhiteSpace(_itemId))
        {
            _validationErrors.Add("ItemId is required.");
        }
        if (string.IsNullOrWhiteSpace(_guestId))
        {
            _validationErrors.Add("GuestId is required.");
        }
        if (_request.PermissionLevel <= 0) // Assuming permission level should be positive
        {
            _validationErrors.Add("A valid PermissionLevel is required.");
        }
        // Add other validation rules as needed

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException("Request validation failed.", _validationErrors);
        }
    }

    public async Task<Guest> AddAsync(CancellationToken cancellationToken = default)
    {
        Validate();
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
            _ => throw new System.ArgumentOutOfRangeException(nameof(_itemType), _itemType, "Invalid item type for adding guest.")
        };
    }
}