using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Users;

/// <summary>
/// Represents a user role in ClickUp.
/// </summary>
public record Role
{
    /// <summary>
    /// Gets or sets the ID of the role.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!; // Initialize with null forgiving operator, assuming Name is always present.

    /// <summary>
    /// Gets or sets a value indicating whether the role is custom.
    /// </summary>
    [JsonPropertyName("custom")]
    public bool Custom { get; init; }

    /// <summary>
    /// Gets or sets the ID of the role from which this role is inherited, if any.
    /// </summary>
    [JsonPropertyName("inherited_role")]
    public int? InheritedRole { get; init; }
}
