using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents settings related to the team sidebar for a View.
/// </summary>
public record ViewTeamSidebar
{
    /// <summary>
    /// Gets a value indicating whether the view or an associated item is pinned in the team sidebar.
    /// </summary>
    [JsonPropertyName("pinned")]
    public bool? Pinned { get; init; }

    /// <summary>
    /// Gets a value indicating whether the sidebar section related to this view is visible.
    /// </summary>
    [JsonPropertyName("visible")]
    public bool? Visible { get; init; }

    /// <summary>
    /// Gets a value indicating whether the "Assign Me" filter or section is active in the sidebar context.
    /// </summary>
    [JsonPropertyName("assign_me")]
    public bool? AssignMe { get; init; }

    /// <summary>
    /// Gets a value indicating whether the "Assign Others" filter or section is active in the sidebar context.
    /// </summary>
    [JsonPropertyName("assign_others")]
    public bool? AssignOthers { get; init; }

    /// <summary>
    /// Gets a value indicating whether the "Unassigned" filter or section is active in the sidebar context.
    /// </summary>
    [JsonPropertyName("unassigned")]
    public bool? Unassigned { get; init; }
}
