using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Features entity

namespace ClickUp.Api.Client.Models.RequestModels.Spaces
{
    public record UpdateSpaceRequest
    (
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("private")] bool? Private,
        [property: JsonPropertyName("admin_can_manage")] bool? AdminCanManage, // For private spaces
        [property: JsonPropertyName("multiple_assignees")] bool? MultipleAssignees,
        [property: JsonPropertyName("features")] Features? Features, // Features object to update
        [property: JsonPropertyName("archived")] bool? Archived // To archive or unarchive the space
    );
}
