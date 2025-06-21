using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    /// <summary>
    /// Represents the subtype of a chat post.
    /// </summary>
    /// <param name="Id">The unique identifier of the post subtype.</param>
    /// <param name="Name">The name of the post subtype.</param>
    public record ChatPostSubtype
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name
    );
}
