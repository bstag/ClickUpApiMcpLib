using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.UserGroups;

/// <summary>
/// Represents the members to add or remove from a user group.
/// </summary>
public record class UpdateUserGroupMembersRequest
(
    [property: JsonPropertyName("add")]
    List<int>? Add,

    [property: JsonPropertyName("rem")]
    List<int>? Rem
);
