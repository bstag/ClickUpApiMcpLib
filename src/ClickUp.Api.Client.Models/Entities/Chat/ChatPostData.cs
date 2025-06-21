using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    /// <summary>
    /// Represents additional data for a chat message of type "post".
    /// </summary>
    /// <param name="Subtype">The subtype of the post, if any.</param>
    /// <param name="Title">The title of the post.</param>
    public record ChatPostData
    (
        [property: JsonPropertyName("subtype")] ChatPostSubtype? Subtype,
        [property: JsonPropertyName("title")] string Title
    );
}
