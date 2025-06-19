using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ChatMessage

namespace ClickUp.Api.Client.Models.ResponseModels.Chat
{
    public record CommentCreateChatMessageResponse
    (
        // Assuming the response is the created ChatMessage object.
        // The OpenAPI spec might have a slightly different wrapper or structure.
        // For example, it might be nested under a "message" or "data" property.
        // Let's assume it's directly the ChatMessage for now based on common patterns.
        // If it's wrapped, e.g. {"message": {...}}, then:
        // [property: JsonPropertyName("message")] ChatMessage Message

        // Directly inheriting from ChatMessage if the response is identical to the entity:
        // public record CommentCreateChatMessageResponse : ChatMessage { ... constructor ... }
        // Or, if it's a distinct structure:
        [property: JsonPropertyName("id")] string Id, // From CommentCreateChatMessageResponse schema example
        [property: JsonPropertyName("date")] long Date,
        [property: JsonPropertyName("user_id")] string UserId,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("content")] object Content, // Can be string or block kit object
        [property: JsonPropertyName("reactions")] List<ChatReaction>? Reactions,
        [property: JsonPropertyName("followers")] List<ChatSimpleUser>? Followers,
        [property: JsonPropertyName("parent_channel_id")] string ParentChannelId
        // This structure is based on a typical response, might need adjustment if ChatMessage entity
        // already covers all these and the response is a direct mapping to ChatMessage.
        // For now, creating a distinct response model.
    );
}
