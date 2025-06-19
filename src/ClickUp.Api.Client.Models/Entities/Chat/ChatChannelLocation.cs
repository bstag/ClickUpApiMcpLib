using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    // Could define a ChatChannelLocationType enum if "folder", "list", "space" are the only values.
    // [JsonConverter(typeof(JsonStringEnumConverter))]
    // public enum ChatChannelLocationType { FOLDER, LIST, SPACE }

    public record ChatChannelLocation
    (
        [property: JsonPropertyName("id")] string Id, // ID of the folder, list, or space
        [property: JsonPropertyName("type")] string Type // "folder", "list", or "space"
    );
}
