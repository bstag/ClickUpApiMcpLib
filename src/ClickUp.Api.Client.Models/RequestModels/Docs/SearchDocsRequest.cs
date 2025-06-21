using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Docs;

/// <summary>
/// Represents the request model for searching documents.
/// </summary>
public class SearchDocsRequest
{
    /// <summary>
    /// Gets or sets the search query string.
    /// </summary>
    [JsonPropertyName("query")]
    public string Query { get; set; } = null!;

    /// <summary>
    /// Gets or sets a list of Space IDs to filter the search by.
    /// </summary>
    [JsonPropertyName("space_ids")]
    public List<string>? SpaceIds { get; set; }

    /// <summary>
    /// Gets or sets a list of Folder IDs to filter the search by.
    /// </summary>
    [JsonPropertyName("folder_ids")]
    public List<string>? FolderIds { get; set; }

    /// <summary>
    /// Gets or sets a list of List IDs to filter the search by.
    /// </summary>
    [JsonPropertyName("list_ids")]
    public List<string>? ListIds { get; set; }

    /// <summary>
    /// Gets or sets a list of CuTask IDs to filter the search by (e.g., docs linked to these tasks).
    /// </summary>
    [JsonPropertyName("task_ids")]
    public List<string>? TaskIds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include archived documents in the search results.
    /// </summary>
    [JsonPropertyName("include_archived")]
    public bool? IncludeArchived { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of documents to return.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the cursor for pagination, used to retrieve the next set of results.
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}
