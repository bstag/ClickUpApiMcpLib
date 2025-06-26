using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks;

public class GetFilteredTeamTasksRequest
{
    public int? Page { get; set; }
    public string? OrderBy { get; set; }
    public bool? Reverse { get; set; }
    public bool? Subtasks { get; set; }
    public IEnumerable<string>? SpaceIds { get; set; }
    public IEnumerable<string>? ProjectIds { get; set; }
    public IEnumerable<string>? ListIds { get; set; }
    public IEnumerable<string>? Statuses { get; set; }
    public bool? IncludeClosed { get; set; }
    public IEnumerable<string>? Assignees { get; set; }
    public IEnumerable<string>? Tags { get; set; }
    public long? DueDateGreaterThan { get; set; }
    public long? DueDateLessThan { get; set; }
    public long? DateCreatedGreaterThan { get; set; }
    public long? DateCreatedLessThan { get; set; }
    public long? DateUpdatedGreaterThan { get; set; }
    public long? DateUpdatedLessThan { get; set; }
    public string? CustomFields { get; set; }
    public bool? CustomTaskIds { get; set; }
    public string? TeamIdForCustomTaskIds { get; set; }
    public IEnumerable<long>? CustomItems { get; set; }
    public long? DateDoneGreaterThan { get; set; }
    public long? DateDoneLessThan { get; set; }
    public string? ParentTaskId { get; set; }
    public bool? IncludeMarkdownDescription { get; set; }
}