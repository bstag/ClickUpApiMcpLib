using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    /// <summary>
    /// Represents the data for creating a "post" type chat message.
    /// </summary>
    /// <param name="Title">The title of the post.</param>
    /// <param name="Subtype">Optional: The subtype of the post, if applicable.</param>
    public record CreateCommentChatPostDataRequest
    (
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("subtype")] CreateCommentChatPostSubtypeRequest? Subtype
    );
}
