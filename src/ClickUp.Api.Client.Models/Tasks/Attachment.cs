namespace ClickUp.Api.Client.Models.Tasks;

/// <summary>
/// Represents an attachment in ClickUp.
/// </summary>
public record Attachment
{
    public string Id { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public long Date { get; init; } // Timestamp (Unix milliseconds)
    public string Title { get; init; } = string.Empty;
    public string Extension { get; init; } = string.Empty;
    public string? ThumbnailSmall { get; init; }
    public string? ThumbnailLarge { get; init; }
    public string Url { get; init; } = string.Empty;
    public long? Size { get; init; } // Size in bytes, optional but useful
    public string? Source { get; init; } // Optional: source of the attachment (e.g., 'upload', 'google_drive')
    public string? EmailData { get; init; } // Optional: if attachment is from an email

    // Consider adding a User property if 'uploader' information is available in other contexts or needed.
    // public User? Uploader { get; init; }
}
