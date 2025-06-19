using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public record ChatSimpleUser
    (
        [property: JsonPropertyName("email")] string? Email, // May not always be present
        [property: JsonPropertyName("id")] string Id, // User ID, typically a string in v3 Chat from examples
        [property: JsonPropertyName("initials")] string? Initials,
        [property: JsonPropertyName("name")] string? Name, // Full name, may not always be present
        [property: JsonPropertyName("username")] string? Username, // Username, may not always be present
        [property: JsonPropertyName("profilePicture")] string? ProfilePicture // Added, as it's common in user summaries
    );
}
