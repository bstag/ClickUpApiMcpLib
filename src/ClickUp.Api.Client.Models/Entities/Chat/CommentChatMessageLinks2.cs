using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public record CommentChatMessageLinks2
    (
        // These are typically relative URLs or identifiers for linked resources
        [property: JsonPropertyName("reactions")] string? Reactions,
        [property: JsonPropertyName("replies")] string? Replies,
        [property: JsonPropertyName("tagged_users")] string? TaggedUsers // Link to tagged users resource
    );
}
