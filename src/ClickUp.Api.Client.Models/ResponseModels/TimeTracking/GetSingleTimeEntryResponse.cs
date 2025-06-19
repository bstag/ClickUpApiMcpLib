using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.TimeTracking; // For TimeEntry

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking
{
    public record GetSingleTimeEntryResponse
    (
        // The response schema for Getsingulartimeentryresponse has 'data' as the TimeEntry.
        [property: JsonPropertyName("data")] TimeEntry Data,
        [property: JsonPropertyName("features")] object? Features // OpenAPI spec shows an empty "features" object. Define if it has structure.
    );
}
