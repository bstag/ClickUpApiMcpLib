using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Docs
{
    /// <summary>
    /// Represents an item in a document's page listing, which can be a page or a nested page.
    /// </summary>
    /// <param name="Id">The unique identifier of the page.</param>
    /// <param name="DocId">The identifier of the parent document this page belongs to.</param>
    /// <param name="ParentPageId">The identifier of the parent page if this is a sub-page. Null for top-level pages.</param>
    /// <param name="WorkspaceId">The identifier of the workspace this page belongs to.</param>
    /// <param name="Name">The name of the page.</param>
    /// <param name="OrderIndex">The order index of the page within its parent (document or parent page).</param>
    /// <param name="Hidden">Indicates whether the page is hidden.</param>
    /// <param name="Pages">A list of nested sub-pages. Null if no sub-pages or not expanded.</param>
    public record DocPageListingItem
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("doc_id")] string DocId,
        [property: JsonPropertyName("parent_page_id")] string? ParentPageId,
        [property: JsonPropertyName("workspace_id")] long WorkspaceId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("orderindex")] int? OrderIndex,
        [property: JsonPropertyName("hidden")] bool? Hidden,
        [property: JsonPropertyName("pages")] List<DocPageListingItem>? Pages
    );
}
