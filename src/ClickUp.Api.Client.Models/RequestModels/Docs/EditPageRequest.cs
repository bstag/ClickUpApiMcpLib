using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Docs
{
    public record EditPageRequest
    (
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("sub_title")] string? SubTitle,
        [property: JsonPropertyName("content")] string? Content, // The new page content
        [property: JsonPropertyName("content_edit_mode")] string? ContentEditMode, // e.g., "replace", "append", "prepend" (API specific)
                                                                               // ClickUp v3 uses "action_type": 0 (replace), 1 (append), 2 (prepend)
                                                                               // and "value" for content, "format" for content_format.
                                                                               // This model might need to be adjusted if it's for a specific patch-like operation.
                                                                               // For a general PUT/Edit, sending the new content is typical.
        [property: JsonPropertyName("content_format")] string? ContentFormat, // e.g., "text/html", "text/md", "application/json"
        [property: JsonPropertyName("orderindex")] int? OrderIndex, // Optional: to change page order
        [property: JsonPropertyName("hidden")] bool? Hidden, // Optional: to hide/unhide the page
        [property: JsonPropertyName("parent_page_id")] string? ParentPageId // Optional: to move the page under a new parent
    );
}
