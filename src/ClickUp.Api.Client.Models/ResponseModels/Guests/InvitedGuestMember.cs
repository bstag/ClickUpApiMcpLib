using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users; // For GuestUserInfo, InvitedByUserInfo if they exist there

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents an invited guest member within a team, often returned as part of inviting a guest to a workspace.
/// </summary>
/// <param name="User">Detailed information about the guest user.</param>
/// <param name="InvitedBy">Information about the user who invited this guest.</param>
/// <param name="CanSeeTimeSpent">Indicates if the guest can see time spent on tasks.</param>
/// <param name="CanSeeTimeEstimated">Indicates if the guest can see time estimated for tasks.</param>
/// <param name="CanEditTags">Indicates if the guest can edit tags.</param>
/// <param name="CanCreateViews">Indicates if the guest can create views.</param>
/// <param name="CanSeePointsEstimated">Indicates if the guest can see estimated story points.</param>
public record InvitedGuestMember
(
    [property: JsonPropertyName("user")]
    GuestUserInfo User,

    [property: JsonPropertyName("invited_by")]
    InvitedByUserInfo? InvitedBy,

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
