using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.TimeTracking;

public class GetTimeEntriesRequest
{
    [JsonPropertyName("start_date")]
    public long? StartDate { get; set; } // Unix time ms

    [JsonPropertyName("end_date")]
    public long? EndDate { get; set; } // Unix time ms

    [JsonPropertyName("assignee")]
    public string? Assignee { get; set; } // Comma-separated user IDs

    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    [JsonPropertyName("list_id")]
    public string? ListId { get; set; }

    [JsonPropertyName("folder_id")]
    public string? FolderId { get; set; }

    [JsonPropertyName("space_id")]
    public string? SpaceId { get; set; }

    // 'project_id' is often an alias for list_id in ClickUp, but check spec if it's distinct here.
    // For now, assuming it might be used if API differentiates.
    [JsonPropertyName("project_id")]
    public string? ProjectId { get; set; }

    [JsonPropertyName("include_task_tags")]
    public bool? IncludeTaskTags { get; set; }

    [JsonPropertyName("include_location_names")]
    public bool? IncludeLocationNames { get; set; }

    [JsonPropertyName("include_task_url")]
    public bool? IncludeTaskUrl { get; set; }

    // Custom fields filter can be complex (e.g., JSON string). Representing as string for now.
    [JsonPropertyName("custom_fields")]
    public string? CustomFields { get; set; }

    [JsonPropertyName("custom_items")]
    public string? CustomItems { get; set; } // Comma-separated custom item IDs

    // 'page' is handled by IAsyncEnumerable or specific pagination parameters in service methods usually, not in this DTO if it's for query params.
    // However, if the API requires 'page' even for non-paginated looking endpoints, it can be added.
    // The GetTimeEntriesAsync method in the interface takes this DTO, and GetTimeEntriesAsyncEnumerableAsync also takes this DTO (but shouldn't have page).
    // For now, including page, as the non-async-enumerable GetTimeEntriesAsync might use it.
    [JsonPropertyName("page")]
    public int? Page { get; set; }
}
