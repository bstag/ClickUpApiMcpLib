using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Doc entity

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    public record SearchDocsResponse
    (
        [property: JsonPropertyName("docs")] List<Doc> Docs,
        [property: JsonPropertyName("next_page_id")] string? NextPageId, // Or "next_cursor"
        [property: JsonPropertyName("total_count")] int? TotalCount,
        [property: JsonPropertyName("last_page")] bool? LastPage
    );
}
