using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.Common
{
    /// <summary>
    /// Represents a member of a workspace or a specific item (like a task or list).
    /// </summary>
    /// <param name="User">The user details of the member.</param>
    /// <param name="Role">The role identifier of the member within the context (e.g., workspace role).</param>
    /// <param name="PermissionLevel">The permission level of the member for a specific shared item.</param>
    public record Member
    (
        [property: JsonPropertyName("user")] User User,
        [property: JsonPropertyName("role")] string? Role,
        [property: JsonPropertyName("permission_level")] string? PermissionLevel
    );
}
