using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Workspaces;

/// <summary>
/// Represents information about member seats in a workspace.
/// </summary>
public record class WorkspaceMemberSeatsInfo
(
    [property: JsonPropertyName("filled_members_seats")]
    int FilledMembersSeats,

    [property: JsonPropertyName("total_member_seats")]
    int TotalMemberSeats,

    [property: JsonPropertyName("empty_member_seats")]
    int EmptyMemberSeats
);
