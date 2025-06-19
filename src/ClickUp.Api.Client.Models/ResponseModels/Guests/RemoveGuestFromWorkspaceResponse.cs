using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Guests;

/// <summary>
/// Represents the response model for removing a guest from a workspace.
/// </summary>
public record class RemoveGuestFromWorkspaceResponse
(
    [property: JsonPropertyName("team")]
    RemoveGuestFromWorkspaceResponseTeam Team
);
