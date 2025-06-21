using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views; // For sub-DTOs like ViewGrouping, ViewFilters etc.

namespace ClickUp.Api.Client.Models.RequestModels.Views;

/// <summary>
/// Represents the request model for creating a new View.
/// </summary>
public class CreateViewRequest
{
    /// <summary>
    /// Gets or sets the name of the new view.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of the new view (e.g., "list", "board", "calendar").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    // Parent information (like list_id, folder_id, or space_id) is typically part of the URL endpoint
    // for view creation, not usually in the request body itself.
    // [JsonPropertyName("parent")]
    // public ViewParent? Parent { get; set; }

    /// <summary>
    /// Gets or sets the grouping settings for the view.
    /// </summary>
    [JsonPropertyName("grouping")]
    public ViewGrouping Grouping { get; set; } = new();

    /// <summary>
    /// Gets or sets the dividing (sectioning) settings for the view.
    /// </summary>
    [JsonPropertyName("divide")]
    public ViewDivide Divide { get; set; } = new();

    /// <summary>
    /// Gets or sets the sorting settings for the view.
    /// </summary>
    [JsonPropertyName("sorting")]
    public ViewSorting Sorting { get; set; } = new();

    /// <summary>
    /// Gets or sets the filter criteria for the view.
    /// </summary>
    [JsonPropertyName("filters")]
    public ViewFilters Filters { get; set; } = new();

    /// <summary>
    /// Gets or sets the column display settings for the view.
    /// </summary>
    [JsonPropertyName("columns")]
    public ViewColumns Columns { get; set; } = new();

    /// <summary>
    /// Gets or sets the team sidebar settings for the view.
    /// </summary>
    [JsonPropertyName("team_sidebar")]
    public ViewTeamSidebar TeamSidebar { get; set; } = new();

    /// <summary>
    /// Gets or sets various view-specific settings.
    /// </summary>
    [JsonPropertyName("settings")]
    public ViewSettings? Settings { get; set; }
}
