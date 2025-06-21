using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ReplyMessage

namespace ClickUp.Api.Client.Models.ResponseModels.Chat
{
    /// <summary>
    /// Represents the response after creating a reply to a chat message.
    /// </summary>
    /// <param name="Reply">The created <see cref="ReplyMessage"/> object.
    /// The actual API might wrap this in a "data" or "reply" property, or return the ReplyMessage structure directly.
    /// </param>
    public record CommentCreateReplyMessageResponse
    (
        [property: JsonPropertyName("reply")] ReplyMessage Reply
    );
}
