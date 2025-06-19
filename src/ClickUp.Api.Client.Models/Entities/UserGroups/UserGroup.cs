using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models; // Assuming Member is in ClickUp.Api.Client.Models

namespace ClickUp.Api.Client.Models.Entities.UserGroups;

/// <summary>
/// Represents a User Group in ClickUp.
/// </summary>
public record UserGroup
(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("team_id")] string TeamId, // Corresponds to workspace_id in some contexts
    [property: JsonPropertyName("userid")] int UserId, // Creator of the group
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("handle")] string Handle,
    [property: JsonPropertyName("date_created")] string DateCreated,
    [property: JsonPropertyName("initials")] string? Initials, // Made nullable as it might not always be present
    [property: JsonPropertyName("members")] List<Member> Members,
    [property: JsonPropertyName("avatar")] UserGroupAvatar? Avatar
);
