using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Doc entity and PageDefaults

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    // PageDefaults is now defined in Entities/Docs/PageDefaults.cs

    public record CreateDocResponse
    (
        [property: JsonPropertyName("doc")] Doc Doc,
        [property: JsonPropertyName("page_defaults")] PageDefaults? PageDefaults,
        [property: JsonPropertyName("default_page_id")] string? DefaultPageId
    );
}
