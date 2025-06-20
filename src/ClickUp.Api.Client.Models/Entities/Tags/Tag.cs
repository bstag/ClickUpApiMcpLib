using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For User

namespace ClickUp.Api.Client.Models.Entities.Tags
{
    public record Tag
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("tag_fg")] string? TagFg,
        [property: JsonPropertyName("tag_bg")] string? TagBg,
        [property: JsonPropertyName("creator")] ComUser? Creator // Assuming creator is a User object, could be an int (user ID)
                                                              // If it can be an int, more complex handling or a different model might be needed
                                                              // For now, aligning with User? based on common patterns.
    );
}
