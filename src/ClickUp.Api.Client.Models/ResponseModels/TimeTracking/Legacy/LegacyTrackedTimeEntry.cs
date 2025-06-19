using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking.Legacy;

/// <summary>
/// Represents a legacy tracked time entry.
/// </summary>
public record class LegacyTrackedTimeEntry
(
    [property: JsonPropertyName("user")]
    User User,

    [property: JsonPropertyName("time")]
    int Time,

    [property: JsonPropertyName("intervals")]
    List<LegacyTimeTrackingInterval> Intervals
);
