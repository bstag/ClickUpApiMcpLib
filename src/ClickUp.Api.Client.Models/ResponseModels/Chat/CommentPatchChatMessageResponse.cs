using System.Text.Json.Serialization;

using ClickUp.Api.Client.Models.Entities.Chat; // For ChatMessage

namespace ClickUp.Api.Client.Models.ResponseModels.Chat
{
    /// <summary>
    /// Represents the response after updating (patching) an existing chat message.
    /// This often mirrors the structure of a <see cref="ChatMessage"/> or a subset of its properties, reflecting the updated state.
    /// </summary>
    /// <param name="Id">The ID of the updated chat message.</param>
    /// <param name="Date">The timestamp (Unix milliseconds) when the message was last updated.</param>
    /// <param name="UserId">The ID of the user associated with the message (e.g., original author or last editor).</param>
    /// <param name="Type">The type of the message (e.g., "comment", "post").</param>
    /// <param name="Content">The updated content of the message. This can be a string or a more complex object.</param>
    /// <param name="Reactions">A list of current reactions on the message, if any.</param>
    /// <param name="Followers">A list of current users following this message, if any.</param>
    /// <param name="ParentChannelId">The ID of the parent channel where the message exists.</param>
    /// <param name="Resolved">Indicates if the message/comment is resolved after the update.</param>
    /// <param name="Assignee">The user to whom the message/comment is assigned after the update, if any.</param>
    public record CommentPatchChatMessageResponse
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("date")] DateTimeOffset Date,
        [property: JsonPropertyName("user_id")] string UserId,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("content")] object Content,
        [property: JsonPropertyName("reactions")] List<ChatReaction>? Reactions,
        [property: JsonPropertyName("followers")] List<ChatSimpleUser>? Followers,
        [property: JsonPropertyName("parent_channel_id")] string ParentChannelId,
        [property: JsonPropertyName("resolved")] bool? Resolved,
        [property: JsonPropertyName("assignee")] ChatSimpleUser? Assignee
    );
}
