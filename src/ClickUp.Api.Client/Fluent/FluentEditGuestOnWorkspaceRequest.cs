using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Guests;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentEditGuestOnWorkspaceRequest
{
    private bool? _canSeePointsEstimated;
    private bool? _canEditTags;
    private bool? _canSeeTimeSpent;
    private bool? _canSeeTimeEstimated;
    private bool? _canCreateViews;
    private int? _customRoleId;

    private readonly string _workspaceId;
    private readonly string _guestId;
    private readonly IGuestsService _guestsService;

    public FluentEditGuestOnWorkspaceRequest(string workspaceId, string guestId, IGuestsService guestsService)
    {
        _workspaceId = workspaceId;
        _guestId = guestId;
        _guestsService = guestsService;
    }

    public FluentEditGuestOnWorkspaceRequest WithCanSeePointsEstimated(bool canSeePointsEstimated)
    {
        _canSeePointsEstimated = canSeePointsEstimated;
        return this;
    }

    public FluentEditGuestOnWorkspaceRequest WithCanEditTags(bool canEditTags)
    {
        _canEditTags = canEditTags;
        return this;
    }

    public FluentEditGuestOnWorkspaceRequest WithCanSeeTimeSpent(bool canSeeTimeSpent)
    {
        _canSeeTimeSpent = canSeeTimeSpent;
        return this;
    }

    public FluentEditGuestOnWorkspaceRequest WithCanSeeTimeEstimated(bool canSeeTimeEstimated)
    {
        _canSeeTimeEstimated = canSeeTimeEstimated;
        return this;
    }

    public FluentEditGuestOnWorkspaceRequest WithCanCreateViews(bool canCreateViews)
    {
        _canCreateViews = canCreateViews;
        return this;
    }

    public FluentEditGuestOnWorkspaceRequest WithCustomRoleId(int customRoleId)
    {
        _customRoleId = customRoleId;
        return this;
    }

    public async Task<Guest> EditAsync(CancellationToken cancellationToken = default)
    {
        var editGuestRequest = new EditGuestOnWorkspaceRequest(
            CanSeePointsEstimated: _canSeePointsEstimated,
            CanEditTags: _canEditTags,
            CanSeeTimeSpent: _canSeeTimeSpent,
            CanSeeTimeEstimated: _canSeeTimeEstimated,
            CanCreateViews: _canCreateViews,
            CustomRoleId: _customRoleId
        );

        return await _guestsService.EditGuestOnWorkspaceAsync(
            _workspaceId,
            _guestId,
            editGuestRequest,
            cancellationToken
        );
    }
}