using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    /// <summary>
    /// Represents basic information about a chat message, often used in contexts like message replies.
    /// </summary>
    public class ChatSimpleMessageInfo
    {
        /// <summary>
        /// Gets or sets the unique identifier of the chat message.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets a preview of the message text content.
        /// </summary>
        [JsonPropertyName("text_preview")]
        public required string TextPreview { get; set; }
    }
}
