using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Users;

/// <summary>
/// Represents a guest user in ClickUp, including their information, permissions, and shared items.
/// </summary>
public record Guest
{
    /// <summary>
    /// Gets the detailed information about the guest user.
    /// </summary>
    [JsonPropertyName("user")]
    public GuestUserInfo User { get; init; } = null!;

    /// <summary>
    /// Gets information about the user who invited this guest.
    /// </summary>
    [JsonPropertyName("invited_by")]
    public InvitedByUserInfo? InvitedBy { get; init; }

    /// <summary>
    /// Gets a value indicating whether the guest can see time spent on tasks.
    /// </summary>
    [JsonPropertyName("can_see_time_spent")]
    public bool? CanSeeTimeSpent { get; init; }

    /// <summary>
    /// Gets a value indicating whether the guest can see time estimated for tasks.
    /// </summary>
    [JsonPropertyName("can_see_time_estimated")]
    public bool? CanSeeTimeEstimated { get; init; }

    /// <summary>
    /// Gets a value indicating whether the guest can edit tags.
    /// </summary>
    [JsonPropertyName("can_edit_tags")]
    public bool? CanEditTags { get; init; }

    /// <summary>
    /// Gets a value indicating whether the guest can create views.
    /// </summary>
    [JsonPropertyName("can_create_views")]
    public bool? CanCreateViews { get; init; }

    /// <summary>
    /// Gets the details of entities (tasks, lists, folders) shared with this guest.
    /// </summary>
    [JsonPropertyName("shared")]
    public GuestSharingDetails Shared { get; init; } = null!;
}
