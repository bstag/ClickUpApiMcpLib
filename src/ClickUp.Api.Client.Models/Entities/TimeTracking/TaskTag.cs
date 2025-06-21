using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For User, if creator is a User object

namespace ClickUp.Api.Client.Models.Entities.TimeTracking
{
    /// <summary>
    /// Represents a Tag associated with a Task, specifically in the context of Time Tracking.
    /// Note: This model is very similar to <see cref="ClickUp.Api.Client.Models.Entities.Tags.Tag"/>.
    /// Consider consolidation if they represent the same concept. The 'Creator' property here is an int ID,
    /// whereas in <see cref="ClickUp.Api.Client.Models.Entities.Tags.Tag"/> it's a User object.
    /// </summary>
    /// <param name="Name">The name of the tag.</param>
    /// <param name="TagFg">The foreground color of the tag (hexadecimal).</param>
    /// <param name="TagBg">The background color of the tag (hexadecimal).</param>
    /// <param name="Creator">The identifier of the user who created the tag. In this context, it's often just the user ID.</param>
    public record TaskTag
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("tag_fg")] string? TagFg,
        [property: JsonPropertyName("tag_bg")] string? TagBg,
        [property: JsonPropertyName("creator")] int? Creator
    );
}
