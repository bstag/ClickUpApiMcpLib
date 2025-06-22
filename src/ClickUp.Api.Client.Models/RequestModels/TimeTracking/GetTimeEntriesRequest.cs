using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking;

/// <summary>
/// Represents the request parameters for querying time entries.
/// </summary>
public class GetTimeEntriesRequest
{
    /// <summary>
    /// Gets or sets the start date for filtering time entries.
    /// </summary>
    [JsonPropertyName("start_date")]
    public System.DateTimeOffset? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date for filtering time entries.
    /// </summary>
    [JsonPropertyName("end_date")]
    public System.DateTimeOffset? EndDate { get; set; }

    /// <summary>
    /// Gets or sets a comma-separated string of user IDs to filter time entries by assignee.
    /// </summary>
    [JsonPropertyName("assignee")]
    public string? Assignee { get; set; }

    /// <summary>
    /// Gets or sets the task ID to filter time entries by.
    /// </summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>
    /// Gets or sets the list ID to filter time entries by.
    /// </summary>
    [JsonPropertyName("list_id")]
    public string? ListId { get; set; }

    /// <summary>
    /// Gets or sets the folder ID to filter time entries by.
    /// </summary>
    [JsonPropertyName("folder_id")]
    public string? FolderId { get; set; }

    /// <summary>
    /// Gets or sets the space ID to filter time entries by.
    /// </summary>
    [JsonPropertyName("space_id")]
    public string? SpaceId { get; set; }

    /// <summary>
    /// Gets or sets the project ID to filter time entries by.
    /// Note: In ClickUp, 'project_id' is often an alias for 'folder_id'. Verify API specifics.
    /// </summary>
    [JsonPropertyName("project_id")]
    public string? ProjectId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include task tags in the response.
    /// </summary>
    [JsonPropertyName("include_task_tags")]
    public bool? IncludeTaskTags { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include location names (List, Folder, Space names) in the response.
    /// </summary>
    [JsonPropertyName("include_location_names")]
    public bool? IncludeLocationNames { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include the task URL in the response.
    /// </summary>
    [JsonPropertyName("include_task_url")]
    public bool? IncludeTaskUrl { get; set; }

    /// <summary>
    /// Gets or sets a filter for custom fields. The format is typically a JSON string.
    /// </summary>
    [JsonPropertyName("custom_fields")]
    public string? CustomFields { get; set; }

    /// <summary>
    /// Gets or sets a comma-separated string of custom item IDs to filter by.
    /// </summary>
    [JsonPropertyName("custom_items")]
    public string? CustomItems { get; set; }

    /// <summary>
    /// Gets or sets the page number for pagination if the endpoint supports it.
    /// Note: For endpoints returning <see cref="IAsyncEnumerable{T}"/>, pagination is typically handled internally.
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }
}
