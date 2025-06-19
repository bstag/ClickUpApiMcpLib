using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Attachments;

/// <summary>
/// Represents the response model for creating a task attachment.
/// </summary>
public record class CreateTaskAttachmentResponse
(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("version")]
    string Version,

    [property: JsonPropertyName("date")]
    long Date,

    [property: JsonPropertyName("title")]
    string Title,

    [property: JsonPropertyName("extension")]
    string Extension,

    [property: JsonPropertyName("thumbnail_small")]
    string ThumbnailSmall,

    [property: JsonPropertyName("thumbnail_large")]
    string ThumbnailLarge,

    [property: JsonPropertyName("url")]
    string Url
);
