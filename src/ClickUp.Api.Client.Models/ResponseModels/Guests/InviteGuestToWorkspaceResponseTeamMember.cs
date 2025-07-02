using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents a team member (guest) in the InviteGuestToWorkspaceResponse.
/// </summary>
public record class InviteGuestToWorkspaceResponseTeamMember
(
    [property: JsonPropertyName("user")]
    InviteGuestToWorkspaceResponseUser User,

    [property: JsonPropertyName("invited_by")]
    InvitedByUserInfoResponse InvitedBy,

    [property: JsonPropertyName("can_see_time_spent")]
    bool? CanSeeTimeSpent,

    [property: JsonPropertyName("can_see_time_estimated")]
    bool? CanSeeTimeEstimated,

    [property: JsonPropertyName("can_edit_tags")]
    bool? CanEditTags,

    [property: JsonPropertyName("can_create_views")]
    bool? CanCreateViews,

    [property: JsonPropertyName("can_see_points_estimated")]
    bool? CanSeePointsEstimated
);
