using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Features entity

namespace ClickUp.Api.Client.Models.RequestModels.Spaces
{
    /// <summary>
    /// Represents the request to update an existing Space.
    /// All properties are optional; only provided properties will be updated.
    /// </summary>
    /// <param name="Name">Optional: New name for the Space.</param>
    /// <param name="Color">Optional: New color for the Space.</param>
    /// <param name="Private">Optional: Set to true to make the Space private, false for public (within the workspace).</param>
    /// <param name="AdminCanManage">Optional: For private Spaces, indicates if admins can manage it.</param>
    /// <param name="MultipleAssignees">Optional: New setting for whether tasks can have multiple assignees.</param>
    /// <param name="Features">Optional: New configuration for features within the Space.
    /// Note: The ClickUp API might expect a flat structure for feature flags; adjust if needed.
    /// </param>
    /// <param name="Archived">Optional: Set to true to archive the Space, false to unarchive.</param>
    public record UpdateSpaceRequest
    (
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("private")] bool? Private,
        [property: JsonPropertyName("admin_can_manage")] bool? AdminCanManage,
        [property: JsonPropertyName("multiple_assignees")] bool? MultipleAssignees,
        [property: JsonPropertyName("features")] Features? Features,
        [property: JsonPropertyName("archived")] bool? Archived
    );
}
