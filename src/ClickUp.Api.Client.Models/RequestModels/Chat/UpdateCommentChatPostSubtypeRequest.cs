using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    /// <summary>
    /// Represents the subtype to set when updating (patching) a chat post.
    /// </summary>
    /// <param name="Id">The ID of the existing post subtype to set.</param>
    public record UpdateCommentChatPostSubtypeRequest
    (
        [property: JsonPropertyName("id")] string Id
    );
}
