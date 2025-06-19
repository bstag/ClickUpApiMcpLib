using System.Collections.Generic;

namespace ClickUp.Api.Client.Models;

/// <summary>
/// Represents a ClickUp Folder.
/// </summary>
public record Folder
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public bool OverrideStatuses { get; init; }
    public bool Hidden { get; init; }
    public Space Space { get; init; } = null!; // Parent Space, assuming Space model is defined
    public string? TaskCount { get; init; }
    public bool? Archived { get; init; }
    public List<Status>? Statuses { get; init; } // Task statuses for this folder, if OverrideStatuses is true
    public List<ClickUpList>? Lists { get; init; } // Assuming ClickUpList model will be defined
}
