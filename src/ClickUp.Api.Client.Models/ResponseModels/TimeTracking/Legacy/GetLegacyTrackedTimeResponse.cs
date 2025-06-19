using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking.Legacy;

/// <summary>
/// Represents the response model for getting legacy tracked time.
/// </summary>
public record class GetLegacyTrackedTimeResponse
(
    [property: JsonPropertyName("data")]
    List<LegacyTrackedTimeEntry> Data
);
