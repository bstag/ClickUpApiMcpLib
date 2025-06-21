using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Common
{
    /// <summary>
    /// Represents a status in ClickUp (e.g., for tasks).
    /// </summary>
    /// <param name="StatusValue">The textual representation of the status (e.g., "To Do", "In Progress").</param>
    /// <param name="Color">The color associated with the status.</param>
    /// <param name="OrderIndex">The order index of the status, determining its position in a list of statuses.</param>
    /// <param name="Type">The type of the status (e.g., "open", "custom", "closed").</param>
    public record Status
    (
        [property: JsonPropertyName("status")] string StatusValue,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("orderindex")] int OrderIndex,
        [property: JsonPropertyName("type")] string? Type
    );
}
