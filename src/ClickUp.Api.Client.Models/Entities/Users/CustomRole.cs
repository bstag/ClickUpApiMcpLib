using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Users;

/// <summary>
/// Represents a custom user role in ClickUp.
/// </summary>
public record CustomRole
{
    /// <summary>
    /// Gets or sets the ID of the custom role.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the Team (Workspace) ID to which this custom role belongs.
    /// </summary>
    [JsonPropertyName("team_id")]
    public string TeamId { get; init; } = null!;

    /// <summary>
    /// Gets or sets the name of the custom role.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Gets or sets the ID of the role from which this custom role is inherited.
    /// </summary>
    [JsonPropertyName("inherited_role")]
    public int InheritedRole { get; init; }

    /// <summary>
    /// Gets or sets the date when this custom role was created.
    /// </summary>
    [JsonPropertyName("date_created")]
    public string DateCreated { get; init; } = null!;

    /// <summary>
    /// Gets or sets the list of member IDs assigned to this custom role.
    /// </summary>
    [JsonPropertyName("members")]
    public List<int> Members { get; init; } = new List<int>(); // Initialize to empty list as a sensible default
}
