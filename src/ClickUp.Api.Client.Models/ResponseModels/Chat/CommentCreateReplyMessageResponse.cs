using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ReplyMessage

namespace ClickUp.Api.Client.Models.ResponseModels.Chat
{
    public record CommentCreateReplyMessageResponse
    (
        // Assuming the response is the created ReplyMessage object.
        // The structure might be nested under a "reply" or "data" property in the actual API.
        // For now, assuming it's directly the ReplyMessage.
        [property: JsonPropertyName("reply")] ReplyMessage Reply // Or directly inherit from ReplyMessage
    );
}
