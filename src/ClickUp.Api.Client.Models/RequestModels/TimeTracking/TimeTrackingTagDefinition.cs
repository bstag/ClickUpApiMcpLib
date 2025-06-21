using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking
{
    /// <summary>
    /// Represents the definition of a tag when creating or updating time entries.
    /// This allows specifying the tag name and optionally its colors.
    /// </summary>
    /// <param name="Name">The name of the tag.</param>
    /// <param name="TagFg">Optional: The foreground color of the tag (hexadecimal).</param>
    /// <param name="TagBg">Optional: The background color of the tag (hexadecimal).</param>
    public record TimeTrackingTagDefinition
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("tag_fg")] string? TagFg,
        [property: JsonPropertyName("tag_bg")] string? TagBg
    );
}
