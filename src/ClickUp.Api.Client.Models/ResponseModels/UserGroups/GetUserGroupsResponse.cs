using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.UserGroups;

namespace ClickUp.Api.Client.Models.ResponseModels.UserGroups;

/// <summary>
/// Represents the response model for getting user groups.
/// </summary>
public record class GetUserGroupsResponse
(
    [property: JsonPropertyName("groups")]
    List<UserGroup> Groups
);
