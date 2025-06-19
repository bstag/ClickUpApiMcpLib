using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Features entity

namespace ClickUp.Api.Client.Models.RequestModels.Spaces
{
    public record CreateSpaceRequest
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("multiple_assignees")] bool? MultipleAssignees,
        [property: JsonPropertyName("features")] Features? Features // Features object to set for the space
        // Note: OpenAPI spec for CreateSpace has features like "due_dates_enabled" directly in request body.
        // Using a nested Features object is a common pattern but might need adjustment
        // if the API expects a flat structure here. For now, aligning with a structured Features object.
        // If API expects flat, properties from Features record would be listed here directly.
    );
}
