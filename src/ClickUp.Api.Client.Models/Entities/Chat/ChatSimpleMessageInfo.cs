using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public class ChatSimpleMessageInfo
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("text_preview")] // Assuming a preview property
        public required string TextPreview { get; set; }
    }
}
