using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking;

/// <summary>
/// Represents the details of a tag on a time entry.
/// </summary>
public record class TimeEntryTagDetailsResponse
(
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("creator")]
    int Creator,

    [property: JsonPropertyName("tag_bg")]
    string TagBg,

    [property: JsonPropertyName("tag_fg")]
    string TagFg
);
