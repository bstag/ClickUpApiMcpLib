using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents a guest in the EditGuestOnWorkspaceResponse.
/// </summary>
public record class EditGuestOnWorkspaceResponseGuest
(
    [property: JsonPropertyName("user")]
    InviteGuestToWorkspaceResponseUser User,

    [property: JsonPropertyName("invited_by")]
    InvitedByUserInfo InvitedBy,

    [property: JsonPropertyName("can_see_time_spent")]
    bool? CanSeeTimeSpent,

    [property: JsonPropertyName("can_see_time_estimated")]
    bool? CanSeeTimeEstimated,

    [property: JsonPropertyName("can_see_points_estimated")]
    bool? CanSeePointsEstimated,

    [property: JsonPropertyName("can_edit_tags")]
    bool? CanEditTags,

    [property: JsonPropertyName("can_create_views")]
    bool? CanCreateViews,

    [property: JsonPropertyName("shared")]
    GuestSharingDetails Shared
);
