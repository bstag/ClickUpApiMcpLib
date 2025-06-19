using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.ResponseModels.Guests; // For InviteGuestToWorkspaceResponseTeam

namespace ClickUp.Api.Client.Models.ResponseModels.Workspaces;

/// <summary>
/// Represents the response model for inviting a user to a workspace.
/// </summary>
public record class InviteUserToWorkspaceResponse
(
    [property: JsonPropertyName("team")]
    InviteGuestToWorkspaceResponseTeam Team
);
