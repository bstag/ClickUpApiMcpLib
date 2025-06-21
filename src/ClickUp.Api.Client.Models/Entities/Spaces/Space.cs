using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For Status

namespace ClickUp.Api.Client.Models.Entities.Spaces
{
    /// <summary>
    /// Represents a Space in ClickUp, which is a top-level organizational unit containing Folders and Lists.
    /// </summary>
    /// <param name="Id">The unique identifier of the space.</param>
    /// <param name="Name">The name of the space.</param>
    /// <param name="Private">Indicates whether the space is private.</param>
    /// <param name="Color">The color associated with the space.</param>
    /// <param name="Avatar">The URL of the avatar for the space.</param>
    /// <param name="AdminCanManage">Indicates if admins can manage this private space.</param>
    /// <param name="Archived">Indicates whether the space is archived.</param>
    /// <param name="Members">A list of members in this space (summary view).</param>
    /// <param name="Statuses">A list of statuses available within this space.</param>
    /// <param name="MultipleAssignees">Indicates if tasks within this space can have multiple assignees.</param>
    /// <param name="Features">The configuration of features enabled for this space.</param>
    /// <param name="TeamId">The identifier of the team (workspace) this space belongs to.</param>
    /// <param name="DefaultListSettings">Default settings for new lists created within this space.</param>
    public record Space
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("private")] bool Private,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("avatar")] string? Avatar,
        [property: JsonPropertyName("admin_can_manage")] bool? AdminCanManage,
        [property: JsonPropertyName("archived")] bool? Archived,
        [property: JsonPropertyName("members")] List<MemberSummary>? Members,
        [property: JsonPropertyName("statuses")] List<Status>? Statuses,
        [property: JsonPropertyName("multiple_assignees")] bool MultipleAssignees,
        [property: JsonPropertyName("features")] Features Features,
        [property: JsonPropertyName("team_id")] string? TeamId,
        [property: JsonPropertyName("default_list_settings")] DefaultListSettings? DefaultListSettings
    );

    /// <summary>
    /// Represents a summary of a member within a Space context.
    /// </summary>
    /// <param name="User">Summary information about the user.</param>
    public record MemberSummary
    (
        [property: JsonPropertyName("user")] UserSummary User
    );

    /// <summary>
    /// Represents a summary of a user, often used within member lists.
    /// </summary>
    /// <param name="Id">The unique identifier of the user.</param>
    /// <param name="Username">The username of the user.</param>
    /// <param name="Color">The color associated with the user.</param>
    /// <param name="ProfilePicture">The URL of the user's profile picture.</param>
    public record UserSummary
    (
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("username")] string Username,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("profilePicture")] string? ProfilePicture
    );

    /// <summary>
    /// Represents default settings for Lists created within a Space.
    /// </summary>
    /// <param name="OverrideStatuses">Indicates if new lists will override space statuses by default.</param>
    /// <param name="DefaultStatus">The name or ID of the default status for tasks in new lists.</param>
    /// <param name="DefaultAssignee">The user ID of the default assignee for tasks in new lists.</param>
    /// <param name="DefaultPriority">The default priority level for tasks in new lists.</param>
    public record DefaultListSettings
    (
        [property: JsonPropertyName("override_statuses")] bool? OverrideStatuses,
        [property: JsonPropertyName("default_status")] string? DefaultStatus,
        [property: JsonPropertyName("default_assignee")] int? DefaultAssignee,
        [property: JsonPropertyName("default_priority")] int? DefaultPriority
    );
}
