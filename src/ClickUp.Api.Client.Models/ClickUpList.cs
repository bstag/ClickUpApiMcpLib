using System.Collections.Generic;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models;

/// <summary>
/// Represents a ClickUp List.
/// </summary>
public record ClickUpList
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public string? Content { get; init; }
    public ListStatusInfo? Status { get; init; } // List's own color/status display
    public ListPriorityInfo? Priority { get; init; } // Using ListPriorityInfo for list's own priority display
    public User? Assignee { get; init; } // Assuming User model
    public string? TaskCount { get; init; }
    public string? DueDate { get; init; } // Consider DateTime, but string from spec for now
    public bool? DueDateTime { get; init; }
    public string? StartDate { get; init; } // Consider DateTime
    public bool? StartDateTime { get; init; }
    public Folder? Folder { get; init; } // Parent Folder, can be null for folderless lists
    public Space Space { get; init; } = null!; // Parent Space, assuming Space model
    public bool Archived { get; init; }
    public bool OverrideStatuses { get; init; }
    public List<Status>? Statuses { get; init; } // CuTask statuses for this list, assuming Status model
    public string? PermissionLevel { get; init; }
    public string? InboundAddress { get; init; }
}
