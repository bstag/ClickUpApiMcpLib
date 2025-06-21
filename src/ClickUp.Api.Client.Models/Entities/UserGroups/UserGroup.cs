using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.Common; // Assuming Member is in ClickUp.Api.Client.Models

namespace ClickUp.Api.Client.Models.Entities.UserGroups;

/// <summary>
/// Represents a User Group in ClickUp.
/// </summary>
/// <param name="Id">The unique identifier of the user group.</param>
/// <param name="TeamId">The identifier of the team (workspace) this group belongs to.</param>
/// <param name="UserId">The identifier of the user who created the group.</param>
/// <param name="Name">The name of the user group.</param>
/// <param name="Handle">The handle or mention name for the user group (e.g., "@groupname").</param>
/// <param name="DateCreated">The date the user group was created, as a string (e.g., Unix timestamp in milliseconds).</param>
/// <param name="Initials">The initials for the user group, if any.</param>
/// <param name="Members">A list of members belonging to this user group.</param>
/// <param name="Avatar">The avatar information for the user group.</param>
public record UserGroup
(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("team_id")] string TeamId,
    [property: JsonPropertyName("userid")] int UserId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("handle")] string Handle,
    [property: JsonPropertyName("date_created")] string DateCreated,
    [property: JsonPropertyName("initials")] string? Initials,
    [property: JsonPropertyName("members")] List<Member> Members,
    [property: JsonPropertyName("avatar")] UserGroupAvatar? Avatar
);
