using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Docs
{
    /// <summary>
    /// Represents the request to create a new Page within a Document.
    /// </summary>
    /// <param name="ParentPageId">Optional: The ID of the parent page if this is a sub-page.</param>
    /// <param name="Name">The name of the new page.</param>
    /// <param name="SubTitle">Optional: The subtitle for the page.</param>
    /// <param name="Content">The content of the page.</param>
    /// <param name="ContentFormat">The format of the content (e.g., "text/html", "text/md", "application/json" for ClickUp's block format).</param>
    /// <param name="OrderIndex">Optional: The order index to specify the page's position.</param>
    /// <param name="Hidden">Optional: Whether the page should be hidden.</param>
    /// <param name="TemplateId">Optional: The ID of a page template to use for creating this page.</param>
    public record CreatePageRequest
    (
        [property: JsonPropertyName("parent_page_id")] string? ParentPageId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("sub_title")] string? SubTitle,
        [property: JsonPropertyName("content")] string Content,
        [property: JsonPropertyName("content_format")] string ContentFormat,
        [property: JsonPropertyName("orderindex")] int? OrderIndex,
        [property: JsonPropertyName("hidden")] bool? Hidden,
        [property: JsonPropertyName("template_id")] string? TemplateId
    );
}
