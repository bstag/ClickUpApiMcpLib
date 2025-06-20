using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tags;

public class ModifyTagRequest
{
    [JsonPropertyName("tag")]
    public TagPayload Tag { get; set; }

    public ModifyTagRequest(TagPayload tag)
    {
        Tag = tag;
    }

    // Optional: Add a constructor for convenience if you often create this with Name, Fg, Bg directly
    public ModifyTagRequest(string name, string tagFg, string tagBg)
    {
        Tag = new TagPayload(name, tagFg, tagBg);
    }
}
