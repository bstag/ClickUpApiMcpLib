using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.UserGroups;

/// <summary>
/// Represents the request model for creating a user group.
/// </summary>
public record class CreateUserGroupRequest
(
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("handle")]
    string? Handle,

    [property: JsonPropertyName("members")]
    List<int> Members
);
