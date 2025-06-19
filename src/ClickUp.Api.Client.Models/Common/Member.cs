using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Common
{
    public record Member
    (
        [property: JsonPropertyName("user")] User User,
        [property: JsonPropertyName("role")] string? Role, // Assuming role might be represented as a string
        [property: JsonPropertyName("permission_level")] string? PermissionLevel // Assuming permission_level might be a string
    );
}
