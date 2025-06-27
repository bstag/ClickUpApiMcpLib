using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Guests;
using ClickUp.Api.Client.Models.ResponseModels.Guests;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class WorkspaceFluentInviteGuestRequest
{
    private string? _email;
    private bool? _canEditTags;
    private bool? _canSeeTimeSpent;
    private bool? _canSeeTimeEstimated;
    private bool? _canCreateViews;
    private bool? _canSeePointsEstimated;
    private int? _customRoleId;

    private readonly string _workspaceId;
    private readonly IGuestsService _guestsService;

    public WorkspaceFluentInviteGuestRequest(string workspaceId, IGuestsService guestsService)
    {
        _workspaceId = workspaceId;
        _guestsService = guestsService;
    }

    public WorkspaceFluentInviteGuestRequest WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public WorkspaceFluentInviteGuestRequest WithCanEditTags(bool canEditTags)
    {
        _canEditTags = canEditTags;
        return this;
    }

    public WorkspaceFluentInviteGuestRequest WithCanSeeTimeSpent(bool canSeeTimeSpent)
    {
        _canSeeTimeSpent = canSeeTimeSpent;
        return this;
    }

    public WorkspaceFluentInviteGuestRequest WithCanSeeTimeEstimated(bool canSeeTimeEstimated)
    {
        _canSeeTimeEstimated = canSeeTimeEstimated;
        return this;
    }

    public WorkspaceFluentInviteGuestRequest WithCanCreateViews(bool canCreateViews)
    {
        _canCreateViews = canCreateViews;
        return this;
    }

    public WorkspaceFluentInviteGuestRequest WithCanSeePointsEstimated(bool canSeePointsEstimated)
    {
        _canSeePointsEstimated = canSeePointsEstimated;
        return this;
    }

    public WorkspaceFluentInviteGuestRequest WithCustomRoleId(int customRoleId)
    {
        _customRoleId = customRoleId;
        return this;
    }

    public async Task<InviteGuestToWorkspaceResponse> InviteAsync(CancellationToken cancellationToken = default)
    {
        var inviteGuestRequest = new InviteGuestToWorkspaceRequest(
            Email: _email ?? string.Empty,
            CanEditTags: _canEditTags,
            CanSeeTimeSpent: _canSeeTimeSpent,
            CanSeeTimeEstimated: _canSeeTimeEstimated,
            CanCreateViews: _canCreateViews,
            CanSeePointsEstimated: _canSeePointsEstimated,
            CustomRoleId: _customRoleId
        );

        return await _guestsService.InviteGuestToWorkspaceAsync(
            _workspaceId,
            inviteGuestRequest,
            cancellationToken
        );
    }
}