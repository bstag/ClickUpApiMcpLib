using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.Entities.Users;

/// <summary>
/// Represents a guest user in ClickUp, including their information, permissions, and shared items.
/// </summary>
public record Guest
{
    [JsonPropertyName("user")]
    public GuestUserInfo User { get; init; } = null!;

    [JsonPropertyName("invited_by")]
    public InvitedByUserInfo? InvitedBy { get; init; }

    [JsonPropertyName("can_see_time_spent")]
    public bool? CanSeeTimeSpent { get; init; }

    [JsonPropertyName("can_see_time_estimated")]
    public bool? CanSeeTimeEstimated { get; init; }

    [JsonPropertyName("can_edit_tags")]
    public bool? CanEditTags { get; init; }

    [JsonPropertyName("can_create_views")]
    public bool? CanCreateViews { get; init; }

    [JsonPropertyName("shared")]
    public GuestSharingDetails Shared { get; init; } = null!;
}
