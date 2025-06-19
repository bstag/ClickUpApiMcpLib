using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Page entity

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    public record CreatePageResponse
    (
        // Typically, a create operation returns the created resource.
        [property: JsonPropertyName("page")] Page Page
        // Or directly the properties of the Page if the API flattens it:
        // [property: JsonPropertyName("id")] string Id,
        // [property: JsonPropertyName("doc_id")] string DocId,
        // ... other Page properties
        // For now, assuming it wraps the Page entity.
    );
}
