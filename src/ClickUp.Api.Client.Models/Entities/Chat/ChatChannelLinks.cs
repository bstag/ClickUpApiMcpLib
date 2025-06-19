using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public record ChatChannelLinks
    (
        // These are typically relative URLs or identifiers for linked resources
        [property: JsonPropertyName("members")] string? Members, // Link to members resource
        [property: JsonPropertyName("followers")] string? Followers // Link to followers resource
    );
}
