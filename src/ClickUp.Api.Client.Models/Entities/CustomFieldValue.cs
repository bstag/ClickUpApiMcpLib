using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities
{
    public record CustomFieldValue
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("value")] object Value,
        [property: JsonPropertyName("type")] string? Type
    );
}
