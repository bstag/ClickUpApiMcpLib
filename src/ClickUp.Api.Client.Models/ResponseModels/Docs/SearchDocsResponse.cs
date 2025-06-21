using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For Doc entity

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    /// <summary>
    /// Represents the response from searching Documents.
    /// </summary>
    /// <param name="Docs">A list of <see cref="Entities.Docs.Doc"/> objects matching the search criteria.</param>
    /// <param name="NextPageId">Optional: An identifier (cursor or page ID) for retrieving the next page of search results. Its presence indicates more results are available.</param>
    /// <param name="TotalCount">Optional: The total number of documents matching the search criteria, if available.</param>
    /// <param name="LastPage">Optional: Indicates if this is the last page of search results.</param>
    public record SearchDocsResponse
    (
        [property: JsonPropertyName("docs")] List<Doc> Docs,
        [property: JsonPropertyName("next_page_id")] string? NextPageId,
        [property: JsonPropertyName("total_count")] int? TotalCount,
        [property: JsonPropertyName("last_page")] bool? LastPage
    );
}
