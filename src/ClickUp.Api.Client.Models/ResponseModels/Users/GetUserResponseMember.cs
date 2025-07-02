using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.ResponseModels.Guests; // For guest-related models

namespace ClickUp.Api.Client.Models.ResponseModels.Users;

/// <summary>
/// Represents a member (user) in the GetUserResponse.
/// </summary>
public record class GetUserResponseMember
(
    [property: JsonPropertyName("user")]
    InviteGuestToWorkspaceResponseUser User,

    [property: JsonPropertyName("invited_by")]
    InvitedByUserInfoResponse InvitedBy,

    [property: JsonPropertyName("shared")]
    GuestSharingDetailsResponse Shared
);
