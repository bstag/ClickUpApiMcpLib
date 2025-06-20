using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Spaces;

/// <summary>
/// Represents the payload for creating or updating a tag.
/// </summary>
public record TagPayload
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    [JsonPropertyName("tag_fg")]
    public string TagFg { get; init; } = null!;

    [JsonPropertyName("tag_bg")]
    public string TagBg { get; init; } = null!;
}

/// <summary>
/// Represents the request body for creating or updating a tag in a Space.
/// </summary>
public record ModifyTagRequest
{
    [JsonPropertyName("tag")]
    public TagPayload Tag { get; init; } = null!;
}
