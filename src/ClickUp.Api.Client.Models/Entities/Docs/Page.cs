using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Docs
{
    /// <summary>
    /// Represents a single Page within a ClickUp Document.
    /// </summary>
    /// <param name="Id">The unique identifier of the page.</param>
    /// <param name="DocId">The identifier of the document this page belongs to.</param>
    /// <param name="Name">The name or title of the page.</param>
    /// <param name="Content">The content of the page, often in a specific format like Markdown or HTML. Can be null if not requested or available.</param>
    public record Page
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("doc_id")] string DocId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("content")] string? Content
    );
}
