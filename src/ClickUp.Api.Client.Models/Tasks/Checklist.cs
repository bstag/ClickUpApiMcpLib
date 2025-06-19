using System;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Models.Tasks;

/// <summary>
/// Represents a checklist within a Task.
/// </summary>
public record Checklist
{
    public string Id { get; init; } = string.Empty;
    public string TaskId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? DateCreated { get; init; } // Timestamp
    public int OrderIndex { get; init; }
    public int Creator { get; init; } // User ID
    public int Resolved { get; init; } // Count of resolved items
    public int Unresolved { get; init; } // Count of unresolved items
    public List<ChecklistItem> Items { get; init; } = new();
}
