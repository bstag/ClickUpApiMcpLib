using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Workspaces;

/// <summary>
/// Represents the response model for getting workspace seats information.
/// </summary>
public record class GetWorkspaceSeatsResponse
(
    [property: JsonPropertyName("members")]
    WorkspaceMemberSeatsInfo Members,

    [property: JsonPropertyName("guests")]
    WorkspaceGuestSeatsInfo Guests
);
