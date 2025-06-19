using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents settings related to the team sidebar for a View.
/// </summary>
public record ViewTeamSidebar
{
    /// <summary>
    /// Indicates if the view or an associated item is pinned in the sidebar.
    /// </summary>
    [JsonPropertyName("pinned")]
    public bool? Pinned { get; init; }

    /// <summary>
    /// Indicates if the sidebar section related to this view is visible.
    /// </summary>
    [JsonPropertyName("visible")]
    public bool? Visible { get; init; }

    // Example from GetView response:
    // "team_sidebar": {
    //   "pinned": false,
    //   "visible": true
    //   // Potentially other settings like "assign_me", "assign_others", "unassigned" for task lists
    // }
    // Adding some common ones, verify with actual schema.

    [JsonPropertyName("assign_me")]
    public bool? AssignMe { get; init; }

    [JsonPropertyName("assign_others")]
    public bool? AssignOthers { get; init; }

    [JsonPropertyName("unassigned")]
    public bool? Unassigned { get; init; }
}
