using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Docs;

/// <summary>
/// Represents the request model for searching documents.
/// </summary>
public record SearchDocsRequest
{
    /// <summary>
    /// Gets or sets the search query string.
    /// </summary>
    [JsonPropertyName("q")] // Corrected based on usage in DocsService.cs
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets a list of Space IDs to filter the search by.
    /// </summary>
    [JsonPropertyName("space_ids")]
    public List<string>? SpaceIds { get; init; }

    /// <summary>
    /// Gets or sets a list of Folder IDs to filter the search by.
    /// </summary>
    [JsonPropertyName("folder_ids")]
    public List<string>? FolderIds { get; init; }

    /// <summary>
    /// Gets or sets a list of List IDs to filter the search by.
    /// </summary>
    [JsonPropertyName("list_ids")]
    public List<string>? ListIds { get; init; }

    /// <summary>
    /// Gets or sets a list of CuTask IDs to filter the search by (e.g., docs linked to these tasks).
    /// </summary>
    [JsonPropertyName("task_ids")]
    public List<string>? TaskIds { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to include archived documents in the search results.
    /// Corresponds to the 'archived' query parameter.
    /// </summary>
    [JsonPropertyName("archived")]
    public bool? IncludeArchived { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of documents to return.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; init; }

    /// <summary>
    /// Gets or sets the cursor for pagination, used to retrieve the next set of results.
    /// </summary>
    [JsonPropertyName("nect_cursor")]
    public string? NextCursor { get; init; }

    /// <summary>
    /// Gets or sets the Parent ID to filter by (e.g., Doc ID for pages).
    /// </summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; init; }

    /// <summary>
    /// Gets or sets the Parent Type (e.g., 1 for Doc, 2 for Page).
    /// </summary>
    [JsonPropertyName("parent_type")]
    public int? ParentType { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to include deleted documents in the search results.
    /// Corresponds to the 'deleted' query parameter.
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool? IncludeDeleted { get; init; }

    /// <summary>
    /// Gets or sets the Creator ID to filter by.
    /// Corresponds to the 'creator' query parameter.
    /// </summary>
    [JsonPropertyName("creator")]
    public int? CreatorId { get; init; }
}
