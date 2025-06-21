using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For User if needed
// Assuming other nested types will be in the same namespace:
// using ClickUp.Api.Client.Models.Entities.Views;

namespace ClickUp.Api.Client.Models.Entities.Views;

/// <summary>
/// Represents a View in ClickUp.
/// Based on GetViewresponse schema and typical View object structure.
/// </summary>
public record View(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("name")]
    string Name,

    /// <summary>
    /// Type of the view (e.g., "list", "board", "calendar", "gantt", "table", "workload", "activity", "map", "chat", "doc", "form", "embed", "timeline", "whiteboard").
    /// </summary>
    [property: JsonPropertyName("type")]
    string Type
)
{
    [JsonPropertyName("parent")]
    public ViewParent? Parent { get; init; }

    [JsonPropertyName("grouping")]
    public ViewGrouping? Grouping { get; init; }

    [JsonPropertyName("divide")]
    public ViewDivide? Divide { get; init; }

    [JsonPropertyName("sorting")]
    public ViewSorting? Sorting { get; init; }

    [JsonPropertyName("filters")]
    public ViewFilters? Filters { get; init; }

    [JsonPropertyName("columns")]
    public ViewColumns? Columns { get; init; }

    [JsonPropertyName("team_sidebar")]
    public ViewTeamSidebar? TeamSidebar { get; init; }

    [JsonPropertyName("settings")]
    public ViewSettings? Settings { get; init; }

    [JsonPropertyName("date_created")]
    public long? DateCreated { get; init; } // Unix timestamp

    [JsonPropertyName("creator")]
    public int? Creator { get; init; } // User ID

    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; init; } // Or float/double? Spec check needed. Assuming int for now.

    [JsonPropertyName("task_count")]
    public int? TaskCount { get; init; }

    [JsonPropertyName("protected")]
    public bool? Protected { get; init; }

    // Additional properties might be found in the specific View schema definition
    // For example, from GetViewresponse:
    [JsonPropertyName("space_id")]
    public string? SpaceId { get; init; }

    [JsonPropertyName("folder_id")]
    public string? FolderId { get; init; }

    [JsonPropertyName("list_id")]
    public string? ListId { get; init; }

    // Add other properties as per the full View schema from OpenAPI spec
}
