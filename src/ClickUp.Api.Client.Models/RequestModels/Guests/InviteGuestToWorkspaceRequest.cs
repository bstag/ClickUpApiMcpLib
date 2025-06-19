using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Guests;

/// <summary>
/// Represents the request model for inviting a guest to a workspace.
/// </summary>
public record class InviteGuestToWorkspaceRequest
(
    [property: JsonPropertyName("email")]
    string Email,

    [property: JsonPropertyName("can_edit_tags")]
    bool? CanEditTags,

    [property: JsonPropertyName("can_see_time_spent")]
    bool? CanSeeTimeSpent,

    [property: JsonPropertyName("can_see_time_estimated")]
    bool? CanSeeTimeEstimated,

    [property: JsonPropertyName("can_create_views")]
    bool? CanCreateViews,

    [property: JsonPropertyName("can_see_points_estimated")]
    bool? CanSeePointsEstimated,

    [property: JsonPropertyName("custom_role_id")]
    int? CustomRoleId
);
