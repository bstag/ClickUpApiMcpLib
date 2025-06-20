using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users; // For GuestUserInfo, InvitedByUserInfo if they exist there

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

// This class represents an item in the "members" array of InviteGuestToWorkspaceResponseTeam
public record InvitedGuestMember
(
    [property: JsonPropertyName("user")]
    GuestUserInfo User, // Assuming GuestUserInfo is suitable from Guest.cs or defined elsewhere

    [property: JsonPropertyName("invited_by")]
    InvitedByUserInfo? InvitedBy, // Assuming InvitedByUserInfo is suitable

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
