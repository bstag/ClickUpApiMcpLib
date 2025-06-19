using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking.Legacy;

/// <summary>
/// Represents the request model for tracking time (legacy).
/// </summary>
public record class TrackTimeRequest
(
    [property: JsonPropertyName("start")]
    long Start,

    [property: JsonPropertyName("end")]
    long End,

    [property: JsonPropertyName("time")]
    int Time
);
