using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Spaces;

/// <summary>
/// Represents the request body for creating or updating a tag in a Space.
/// </summary>
public record ModifyTagRequest
{
    /// <summary>
    /// Gets or sets the name of the tag.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Gets or sets the foreground color of the tag.
    /// </summary>
    [JsonPropertyName("tag_fg")]
    public string TagForegroundColor { get; init; } = null!;

    /// <summary>
    /// Gets or sets the background color of the tag.
    /// </summary>
    [JsonPropertyName("tag_bg")]
    public string TagBackgroundColor { get; init; } = null!;
}
