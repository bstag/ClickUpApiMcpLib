using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking.Legacy;

/// <summary>
/// Represents the response model for tracking time (legacy).
/// </summary>
public record class TrackTimeResponse
(
    [property: JsonPropertyName("id")]
    string Id
);
