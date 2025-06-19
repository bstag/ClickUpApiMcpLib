using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Page entity

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    public record GetDocPagesResponse
    (
        [property: JsonPropertyName("pages")] List<Page> Pages,
        [property: JsonPropertyName("next_page_id")] string? NextPageId, // For pagination, if applicable
        [property: JsonPropertyName("total_count")] int? TotalCount // Total number of pages, if applicable
    );
}
