using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.ResponseModels.Guests; // For RemoveGuestFromWorkspaceResponseTeam

namespace ClickUp.Api.Client.Models.ResponseModels.Workspaces;

/// <summary>
/// Represents the response model for removing a user from a workspace.
/// </summary>
public record class RemoveUserFromWorkspaceResponse
(
    [property: JsonPropertyName("team")]
    RemoveGuestFromWorkspaceResponseTeam Team
);
