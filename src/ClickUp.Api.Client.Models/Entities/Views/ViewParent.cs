using System.Text.Json.Serialization;
// Potentially: using ClickUp.Api.Client.Models.Entities.Views.Enums;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents the parent of a View (e.g., Workspace, Space, Folder, List).
/// </summary>
/// <param name="Id">The unique identifier of the parent entity.</param>
/// <param name="Type">An integer representing the type of the parent entity.
/// Common values: 7 for Workspace (Team), 4 for Space, 5 for Folder, 6 for List.
/// Consider using <see cref="Enums.ViewParentType"/> for a more type-safe representation if appropriate.
/// </param>
public record ViewParent(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("type")]
    int Type
)
{
    /// <summary>
    /// Gets the name of the parent entity. This might not always be populated depending on the API response.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
