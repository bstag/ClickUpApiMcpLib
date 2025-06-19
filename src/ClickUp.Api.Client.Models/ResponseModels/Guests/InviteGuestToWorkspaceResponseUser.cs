using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents a user object within the InviteGuestToWorkspaceResponse.
/// </summary>
public record class InviteGuestToWorkspaceResponseUser
(
    [property: JsonPropertyName("id")]
    int Id,

    [property: JsonPropertyName("username")]
    string? Username,

    [property: JsonPropertyName("email")]
    string Email,

    [property: JsonPropertyName("color")]
    string? Color,

    [property: JsonPropertyName("profilePicture")]
    string? ProfilePicture,

    [property: JsonPropertyName("initials")]
    string? Initials,

    [property: JsonPropertyName("role")]
    int Role,

    [property: JsonPropertyName("custom_role")]
    CustomRole? CustomRole,

    [property: JsonPropertyName("last_active")]
    string? LastActive,

    [property: JsonPropertyName("date_joined")]
    string? DateJoined,

    [property: JsonPropertyName("date_invited")]
    string DateInvited
);
