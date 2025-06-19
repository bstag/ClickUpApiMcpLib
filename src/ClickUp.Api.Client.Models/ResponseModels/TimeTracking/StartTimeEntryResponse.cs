using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.TimeTracking; // For TimeEntry

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking
{
    public record StartTimeEntryResponse
    (
        // The response for starting a time entry typically returns the newly created, running TimeEntry.
        // The 'duration' field in the TimeEntry will likely be negative or zero,
        // and 'end' will be null.
        [property: JsonPropertyName("data")] TimeEntry Data
        // OpenAPI spec for StartatimeEntryresponse shows 'data' as TimeEntry.
        // It also shows an empty 'features' object, which can be ignored if always empty
        // or added here if it ever contains data.
        // [property: JsonPropertyName("features")] object? Features
    );
}
