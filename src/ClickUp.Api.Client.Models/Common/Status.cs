using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Common
{
    public record Status
    (
        [property: JsonPropertyName("status")] string StatusValue, // Name kept as StatusValue to avoid conflict with type
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("orderindex")] int OrderIndex, // Assuming orderindex is generally required for statuses
        [property: JsonPropertyName("type")] string? Type // e.g. "open", "custom", "closed"
    );
}
