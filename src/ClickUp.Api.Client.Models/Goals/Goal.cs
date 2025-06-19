using System;
using System.Collections.Generic;
using ClickUp.Api.Client.Models; // For User model

namespace ClickUp.Api.Client.Models.Goals;

// Forward declaration for KeyResult, will be defined in its own file.
// public record KeyResult;

/// <summary>
/// Represents a Goal in ClickUp.
/// </summary>
public record Goal
{
    public string Id { get; init; } = string.Empty;
    public string? PrettyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string TeamId { get; init; } = string.Empty; // Workspace ID
    public int Creator { get; init; } // User ID
    public string? Owner { get; init; } // User ID of primary owner (older field, 'owners' is preferred)
    public string? Color { get; init; }
    public string DateCreated { get; init; } = string.Empty; // Timestamp
    public string? StartDate { get; init; } // Timestamp
    public string DueDate { get; init; } = string.Empty; // Timestamp
    public string Description { get; init; } = string.Empty;
    public bool Private { get; init; }
    public bool Archived { get; init; }
    public bool MultipleOwners { get; init; }
    public string? EditorToken { get; init; }
    public string? DateUpdated { get; init; } // Timestamp
    public string? LastUpdate { get; init; } // Timestamp
    public string? FolderId { get; init; } // Goal Folder ID
    public bool? Pinned { get; init; }
    public List<User> Owners { get; init; } = new(); // List of User objects
    public int? KeyResultCount { get; init; }
    public List<User> Members { get; init; } = new(); // Users with access
    public List<string> GroupMembers { get; init; } = new(); // List of group IDs with access
    public int? PercentCompleted { get; init; }
    public List<KeyResult>? KeyResults { get; init; } // List of KeyResult objects
    public List<object>? History { get; init; } = new(); // Placeholder for history events
    public string? PrettyUrl { get; init; }
}
