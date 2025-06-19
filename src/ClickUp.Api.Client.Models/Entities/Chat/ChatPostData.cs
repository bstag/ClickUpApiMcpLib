using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public record ChatPostData
    (
        [property: JsonPropertyName("subtype")] ChatPostSubtype? Subtype, // Optional, might not be present for all posts
        [property: JsonPropertyName("title")] string Title
    );
}
