using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.UserGroups;

/// <summary>
/// Represents the request model for updating a user group.
/// </summary>
public record class UpdateUserGroupRequest
(
    [property: JsonPropertyName("name")]
    string? Name,

    [property: JsonPropertyName("handle")]
    string? Handle,

    [property: JsonPropertyName("members")]
    UserGroupMembersUpdate? Members
);
