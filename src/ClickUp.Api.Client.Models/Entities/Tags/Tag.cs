using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User

namespace ClickUp.Api.Client.Models.Entities.Tags
{
    /// <summary>
    /// Represents a Tag in ClickUp.
    /// </summary>
    /// <param name="Name">The name of the tag.</param>
    /// <param name="TagFg">The foreground color of the tag (hexadecimal).</param>
    /// <param name="TagBg">The background color of the tag (hexadecimal).</param>
    /// <param name="Creator">The user who created the tag. This might be a user ID or a full User object depending on the API endpoint.</param>
    public record Tag
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("tag_fg")] string? TagFg,
        [property: JsonPropertyName("tag_bg")] string? TagBg,
        [property: JsonPropertyName("creator")] User? Creator
    );
}
