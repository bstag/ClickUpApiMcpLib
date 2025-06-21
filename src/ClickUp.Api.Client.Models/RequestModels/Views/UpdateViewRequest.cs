using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.RequestModels.Views;

/// <summary>
/// Represents the request model for updating an existing View.
/// All properties are typically required to define the new state of the view.
/// If only partial updates are allowed by the API, some properties might be made nullable.
/// </summary>
public class UpdateViewRequest
{
    /// <summary>
    /// Gets or sets the name of the view.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of the view (e.g., "list", "board", "calendar").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

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
