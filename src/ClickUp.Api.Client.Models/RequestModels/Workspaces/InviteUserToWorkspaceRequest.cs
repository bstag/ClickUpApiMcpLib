using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Workspaces;

/// <summary>
/// Represents the request model for inviting a user to a workspace.
/// </summary>
public record class InviteUserToWorkspaceRequest
(
    [property: JsonPropertyName("email")]
    string Email,

    [property: JsonPropertyName("admin")]
    bool Admin,

    [property: JsonPropertyName("custom_role_id")]
    int? CustomRoleId
);
