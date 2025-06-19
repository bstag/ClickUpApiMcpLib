using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    // Note: This model appears identical to ChatSimpleUser.
    // If confirmed, ChatSimpleUser should be used everywhere to avoid redundancy.
    public record CommentSimpleUser
    (
        [property: JsonPropertyName("email")] string? Email,
        [property: JsonPropertyName("id")] string Id, // User ID
        [property: JsonPropertyName("initials")] string? Initials,
        [property: JsonPropertyName("name")] string? Name, // Full name
        [property: JsonPropertyName("username")] string? Username,
        [property: JsonPropertyName("profilePicture")] string? ProfilePicture
    );
}
