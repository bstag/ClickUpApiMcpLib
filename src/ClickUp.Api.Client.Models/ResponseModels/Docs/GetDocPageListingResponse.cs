using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Docs; // For DocPageListingItem

namespace ClickUp.Api.Client.Models.ResponseModels.Docs
{
    /// <summary>
    /// Represents the response when listing pages within a document, typically showing a hierarchical structure.
    /// </summary>
    /// <param name="Pages">A list of <see cref="DocPageListingItem"/> objects, representing the top-level pages and their nested sub-pages.</param>
    public record GetDocPageListingResponse
    (
        [property: JsonPropertyName("pages")] List<DocPageListingItem> Pages
    );
}
