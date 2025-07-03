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

    // Parameters for custom task ID handling, if applicable to the service method
    public bool? CustomTaskIds { get; set; }
    public string? TeamIdForCustomTaskIds { get; set; } // Workspace ID for custom task IDs

    public Dictionary<string, string> ToDictionary()
    {
        var parameters = new Dictionary<string, string>();

        if (TimeRange != null)
        {
            // API uses start_date and end_date for this endpoint
            parameters["start_date"] = TimeRange.StartDate.ToUnixTimeMilliseconds().ToString();
            parameters["end_date"] = TimeRange.EndDate.ToUnixTimeMilliseconds().ToString();
        }

        if (AssigneeUserId.HasValue) parameters["assignee"] = AssigneeUserId.Value.ToString();
        if (!string.IsNullOrWhiteSpace(TaskId)) parameters["task_id"] = TaskId;
        if (!string.IsNullOrWhiteSpace(ListId)) parameters["list_id"] = ListId;
        if (!string.IsNullOrWhiteSpace(FolderId)) parameters["folder_id"] = FolderId;
        if (!string.IsNullOrWhiteSpace(SpaceId)) parameters["space_id"] = SpaceId;

        if (IncludeTaskTags.HasValue) parameters["include_task_tags"] = IncludeTaskTags.Value.ToString().ToLowerInvariant();
        if (IncludeLocationNames.HasValue) parameters["include_location_names"] = IncludeLocationNames.Value.ToString().ToLowerInvariant();
        if (Page.HasValue) parameters["page"] = Page.Value.ToString();

        if (CustomTaskIds.HasValue) parameters["custom_task_ids"] = CustomTaskIds.Value.ToString().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(TeamIdForCustomTaskIds)) parameters["team_id"] = TeamIdForCustomTaskIds; // This is the query param name if custom_task_ids is true

        return parameters;
    }
}
