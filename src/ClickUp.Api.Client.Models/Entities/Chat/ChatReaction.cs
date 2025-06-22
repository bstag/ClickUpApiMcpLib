using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    /// <summary>
    /// Represents a reaction to a chat message.
    /// </summary>
    /// <param name="Date">The timestamp when the reaction was made.</param>
    /// <param name="Reaction">The reaction content, typically an emoji.</param>
    /// <param name="UserId">The identifier of the user who made the reaction.</param>
    public record ChatReaction
    (
        [property: JsonPropertyName("date")] System.DateTimeOffset? Date,
        [property: JsonPropertyName("reaction")] string Reaction,
        [property: JsonPropertyName("user_id")] string UserId
    );
}