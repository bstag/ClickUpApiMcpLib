using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking;

/// <summary>
/// Represents the request model for changing tag names from time entries.
/// </summary>
public record class ChangeTagNamesFromTimeEntriesRequest
(
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("new_name")]
    string NewName,

    [property: JsonPropertyName("tag_bg")]
    string TagBg,

    [property: JsonPropertyName("tag_fg")]
    string TagFg
);
