using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Docs
{
    /// <summary>
    /// Represents the default display and formatting settings for a Page in a Document.
    /// </summary>
    /// <param name="SubTitle">The subtitle of the page.</param>
    /// <param name="IconType">The type of icon used for the page (e.g., 0 for none, 1 for emoji, 2 for image).</param>
    /// <param name="IconValue">The value of the icon (e.g., emoji character or image URL).</param>
    /// <param name="CoverImageUrl">The URL of the cover image for the page.</param>
    /// <param name="CoverImagePosition">The position of the cover image (e.g., 50 for center).</param>
    /// <param name="FontFamily">The font family used for the page content (e.g., "Inter").</param>
    /// <param name="FontSize">The font size used for the page content (e.g., "16px").</param>
    /// <param name="PageWidth">The width setting for the page content (e.g., "standard", "full").</param>
    /// <param name="ShowComments">Indicates whether comments are shown on the page.</param>
    /// <param name="ShowPageHistory">Indicates whether page history is shown.</param>
    /// <param name="ShowNavigationBar">Indicates whether the navigation bar is shown.</param>
    /// <param name="ShowTaskBreadcrumbs">Indicates whether task breadcrumbs are shown (if applicable).</param>
    public record PageDefaults
    (
        [property: JsonPropertyName("sub_title")] string? SubTitle,
        [property: JsonPropertyName("icon_type")] int? IconType,
        [property: JsonPropertyName("icon_value")] string? IconValue,
        [property: JsonPropertyName("cover_image_url")] string? CoverImageUrl,
        [property: JsonPropertyName("cover_image_position")] int? CoverImagePosition,
        [property: JsonPropertyName("font_family")] string? FontFamily,
        [property: JsonPropertyName("font_size")] string? FontSize,
        [property: JsonPropertyName("page_width")] string? PageWidth,
        [property: JsonPropertyName("show_comments")] bool? ShowComments,
        [property: JsonPropertyName("show_page_history")] bool? ShowPageHistory,
        [property: JsonPropertyName("show_navigation_bar")] bool? ShowNavigationBar,
        [property: JsonPropertyName("show_task_breadcrumbs")] bool? ShowTaskBreadcrumbs
    );
}
