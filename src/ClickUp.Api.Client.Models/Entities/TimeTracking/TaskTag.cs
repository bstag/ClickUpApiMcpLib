using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For User, if creator is a User object

namespace ClickUp.Api.Client.Models.Entities.TimeTracking
{
    // Note: This model might be identical to Common.Tag.
    // If the structure (especially for 'creator') aligns perfectly with Common.Tag,
    // Common.Tag should be used instead to avoid redundancy.
    // This file is created based on the instruction that TaskTag might be distinct.
    public record TaskTag
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("tag_fg")] string? TagFg,
        [property: JsonPropertyName("tag_bg")] string? TagBg,
        [property: JsonPropertyName("creator")] int? Creator // In some contexts, creator might be just an ID for tags within time entries.
                                                             // If it's a full User object, then Common.User should be used,
                                                             // and this model becomes more like Common.Tag.
    );
}
