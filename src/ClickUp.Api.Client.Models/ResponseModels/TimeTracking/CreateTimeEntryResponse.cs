using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.TimeTracking; // For TimeEntry

namespace ClickUp.Api.Client.Models.ResponseModels.TimeTracking
{
    public record CreateTimeEntryResponse
    (
        // The response schema for Createatimeentryresponse has 'data' as the created TimeEntry.
        [property: JsonPropertyName("data")] TimeEntry Data,
        [property: JsonPropertyName("features")] object? Features // OpenAPI spec shows an empty "features" object. Define if it has structure.
    );
}
