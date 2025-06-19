using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the team information in the RemoveGuestFromWorkspaceResponse.
/// </summary>
public record class RemoveGuestFromWorkspaceResponseTeam
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
    List<string> Members
);
