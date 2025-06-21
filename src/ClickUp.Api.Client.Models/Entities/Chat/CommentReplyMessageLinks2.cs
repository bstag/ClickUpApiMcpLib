using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    /// <summary>
    /// Represents API links related to a reply message within a chat.
    /// </summary>
    /// <param name="Reactions">Link to the reactions for this reply message.</param>
    /// <param name="TaggedUsers">Link to the users tagged in this reply message.</param>
    public record CommentReplyMessageLinks2
    (
        [property: JsonPropertyName("reactions")] string? Reactions,
        [property: JsonPropertyName("tagged_users")] string? TaggedUsers
    );
}
