using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Attachments
{
    public record Attachment
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("version")] string Version, // Assuming string, could be int
        [property: JsonPropertyName("date")] long Date, // Assuming Unix timestamp
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("extension")] string Extension,
        [property: JsonPropertyName("thumbnail_small")] string? ThumbnailSmall,
        [property: JsonPropertyName("thumbnail_large")] string? ThumbnailLarge,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("orientation")] string? Orientation, // Added as it's often part of attachment info
        [property: JsonPropertyName("type")] int? Type, // Added as it's often part of attachment info
        [property: JsonPropertyName("source")] int? Source, // Added as it's often part of attachment info
        [property: JsonPropertyName("email_from")] string? EmailFrom, // If from email attachment
        [property: JsonPropertyName("email_to")] List<string>? EmailTo, // If from email attachment
        [property: JsonPropertyName("email_cc")] List<string>? EmailCc, // If from email attachment
        [property: JsonPropertyName("email_reply_to")] string? EmailReplyTo, // If from email attachment
        [property: JsonPropertyName("email_date")] string? EmailDate, // If from email attachment
        [property: JsonPropertyName("email_subject")] string? EmailSubject, // If from email attachment
        [property: JsonPropertyName("email_preview")] string? EmailPreview, // If from email attachment
        [property: JsonPropertyName("email_text_content")] string? EmailTextContent, // If from email attachment
        [property: JsonPropertyName("email_html_content_id")] string? EmailHtmlContentId, // If from email attachment
        [property: JsonPropertyName("email_attachments_count")] int? EmailAttachmentsCount, // If from email attachment
        [property: JsonPropertyName("user")] Common.ComUser? User // User who uploaded, assuming User model exists in Common
    );
}
