using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Docs
{
    public record DocPageListingItem
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("doc_id")] string DocId,
        [property: JsonPropertyName("parent_page_id")] string? ParentPageId,
        [property: JsonPropertyName("workspace_id")] long WorkspaceId, // Assuming long based on typical ID types
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("orderindex")] int? OrderIndex, // Order of the page
        [property: JsonPropertyName("hidden")] bool? Hidden, // If the page is hidden
        [property: JsonPropertyName("pages")] List<DocPageListingItem>? Pages // For nested pages
    );
}
