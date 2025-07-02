using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    /// <summary>
    /// Represents the data for updating (patching) a "post" type chat message.
    /// All properties are optional.
    /// </summary>
    /// <param name="Title">Optional: New title for the post.</param>
    /// <param name="Subtype">Optional: New subtype for the post.</param>
    public record UpdateCommentChatPostDataRequest
    (
        [property: JsonPropertyName("title")] string? Title,
        [property: JsonPropertyName("subtype")] UpdateCommentChatPostSubtypeRequest? Subtype
    );
}
