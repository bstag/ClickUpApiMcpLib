using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Features entity

namespace ClickUp.Api.Client.Models.RequestModels.Spaces
{
    /// <summary>
    /// Represents the request to create a new Space.
    /// </summary>
    /// <param name="Name">The name of the new Space.</param>
    /// <param name="MultipleAssignees">Optional: Indicates if tasks within this Space can have multiple assignees.</param>
    /// <param name="Features">Optional: Configuration for features to be enabled or disabled for this Space.
    /// Note: The ClickUp API might expect a flat structure for feature flags (e.g., "due_dates_enabled") directly in the request body
    /// rather than a nested object. This model uses a nested <see cref="Entities.Spaces.Features"/> object; adjust if API behavior differs.
    /// </param>
    public record CreateSpaceRequest
    (
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("multiple_assignees")] bool? MultipleAssignees,
        [property: JsonPropertyName("features")] Features? Features
    );
}
