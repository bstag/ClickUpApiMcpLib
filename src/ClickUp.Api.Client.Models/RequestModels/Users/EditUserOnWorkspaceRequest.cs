using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Users;

/// <summary>
/// Represents the request model for editing a user on a workspace.
/// </summary>
public record class EditUserOnWorkspaceRequest
(
    [property: JsonPropertyName("username")]
    string Username,

    [property: JsonPropertyName("admin")]
    bool Admin,

    [property: JsonPropertyName("custom_role_id")]
    int CustomRoleId
);
