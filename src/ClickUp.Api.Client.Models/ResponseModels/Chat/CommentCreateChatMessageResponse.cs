using System.Text.Json.Serialization;

using ClickUp.Api.Client.Models.Entities.Chat; // For ChatMessage

namespace ClickUp.Api.Client.Models.ResponseModels.Chat
{
    /// <summary>
    /// Represents the response after creating a new chat message.
    /// This often mirrors the structure of a <see cref="ChatMessage"/> or a subset of its properties.
    /// </summary>
    /// <param name="Id">The ID of the newly created chat message.</param>
    /// <param name="Date">The timestamp (Unix milliseconds) when the message was created.</param>
    /// <param name="UserId">The ID of the user who created the message.</param>
    /// <param name="Type">The type of the message (e.g., "comment", "post").</param>
    /// <param name="Content">The content of the message. This can be a string or a more complex object (e.g., for block kit messages).</param>
    /// <param name="Reactions">A list of reactions on the message, if any.</param>
    /// <param name="Followers">A list of users following this message, if any.</param>
    /// <param name="ParentChannelId">The ID of the parent channel where the message was created.</param>
    public record CommentCreateChatMessageResponse
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("date")] DateTimeOffset Date,
        [property: JsonPropertyName("user_id")] string UserId,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("content")] object Content,
        [property: JsonPropertyName("reactions")] List<ChatReaction>? Reactions,
        [property: JsonPropertyName("followers")] List<ChatSimpleUser>? Followers,
        [property: JsonPropertyName("parent_channel_id")] string ParentChannelId
    );
}