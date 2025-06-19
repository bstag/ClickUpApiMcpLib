using System;
using System.Collections.Generic;
using ClickUp.Api.Client.Models; // For User model

namespace ClickUp.Api.Client.Models.Tasks;

/// <summary>
/// Represents a text object within a Comment, potentially for rich text.
/// </summary>
public record CommentTextEntry // Changed from CommentText to avoid conflict if Comment.Text property is desired
{
    public string Text { get; init; } = string.Empty;
}

/// <summary>
/// Represents a comment in ClickUp.
/// </summary>
public record Comment
{
    public string Id { get; init; } = string.Empty;
    public List<CommentTextEntry> CommentTextEntries { get; init; } = new(); // Maps to 'comment' in JSON
    public string CommentText { get; init; } = string.Empty; // Plain text version
    public User User { get; init; } = null!; // User who made the comment
    public bool Resolved { get; init; }
    public User? Assignee { get; init; } // User to whom the comment is assigned
    public User? AssignedBy { get; init; } // User who assigned the comment
    public List<object>? Reactions { get; init; } = new(); // Placeholder for reactions, could be List<string> or List<ReactionObject>
    public string Date { get; init; } = string.Empty; // Timestamp
    public string? ReplyCount { get; init; } // From GetTaskCommentsresponse example
}
