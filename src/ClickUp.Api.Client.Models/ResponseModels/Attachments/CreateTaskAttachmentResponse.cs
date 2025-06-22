using System.Text.Json.Serialization;

using ClickUp.Api.Client.Models.Entities.Users; // Assuming User10 is similar to existing User model


using System.Collections.Generic; // For List if needed for properties like email_data
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Attachments
{
    // Based on OpenAPI spec "CreateAttachmentresponse"
    public record CreateTaskAttachmentResponse
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("version")] string Version,
        [property: JsonPropertyName("date")] DateTimeOffset Date, // Unix timestamp
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("extension")] string Extension,
        [property: JsonPropertyName("thumbnail_small")] string? ThumbnailSmall,
        [property: JsonPropertyName("thumbnail_large")] string? ThumbnailLarge,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("url_w_query")] string UrlWQuery,
        [property: JsonPropertyName("url_w_host")] string UrlWHost,
        [property: JsonPropertyName("is_folder")] bool IsFolder,
        [property: JsonPropertyName("parent_id")] string ParentId,
        [property: JsonPropertyName("size")] long Size, // Assuming long for size in bytes
        [property: JsonPropertyName("total_comments")] int TotalComments,
        [property: JsonPropertyName("resolved_comments")] int ResolvedComments,
        [property: JsonPropertyName("user")] User User, // Assuming User10 from spec maps to our User model
        [property: JsonPropertyName("deleted")] bool Deleted,
        [property: JsonPropertyName("orientation")] object? Orientation, // Define a specific model if structure is known
        [property: JsonPropertyName("type")] int Type,
        [property: JsonPropertyName("source")] int Source,
        [property: JsonPropertyName("email_data")] object? EmailData, // Define a specific model if structure is known
        [property: JsonPropertyName("resource_id")] string ResourceId
    );

    // TODO: Define specific models for Orientation and EmailData if their structure is known and needed.
    // For User, assuming ClickUp.Api.Client.Models.Entities.Users.User is compatible with User10.
    // If User10 has a different structure, a separate User10 model might be needed.
}


