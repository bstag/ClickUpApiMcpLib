using System.Collections.Generic;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models;

/// <summary>
/// Represents a ClickUp Workspace (often referred to as Team in API v2).
/// </summary>
public record ClickUpWorkspace
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Color { get; init; }
    public string? Avatar { get; init; }
    public List<User>? Members { get; init; } // Assuming User model is already defined
}
