using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    /// <summary>
    /// Represents the location (e.g., Folder, List, Space) to which a chat channel is related.
    /// </summary>
    /// <param name="Id">The unique identifier of the location entity (e.g., Folder ID, List ID, Space ID).</param>
    /// <param name="Type">The type of the location entity.</param>
    /// <example>"folder", "list", "space"</example>
    public record ChatChannelLocation
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("type")] string Type
    );
}
