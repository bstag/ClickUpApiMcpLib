using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Page entity

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    /// <summary>
    /// Represents the response when retrieving multiple pages of a document, potentially with pagination.
    /// </summary>
    /// <param name="Pages">A list of <see cref="Entities.Docs.Page"/> objects.</param>
    /// <param name="NextPageId">Optional: An identifier for the next page of results, used for pagination if applicable.</param>
    /// <param name="TotalCount">Optional: The total number of pages available, if applicable.</param>
    public record GetDocPagesResponse
    (
        [property: JsonPropertyName("pages")] List<Page> Pages,
        [property: JsonPropertyName("next_page_id")] string? NextPageId,
        [property: JsonPropertyName("total_count")] int? TotalCount
    );
}
