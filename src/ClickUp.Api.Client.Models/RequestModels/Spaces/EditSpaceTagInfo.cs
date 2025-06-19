using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Spaces;

/// <summary>
/// Represents the information for editing a space tag.
/// </summary>
public record class EditSpaceTagInfo
(
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("fg_color")]
    string FgColor,

    [property: JsonPropertyName("bg_color")]
    string BgColor
);
