using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Workspaces;

/// <summary>
/// Represents the response model for getting workspace seats information.
/// </summary>
public record class GetWorkspaceSeatsResponse
(
    [property: JsonPropertyName("members")]
    WorkspaceMemberSeatsInfoResponse Members,

    [property: JsonPropertyName("guests")]
    WorkspaceGuestSeatsInfoResponse Guests
);
