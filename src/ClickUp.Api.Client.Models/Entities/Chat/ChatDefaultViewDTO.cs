using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Chat
{
    public record ChatDefaultViewDTO
    (
        [property: JsonPropertyName("type")] int Type, // View type identifier
        [property: JsonPropertyName("view_id")] string ViewId,
        [property: JsonPropertyName("standard")] bool Standard // Whether it's a standard view type
    );
}
