using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Views; // For sub-DTOs like ViewGrouping, ViewFilters etc.

namespace ClickUp.Api.Client.Models.RequestModels.Views;

public class CreateViewRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    // Parent is usually determined by the endpoint (e.g., list_id, folder_id) and not part of request body for creation.
    // [JsonPropertyName("parent")]
    // public ViewParent? Parent { get; set; }

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
