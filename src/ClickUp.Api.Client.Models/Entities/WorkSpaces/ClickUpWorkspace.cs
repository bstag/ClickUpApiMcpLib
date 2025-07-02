using System.Collections.Generic;

using ClickUp.Api.Client.Models.Common; // For Member
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.Entities.WorkSpaces;

/// <summary>
/// Represents a ClickUp Workspace, also known as a Team in the API.
/// </summary>
public record ClickUpWorkspace
{
    /// <summary>
    /// Gets the unique identifier of the workspace.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the workspace.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the color associated with the workspace.
    /// </summary>
    /// <example>"#FF0000"</example>
    public string? Color { get; init; }

    /// <summary>
    /// Gets the URL of the avatar for the workspace.
    /// </summary>
    public string? Avatar { get; init; }

    /// <summary>
    /// Gets the list of members in this workspace.
    /// </summary>
    public List<Member>? Members { get; init; }
}
