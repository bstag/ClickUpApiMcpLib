using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.RequestModels.Views;

public class UpdateViewRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("grouping")]
    public ViewGrouping Grouping { get; set; } = new();

    [JsonPropertyName("divide")]
    public ViewDivide Divide { get; set; } = new();

    [JsonPropertyName("sorting")]
    public ViewSorting Sorting { get; set; } = new();

    [JsonPropertyName("filters")]
    public ViewFilters Filters { get; set; } = new();

    [JsonPropertyName("columns")]
    public ViewColumns Columns { get; set; } = new();

    [JsonPropertyName("team_sidebar")]
    public ViewTeamSidebar TeamSidebar { get; set; } = new();

    [JsonPropertyName("settings")]
    public ViewSettings? Settings { get; set; }
}
