using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For Status

namespace ClickUp.Api.Client.Models.Entities.Spaces
{
    public record Space
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("private")] bool Private,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("avatar")] string? Avatar, // URL to space avatar
        [property: JsonPropertyName("admin_can_manage")] bool? AdminCanManage, // If admins can manage private space
        [property: JsonPropertyName("archived")] bool? Archived,
        [property: JsonPropertyName("members")] List<MemberSummary>? Members, // Simplified member info
        [property: JsonPropertyName("statuses")] List<Status>? Statuses,
        [property: JsonPropertyName("multiple_assignees")] bool MultipleAssignees,
        [property: JsonPropertyName("features")] Features Features, // The Features record created earlier
        [property: JsonPropertyName("team_id")] string? TeamId, // Workspace ID
        [property: JsonPropertyName("default_list_settings")] DefaultListSettings? DefaultListSettings // Settings for new lists in this space
    );

    // Simplified member info for Space's members list
    public record MemberSummary
    (
        [property: JsonPropertyName("user")] UserSummary User // Simplified User record
    );

    public record UserSummary
    (
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("username")] string Username,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("profilePicture")] string? ProfilePicture
    );

    public record DefaultListSettings
    (
        [property: JsonPropertyName("override_statuses")] bool? OverrideStatuses,
        [property: JsonPropertyName("default_status")] string? DefaultStatus, // Name/ID of default status
        [property: JsonPropertyName("default_assignee")] int? DefaultAssignee, // User ID
        [property: JsonPropertyName("default_priority")] int? DefaultPriority
    );
}
