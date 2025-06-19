using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Docs
{
    public record CreatePageRequest
    (
        [property: JsonPropertyName("parent_page_id")] string? ParentPageId, // Optional, for creating a sub-page
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("sub_title")] string? SubTitle,
        [property: JsonPropertyName("content")] string Content, // The actual page content
        [property: JsonPropertyName("content_format")] string ContentFormat, // e.g., "text/html", "text/md", "application/json" (for ClickUp's block format)
        [property: JsonPropertyName("orderindex")] int? OrderIndex, // Optional: to specify page order
        [property: JsonPropertyName("hidden")] bool? Hidden, // Optional: whether the page should be hidden
        [property: JsonPropertyName("template_id")] string? TemplateId // Optional: create from a page template
    );
}
