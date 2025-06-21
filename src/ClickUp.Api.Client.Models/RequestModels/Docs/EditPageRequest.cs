using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Docs
{
    /// <summary>
    /// Represents the request to edit an existing Page within a Document.
    /// All properties are optional; only provided properties will be updated.
    /// </summary>
    /// <param name="Name">Optional: New name for the page.</param>
    /// <param name="SubTitle">Optional: New subtitle for the page.</param>
    /// <param name="Content">Optional: New content for the page.</param>
    /// <param name="ContentEditMode">Optional: Mode for editing content (e.g., "replace", "append", "prepend"). API specific values (like numeric action_type) should be confirmed.</param>
    /// <param name="ContentFormat">Optional: The format of the new content (e.g., "text/html", "text/md", "application/json").</param>
    /// <param name="OrderIndex">Optional: New order index to change the page's position.</param>
    /// <param name="Hidden">Optional: Set to true to hide the page, false to unhide.</param>
    /// <param name="ParentPageId">Optional: New parent page ID to move this page under a different parent.</param>
    public record EditPageRequest
    (
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("sub_title")] string? SubTitle,
        [property: JsonPropertyName("content")] string? Content,
        [property: JsonPropertyName("content_edit_mode")] string? ContentEditMode,
        [property: JsonPropertyName("content_format")] string? ContentFormat,
        [property: JsonPropertyName("orderindex")] int? OrderIndex,
        [property: JsonPropertyName("hidden")] bool? Hidden,
        [property: JsonPropertyName("parent_page_id")] string? ParentPageId
    );
}
