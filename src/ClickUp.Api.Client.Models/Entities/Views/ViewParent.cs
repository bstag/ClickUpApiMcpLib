using System.Text.Json.Serialization;
// Potentially: using ClickUp.Api.Client.Models.Entities.Views.Enums;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the parent of a View (e.g., Workspace, Space, Folder, List).
/// </summary>
public record ViewParent(
    [property: JsonPropertyName("id")]
    string Id, // ID of the parent item

    /// <summary>
    /// Type of the parent.
    /// OpenAPI spec indicates: 7 for Workspace, 4 for Space, 5 for Folder, 6 for List.
    /// Consider mapping to an enum (e.g., ViewParentType).
    /// </summary>
    [property: JsonPropertyName("type")]
    int Type
)
{
    // The schema might also include 'name' or other relevant parent details.
    // Adding 'name' as it's common, but needs schema verification.
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
