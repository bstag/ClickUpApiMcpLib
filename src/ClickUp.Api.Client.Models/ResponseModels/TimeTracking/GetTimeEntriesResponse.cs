using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.TimeTracking; // For TimeEntry

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking
{
    public record GetTimeEntriesResponse
    (
        // The OpenAPI spec for Gettimeentrieswithinadaterangeresponse has 'data' as a list of 'Datum1'.
        // 'Datum1' is the detailed time entry model.
        [property: JsonPropertyName("data")] List<TimeEntry> Data,
        [property: JsonPropertyName("total_count")] int? TotalCount, // If API provides total count for pagination
        [property: JsonPropertyName("next_page_id")] string? NextPageId // For cursor-based pagination if used
    );
}
