using System;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models;

/// <summary>
/// Represents a ClickUp Task.
/// </summary>
public record Task
{
    public string Id { get; init; } = string.Empty;
    public string? CustomId { get; init; }
    public double? CustomItemId { get; init; } // Based on 'custom_item_id' being number or null
    public string Name { get; init; } = string.Empty;
    public string? TextContent { get; init; }
    public string? Description { get; init; }
    public Status? Status { get; init; } // Assuming Status model is defined
    public string? OrderIndex { get; init; }
    public string? DateCreated { get; init; } // Consider DateTimeOffset
    public string? DateUpdated { get; init; } // Consider DateTimeOffset
    public string? DateClosed { get; init; } // Consider DateTimeOffset
    public string? DateDone { get; init; } // Consider DateTimeOffset
    public User? Creator { get; init; } // Assuming User model is defined
    public List<User>? Assignees { get; init; }
    public List<User>? Watchers { get; init; }
    public List<string>? Checklists { get; init; } // Placeholder: List of checklist IDs or names
    public List<string>? Tags { get; init; } // Placeholder: List of tag names
    public string? ParentId { get; init; } // Renamed from 'parent' to avoid ambiguity
    public Priority? Priority { get; init; } // Assuming Priority model is defined
    public string? DueDate { get; init; } // Consider DateTimeOffset
    public string? StartDate { get; init; } // Consider DateTimeOffset
    public double? Points { get; init; }
    public long? TimeEstimate { get; init; } // Assuming milliseconds
    public long? TimeSpent { get; init; } // Assuming milliseconds
    public ClickUpList? List { get; init; } // Assuming ClickUpList model is defined
    public Folder? Folder { get; init; } // Assuming Folder model is defined
    public Space? Space { get; init; } // Assuming Space model is defined
    public string? Url { get; init; }
    public object? CustomFields { get; init; } // Placeholder for custom fields
    public List<string>? Dependencies { get; init; } // List of task IDs
    public List<LinkedTask>? LinkedTasks { get; init; } // Assuming LinkedTask model will be defined
    public string? TeamId { get; init; } // Workspace ID
    public string? PermissionLevel { get; init; }
    public bool? Archived { get; init; }
}
