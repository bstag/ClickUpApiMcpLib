using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    /// <summary>
    /// Represents the subtype to associate with a chat post when creating it.
    /// </summary>
    /// <param name="Id">The ID of the existing post subtype.</param>
    public record CreateCommentChatPostSubtypeRequest
    (
        [property: JsonPropertyName("id")] string Id
    );
}
