using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    public record CommentChatPostSubtypeCreate
    (
        [property: JsonPropertyName("id")] string Id // ID of the existing post subtype
    );
}
