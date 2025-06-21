using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tags;

/// <summary>
/// Represents the request body for creating or updating a Tag.
/// </summary>
public class ModifyTagRequest
{
    /// <summary>
    /// Gets or sets the payload containing the tag's details.
    /// </summary>
    [JsonPropertyName("tag")]
    public TagPayload Tag { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifyTagRequest"/> class.
    /// </summary>
    /// <param name="tag">The tag payload with name and color information.</param>
    public ModifyTagRequest(TagPayload tag)
    {
        Tag = tag;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifyTagRequest"/> class with individual tag properties.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    /// <param name="tagFg">The foreground color of the tag.</param>
    /// <param name="tagBg">The background color of the tag.</param>
    public ModifyTagRequest(string name, string tagFg, string tagBg)
    {
        Tag = new TagPayload(name, tagFg, tagBg);
    }
}
