using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tags;

/// <summary>
/// Represents the detailed payload for a Tag, including its name and colors.
/// </summary>
public class TagPayload
{
    /// <summary>
    /// Gets or sets the name of the tag.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the foreground color of the tag (hexadecimal).
    /// </summary>
    [JsonPropertyName("tag_fg")]
    public string TagFg { get; set; }

    /// <summary>
    /// Gets or sets the background color of the tag (hexadecimal).
    /// </summary>
    [JsonPropertyName("tag_bg")]
    public string TagBg { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TagPayload"/> class.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    /// <param name="tagFg">The foreground color of the tag.</param>
    /// <param name="tagBg">The background color of the tag.</param>
    public TagPayload(string name, string tagFg, string tagBg)
    {
        Name = name;
        TagFg = tagFg;
        TagBg = tagBg;
    }
}
