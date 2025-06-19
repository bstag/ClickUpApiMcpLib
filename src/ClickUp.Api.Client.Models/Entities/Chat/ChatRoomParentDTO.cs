using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public record ChatRoomParentDTO
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("type")] int Type // Type identifier (e.g., 0 for Workspace, 1 for Space, etc. - needs definition)
    );
}
