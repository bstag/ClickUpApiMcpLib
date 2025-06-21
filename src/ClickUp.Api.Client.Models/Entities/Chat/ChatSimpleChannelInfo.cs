using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    /// <summary>
    /// Represents basic information about a chat channel, often used in nested contexts.
    /// </summary>
    public class ChatSimpleChannelInfo
    {
        /// <summary>
        /// Gets or sets the unique identifier of the chat channel.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the chat channel.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}
