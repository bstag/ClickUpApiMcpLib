using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Common;

/// <summary>
/// Represents the sharing options for an entity (e.g., CuTask, List, View).
/// </summary>
public record SharingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is publicly shared.
    /// </summary>
    [JsonPropertyName("is_public")]
    public bool? IsPublic { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is shared with all team members (everyone in the workspace).
    /// </summary>
    [JsonPropertyName("share_all_team_members")]
    public bool? ShareAllTeamMembers { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is shared with guests.
    /// </summary>
    [JsonPropertyName("share_with_guests")]
    public bool? ShareWithGuests { get; init; }

    /// <summary>
    /// Gets or sets the list of user IDs with whom the entity is shared.
    /// </summary>
    [JsonPropertyName("shared_user_ids")]
    public List<long>? SharedUserIds { get; init; }

    /// <summary>
    /// Gets or sets the list of group IDs with whom the entity is shared.
    /// </summary>
    [JsonPropertyName("shared_group_ids")]
    public List<string>? SharedGroupIds { get; init; }
}
