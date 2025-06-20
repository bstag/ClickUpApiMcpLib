using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Spaces;

public record EditSpaceTagRequest
(
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("tag_fg")]
    string TagFg,

    [property: JsonPropertyName("tag_bg")]
    string TagBg
);
