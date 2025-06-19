using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    public record CommentChatPostDataCreate
    (
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("subtype")] CommentChatPostSubtypeCreate? Subtype // Optional: if post has a specific subtype
    );
}
