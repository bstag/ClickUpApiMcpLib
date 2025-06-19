using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Attachments;

/// <summary>
/// Represents the response for creating a task attachment.
/// Based on the inline schema `CreateTaskAttachmentresponse` from POST /v2/task/{task_id}/attachment.
/// </summary>
public record CreateTaskAttachmentResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("version")]
    public string Version { get; init; }

    /// <summary>
    /// Unix timestamp of the attachment date.
    /// </summary>
    [JsonPropertyName("date")]
    public long Date { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; }

    [JsonPropertyName("extension")]
    public string Extension { get; init; }

    [JsonPropertyName("thumbnail_small")]
    public string? ThumbnailSmall { get; init; }

    [JsonPropertyName("thumbnail_large")]
    public string? ThumbnailLarge { get; init; }

    [JsonPropertyName("url")]
    public string Url { get; init; }

    // The provided schema in the prompt doesn't list 'url_w_redirect' or 'email_data',
    // but sometimes attachment responses include these.
    // For now, sticking to the explicitly mentioned fields.
    // If the full schema has more, they can be added.
    // Example from a similar GET attachment might have:
    // "url_w_redirect": "string",
    // "email_data": "string",
    // "orientation": null, // or object
    // "source": "string",
    // "is_folder": false,
    // "size": 0, // integer
    // "type": 0, // integer
    // "hidden_by": null, // or object
    // "parent_id": "string",
    // "user": { "id": 0, ... }
    // The CreateTaskAttachmentresponse is usually simpler than a full Attachment object.
}
