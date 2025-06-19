using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Common
{
    public record User
    (
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("username")] string Username,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("profilePicture")] string? ProfilePicture,
        [property: JsonPropertyName("initials")] string? Initials,
        [property: JsonPropertyName("week_start_day")] int? WeekStartDay, // Assuming int, could be string
        [property: JsonPropertyName("global_font_support")] bool? GlobalFontSupport,
        [property: JsonPropertyName("timezone")] string? Timezone
    );
}
