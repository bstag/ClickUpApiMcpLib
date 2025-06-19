using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking.Legacy;

/// <summary>
/// Represents a legacy time tracking interval.
/// </summary>
public record class LegacyTimeTrackingInterval
(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("start")]
    string? Start,

    [property: JsonPropertyName("end")]
    string? End,

    [property: JsonPropertyName("time")]
    string Time,

    [property: JsonPropertyName("source")]
    string Source,

    [property: JsonPropertyName("date_added")]
    string DateAdded
);
