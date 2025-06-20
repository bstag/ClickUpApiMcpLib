using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public class ChatSimpleMessageInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("text_preview")] // Assuming a preview property
        public string TextPreview { get; set; }
    }
}
