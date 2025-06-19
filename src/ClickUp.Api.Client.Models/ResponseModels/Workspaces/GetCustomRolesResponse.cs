using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.ResponseModels.Workspaces;

/// <summary>
/// Represents the response model for getting custom roles in a workspace.
/// </summary>
public record class GetCustomRolesResponse
(
    [property: JsonPropertyName("custom_roles")]
    List<CustomRole> CustomRoles
);
