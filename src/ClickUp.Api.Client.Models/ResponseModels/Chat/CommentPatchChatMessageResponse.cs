using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ChatMessage

namespace ClickUp.Api.Client.Models.ResponseModels.Chat
{
    public record CommentPatchChatMessageResponse
    (
        // Assuming the response is the updated ChatMessage object.
        // Similar to Create response, this might be a direct ChatMessage or wrapped.
        // For now, assuming direct ChatMessage or a structure very close to it.
        // If the response is exactly a ChatMessage, this specific record might be merged
        // or ChatMessage used directly.

        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("date")] long Date, // Or DateUpdated specifically
        [property: JsonPropertyName("user_id")] string UserId,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("content")] object Content,
        [property: JsonPropertyName("reactions")] List<ChatReaction>? Reactions,
        [property: JsonPropertyName("followers")] List<ChatSimpleUser>? Followers,
        [property: JsonPropertyName("parent_channel_id")] string ParentChannelId,
        [property: JsonPropertyName("resolved")] bool? Resolved,
        [property: JsonPropertyName("assignee")] ChatSimpleUser? Assignee
        // This structure is based on a typical response, might need adjustment.
    );
}
