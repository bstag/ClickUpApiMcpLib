using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Docs
{
    public record PageDefaults
    (
        [property: JsonPropertyName("sub_title")] string? SubTitle,
        [property: JsonPropertyName("icon_type")] int? IconType, // 0: none, 1: emoji, 2: image
        [property: JsonPropertyName("icon_value")] string? IconValue, // emoji char or image URL
        [property: JsonPropertyName("cover_image_url")] string? CoverImageUrl,
        [property: JsonPropertyName("cover_image_position")] int? CoverImagePosition, // e.g., 50 for center
        [property: JsonPropertyName("font_family")] string? FontFamily, // e.g., "Inter"
        [property: JsonPropertyName("font_size")] string? FontSize, // e.g., "16px"
        [property: JsonPropertyName("page_width")] string? PageWidth, // e.g., "standard"
        [property: JsonPropertyName("show_comments")] bool? ShowComments,
        [property: JsonPropertyName("show_page_history")] bool? ShowPageHistory,
        [property: JsonPropertyName("show_navigation_bar")] bool? ShowNavigationBar,
        [property: JsonPropertyName("show_task_breadcrumbs")] bool? ShowTaskBreadcrumbs
    );
}
