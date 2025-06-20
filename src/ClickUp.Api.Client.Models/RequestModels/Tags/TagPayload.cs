using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tags;

public class TagPayload
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("tag_fg")]
    public string TagFg { get; set; } // Foreground color

    [JsonPropertyName("tag_bg")]
    public string TagBg { get; set; } // Background color

    public TagPayload(string name, string tagFg, string tagBg)
    {
        Name = name;
        TagFg = tagFg;
        TagBg = tagBg;
    }
}
