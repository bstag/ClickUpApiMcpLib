using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    /// <summary>
    /// Represents the request parameters for getting chat messages.
    /// </summary>
    public class GetChatMessagesRequest
    {
        /// <summary>
        /// Optional. The cursor for pagination.
        /// </summary>
        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }

        /// <summary>
        /// Optional. The maximum number of messages to return per page.
        /// </summary>
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        /// <summary>
        /// Optional. The desired format for the message content.
        /// </summary>
        [JsonPropertyName("content_format")]
        public string? ContentFormat { get; set; }

        public GetChatMessagesRequest() { }
    }
}
