using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Lists
{
    /// <summary>
    /// Represents the priority information for a List.
    /// </summary>
    /// <param name="Priority">The priority value as a string (e.g., "high", "normal", or a numeric string).</param>
    /// <param name="Color">The color associated with this priority, if any.</param>
    public record ListPriorityInfo
    (
        [property: JsonPropertyName("priority")] string Priority,
        [property: JsonPropertyName("color")] string? Color
    );
}
