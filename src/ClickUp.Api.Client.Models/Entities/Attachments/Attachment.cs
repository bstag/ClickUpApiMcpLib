using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.Entities.Attachments
{
    /// <summary>
    /// Represents an attachment in ClickUp, which can be a file or an email attachment.
    /// </summary>
    /// <param name="Id">The unique identifier of the attachment.</param>
    /// <param name="Version">The version identifier of the attachment.</param>
    /// <param name="Date">The date the attachment was created, as a Unix timestamp (milliseconds).</param>
    /// <param name="Title">The title or filename of the attachment.</param>
    /// <param name="Extension">The file extension of the attachment.</param>
    /// <param name="ThumbnailSmall">The URL to a small thumbnail of the attachment, if available.</param>
    /// <param name="ThumbnailLarge">The URL to a large thumbnail of the attachment, if available.</param>
    /// <param name="Url">The URL to download the attachment.</param>
    /// <param name="Orientation">The orientation of the attachment if it's an image (e.g., "portrait", "landscape").</param>
    /// <param name="Type">An integer representing the type of the attachment.</param>
    /// <param name="Source">An integer representing the source of the attachment.</param>
    /// <param name="EmailFrom">The sender's email address, if the attachment is from an email.</param>
    /// <param name="EmailTo">A list of recipient email addresses, if the attachment is from an email.</param>
    /// <param name="EmailCc">A list of CC recipient email addresses, if the attachment is from an email.</param>
    /// <param name="EmailReplyTo">The reply-to email address, if the attachment is from an email.</param>
    /// <param name="EmailDate">The date of the email, if the attachment is from an email.</param>
    /// <param name="EmailSubject">The subject of the email, if the attachment is from an email.</param>
    /// <param name="EmailPreview">A preview text of the email content, if the attachment is from an email.</param>
    /// <param name="EmailTextContent">The plain text content of the email, if the attachment is from an email.</param>
    /// <param name="EmailHtmlContentId">An identifier for the HTML content of the email, if available.</param>
    /// <param name="EmailAttachmentsCount">The count of attachments in the email, if the attachment is from an email.</param>
    /// <param name="User">The user who uploaded or created the attachment.</param>
    public record Attachment
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("version")] string Version,
        [property: JsonPropertyName("date")] long Date,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("extension")] string Extension,
        [property: JsonPropertyName("thumbnail_small")] string? ThumbnailSmall,
        [property: JsonPropertyName("thumbnail_large")] string? ThumbnailLarge,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("orientation")] string? Orientation,
        [property: JsonPropertyName("type")] int? Type,
        [property: JsonPropertyName("source")] int? Source,
        [property: JsonPropertyName("email_from")] string? EmailFrom,
        [property: JsonPropertyName("email_to")] List<string>? EmailTo,
        [property: JsonPropertyName("email_cc")] List<string>? EmailCc,
        [property: JsonPropertyName("email_reply_to")] string? EmailReplyTo,
        [property: JsonPropertyName("email_date")] string? EmailDate,
        [property: JsonPropertyName("email_subject")] string? EmailSubject,
        [property: JsonPropertyName("email_preview")] string? EmailPreview,
        [property: JsonPropertyName("email_text_content")] string? EmailTextContent,
        [property: JsonPropertyName("email_html_content_id")] string? EmailHtmlContentId,
        [property: JsonPropertyName("email_attachments_count")] int? EmailAttachmentsCount,
        [property: JsonPropertyName("user")] User? User
    );
}
