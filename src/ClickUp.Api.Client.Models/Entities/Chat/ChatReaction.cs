using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public record ChatReaction
    (
        [property: JsonPropertyName("date")] long? Date, // Timestamp of when the reaction was made
        [property: JsonPropertyName("reaction")] string Reaction, // The emoji or reaction string
        [property: JsonPropertyName("user_id")] string UserId // ID of the user who reacted
    );
}
