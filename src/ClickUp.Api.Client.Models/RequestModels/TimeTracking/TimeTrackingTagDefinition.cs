using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking
{
    // Definition for tags when creating/updating time entries.
    public record TimeTrackingTagDefinition
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("tag_fg")] string? TagFg, // Optional: foreground color
        [property: JsonPropertyName("tag_bg")] string? TagBg  // Optional: background color
    );
}
