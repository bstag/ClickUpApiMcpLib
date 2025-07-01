using System.Collections.Generic;

using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.Entities.Lists;

/// <summary>
/// Represents a ClickUp List, a container for tasks.
/// </summary>
public record ClickUpList
{
    /// <summary>
    /// Gets the unique identifier of the list.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the list.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the order index of the list, determining its position.
    /// </summary>
    public int OrderIndex { get; init; }

    /// <summary>
    /// Gets the content or description of the list.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Gets the status information for the list itself (e.g., color).
    /// </summary>
    public ListStatusInfo? Status { get; init; }

    /// <summary>
    /// Gets the priority information for the list itself.
    /// </summary>
    public ListPriorityInfo? Priority { get; init; }

    /// <summary>
    /// Gets the user assigned to this list.
    /// </summary>
    public User? Assignee { get; init; }

    /// <summary>
    /// Gets the count of tasks in the list, typically as a string.
    /// </summary>
    public string? TaskCount { get; init; }

    /// <summary>
    /// Gets the due date of the list as a string (e.g., Unix timestamp in milliseconds).
    /// </summary>
    public string? DueDate { get; init; }

    /// <summary>
    /// Gets a value indicating whether the due date includes a time component.
    /// </summary>
    public bool? DueDateTime { get; init; }

    /// <summary>
    /// Gets the start date of the list as a string (e.g., Unix timestamp in milliseconds).
    /// </summary>
    public string? StartDate { get; init; }

    /// <summary>
    /// Gets a value indicating whether the start date includes a time component.
    /// </summary>
    public bool? StartDateTime { get; init; }

    /// <summary>
    /// Gets the parent folder of the list, if any. Null for folderless lists.
    /// </summary>
    public Folder? Folder { get; init; }

    /// <summary>
    /// Gets the parent space of the list.
    /// </summary>
    public Space Space { get; init; } = null!;

    /// <summary>
    /// Gets a value indicating whether the list is archived.
    /// </summary>
    public bool Archived { get; init; }

    /// <summary>
    /// Gets a value indicating whether this list uses its own set of statuses, overriding those from the space.
    /// </summary>
    public bool OverrideStatuses { get; init; }

    /// <summary>
    /// Gets the list of task statuses defined for this list if <see cref="OverrideStatuses"/> is true.
    /// </summary>
    public List<Status>? Statuses { get; init; }

    /// <summary>
    /// Gets the permission level of the current user for this list.
    /// </summary>
    public string? PermissionLevel { get; init; }

    /// <summary>
    /// Gets the inbound email address for creating tasks in this list.
    /// </summary>
    public string? InboundAddress { get; init; }
}
