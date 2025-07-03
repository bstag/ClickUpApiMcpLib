using System;
using System.Collections.Generic;
using ClickUp.Api.Client.Models.Common.ValueObjects;

namespace ClickUp.Api.Client.Models.Parameters;

/// <summary>
/// Represents parameters for retrieving time entries.
/// </summary>
public class GetTimeEntriesRequestParameters
{
    public TimeRange? TimeRange { get; set; }
    public long? AssigneeUserId { get; set; }
    public string? TaskId { get; set; }
    public string? ListId { get; set; }
    public string? FolderId { get; set; }
    public string? SpaceId { get; set; }
    public bool? IncludeTaskTags { get; set; }
    public bool? IncludeLocationNames { get; set; }
    public int? Page { get; set; }
    public bool? IncludeTimers { get; set; }

    // Parameters for custom task ID handling, if applicable to the service method
    public bool? CustomTaskIds { get; set; }
    public string? TeamIdForCustomTaskIds { get; set; } // Workspace ID for custom task IDs

    public List<KeyValuePair<string, string>> ToQueryParametersList()
    {
        var parameters = new List<KeyValuePair<string, string>>();

        if (TimeRange != null)
        {
            // API uses start_date and end_date for this endpoint
            parameters.Add(new KeyValuePair<string, string>("start_date", TimeRange.StartDate.ToUnixTimeMilliseconds().ToString()));
            parameters.Add(new KeyValuePair<string, string>("end_date", TimeRange.EndDate.ToUnixTimeMilliseconds().ToString()));
        }

        if (AssigneeUserId.HasValue) parameters.Add(new KeyValuePair<string, string>("assignee", AssigneeUserId.Value.ToString()));
        if (!string.IsNullOrWhiteSpace(TaskId)) parameters.Add(new KeyValuePair<string, string>("task_id", TaskId));
        if (!string.IsNullOrWhiteSpace(ListId)) parameters.Add(new KeyValuePair<string, string>("list_id", ListId));
        if (!string.IsNullOrWhiteSpace(FolderId)) parameters.Add(new KeyValuePair<string, string>("folder_id", FolderId));
        if (!string.IsNullOrWhiteSpace(SpaceId)) parameters.Add(new KeyValuePair<string, string>("space_id", SpaceId));

        if (IncludeTaskTags.HasValue) parameters.Add(new KeyValuePair<string, string>("include_task_tags", IncludeTaskTags.Value.ToString().ToLowerInvariant()));
        if (IncludeLocationNames.HasValue) parameters.Add(new KeyValuePair<string, string>("include_location_names", IncludeLocationNames.Value.ToString().ToLowerInvariant()));
        if (Page.HasValue) parameters.Add(new KeyValuePair<string, string>("page", Page.Value.ToString()));
        if (IncludeTimers.HasValue) parameters.Add(new KeyValuePair<string, string>("include_timers", IncludeTimers.Value.ToString().ToLowerInvariant()));

        if (CustomTaskIds.HasValue) parameters.Add(new KeyValuePair<string, string>("custom_task_ids", CustomTaskIds.Value.ToString().ToLowerInvariant()));
        if (!string.IsNullOrWhiteSpace(TeamIdForCustomTaskIds)) parameters.Add(new KeyValuePair<string, string>("team_id", TeamIdForCustomTaskIds)); // This is the query param name if custom_task_ids is true

        return parameters;
    }
}
