using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    public record CommentChatPostDataPatch
    (
        [property: JsonPropertyName("title")] string? Title, // Optional: new title
        [property: JsonPropertyName("subtype")] CommentChatPostSubtypePatch? Subtype // Optional: new subtype
    );
}
