using System.Collections.Generic;

namespace ClickUp.Api.Client.Models;

/// <summary>
/// Represents a ClickUp Space.
/// </summary>
public record Space
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool Private { get; init; }
    public string? Color { get; init; }
    public string? Avatar { get; init; }
    public bool? AdminCanManage { get; init; }
    public bool? Archived { get; init; }
    public List<User>? Members { get; init; } // Assuming User model is defined
    public List<Status>? Statuses { get; init; } // Assuming Status model will be defined
    public bool MultipleAssignees { get; init; }
    public object? Features { get; init; } // Placeholder for complex 'features' object, to be detailed later
}
