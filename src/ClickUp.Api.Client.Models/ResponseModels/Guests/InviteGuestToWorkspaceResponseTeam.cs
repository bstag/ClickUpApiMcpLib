using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the team information in the InviteGuestToWorkspaceResponse.
/// </summary>
public record class InviteGuestToWorkspaceResponseTeam
(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("color")]
    string Color,

    [property: JsonPropertyName("avatar")]
    string? Avatar,

    [property: JsonPropertyName("members")]
    List<InviteGuestToWorkspaceResponseTeamMember> Members,

    [property: JsonPropertyName("roles")]
    List<Role> Roles
);
