using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents a guest in the AddGuestToTaskResponse.
/// </summary>
public record class AddGuestToTaskResponseGuest
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

    [property: JsonPropertyName("shared")]
    GuestSharingDetailsResponse Shared
);
