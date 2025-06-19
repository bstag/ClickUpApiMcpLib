using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Workspaces;

/// <summary>
/// Represents information about guest seats in a workspace.
/// </summary>
public record class WorkspaceGuestSeatsInfo
(
    [property: JsonPropertyName("filled_guest_seats")]
    int FilledGuestSeats,

    [property: JsonPropertyName("total_guest_seats")]
    int TotalGuestSeats,

    [property: JsonPropertyName("empty_guest_seats")]
    int EmptyGuestSeats
);
