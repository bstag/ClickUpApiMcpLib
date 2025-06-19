using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Docs
{
    public record Page
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("doc_id")] string DocId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("content")] string? Content // Assuming content can be nullable
    );
}
