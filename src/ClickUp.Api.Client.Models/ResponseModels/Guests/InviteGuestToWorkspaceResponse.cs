using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response model for inviting a guest to a workspace.
/// </summary>
public record class InviteGuestToWorkspaceResponse
(
    [property: JsonPropertyName("team")]
    InviteGuestToWorkspaceResponseTeam Team
);
