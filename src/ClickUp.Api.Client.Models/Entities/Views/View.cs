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
/// <param name="Id">The unique identifier of the view.</param>
/// <param name="Name">The name of the view.</param>
/// <param name="Type">The type of the view (e.g., "list", "board", "calendar").</param>
public record View(
    [property: JsonPropertyName("id")]
    string Id,

    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("type")]
    string Type
)
{
    /// <summary>
    /// Gets the parent entity (Space, Folder, or List) of this view.
    /// </summary>
    [JsonPropertyName("parent")]
    public ViewParent? Parent { get; init; }

    /// <summary>
    /// Gets the grouping settings for this view.
    /// </summary>
    [JsonPropertyName("grouping")]
    public ViewGrouping? Grouping { get; init; }

    /// <summary>
    /// Gets the dividing or sectioning settings for this view (e.g., columns in a Board view).
    /// </summary>
    [JsonPropertyName("divide")]
    public ViewDivide? Divide { get; init; }

    /// <summary>
    /// Gets the sorting settings for this view.
    /// </summary>
    [JsonPropertyName("sorting")]
    public ViewSorting? Sorting { get; init; }

    /// <summary>
    /// Gets the filter criteria applied to this view.
    /// </summary>
    [JsonPropertyName("filters")]
    public ViewFilters? Filters { get; init; }

    /// <summary>
    /// Gets the column display settings for this view (relevant for List, Table views).
    /// </summary>
    [JsonPropertyName("columns")]
    public ViewColumns? Columns { get; init; }

    /// <summary>
    /// Gets settings related to the team sidebar for this view.
    /// </summary>
    [JsonPropertyName("team_sidebar")]
    public ViewTeamSidebar? TeamSidebar { get; init; }

    /// <summary>
    /// Gets various view-specific settings.
    /// </summary>
    [JsonPropertyName("settings")]
    public ViewSettings? Settings { get; init; }

    /// <summary>
    /// Gets the timestamp when the view was created.
    /// </summary>
    [JsonPropertyName("date_created")]
    public DateTimeOffset? DateCreated { get; init; }

    /// <summary>
    /// Gets the identifier of the user who created the view.
    /// </summary>
    [JsonPropertyName("creator")]
    public int? Creator { get; init; }

    /// <summary>
    /// Gets the order index of the view.
    /// </summary>
    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; init; }

    /// <summary>
    /// Gets the count of tasks visible in this view.
    /// </summary>
    [JsonPropertyName("task_count")]
    public int? TaskCount { get; init; }

    /// <summary>
    /// Gets a value indicating whether the view is protected (e.g., cannot be easily deleted or modified by non-admins).
    /// </summary>
    [JsonPropertyName("protected")]
    public bool? Protected { get; init; }

    /// <summary>
    /// Gets the identifier of the Space this view belongs to, if applicable.
    /// </summary>
    [JsonPropertyName("space_id")]
    public string? SpaceId { get; init; }

    /// <summary>
    /// Gets the identifier of the Folder this view belongs to, if applicable.
    /// </summary>
    [JsonPropertyName("folder_id")]
    public string? FolderId { get; init; }

    /// <summary>
    /// Gets the identifier of the List this view belongs to, if applicable.
    /// </summary>
    [JsonPropertyName("list_id")]
    public string? ListId { get; init; }
}
