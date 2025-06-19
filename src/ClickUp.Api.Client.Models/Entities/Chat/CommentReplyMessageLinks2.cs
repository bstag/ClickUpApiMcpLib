using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public record CommentReplyMessageLinks2
    (
        [property: JsonPropertyName("reactions")] string? Reactions, // Link to reactions resource
        [property: JsonPropertyName("tagged_users")] string? TaggedUsers // Link to tagged users resource
        // Note: No "replies" link here, as it's for a reply itself.
    );
}
