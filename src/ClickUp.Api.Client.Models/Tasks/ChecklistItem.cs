using System;
using System.Collections.Generic;
using ClickUp.Api.Client.Models; // For User model

namespace ClickUp.Api.Client.Models.Tasks;

/// <summary>
/// Represents an item within a Checklist.
/// </summary>
public record ChecklistItem
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public User? Assignee { get; init; } // Assuming User model is defined
    public bool Resolved { get; init; }
    public string? ParentId { get; init; } // Renamed from 'parent' to avoid ambiguity
    public string? DateCreated { get; init; } // Timestamp
    public List<string> Children { get; init; } = new(); // List of child ChecklistItem IDs
}
