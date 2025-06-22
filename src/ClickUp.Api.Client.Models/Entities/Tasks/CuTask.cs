using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Checklists; // For User, Status, Priority, Tag, Checklist
using ClickUp.Api.Client.Models.Entities.CustomFields; // For CustomFieldValue (if it's in this namespace)
                                                       // If CustomFieldValue is in Entities, then using ClickUp.Api.Client.Models.Entities; is fine.
using ClickUp.Api.Client.Models.Entities.Lists;   // For List (simplified)
using ClickUp.Api.Client.Models.Entities.Folders; // For Folder (simplified)
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.Entities.Users; // For Space (simplified)


// Assuming CustomFieldValue is in ClickUp.Api.Client.Models.Entities namespace based on previous steps
// using ClickUp.Api.Client.Models.Entities;

namespace ClickUp.Api.Client.Models.Entities.Tasks
{
    /// <summary>
    /// Represents a simplified reference to a List, typically used within a CuTask context.
    /// </summary>
    /// <param name="Id">The ID of the list.</param>
    /// <param name="Name">The name of the list.</param>
    /// <param name="Access">Indicates if the current user has access to this list.</param>
    public record TaskListReference([property: JsonPropertyName("id")] string Id, [property: JsonPropertyName("name")] string? Name, [property: JsonPropertyName("access")] bool? Access);

    /// <summary>
    /// Represents a simplified reference to a Folder, typically used within a CuTask context.
    /// </summary>
    /// <param name="Id">The ID of the folder.</param>
    /// <param name="Name">The name of the folder.</param>
    /// <param name="Access">Indicates if the current user has access to this folder.</param>
    public record TaskFolderReference([property: JsonPropertyName("id")] string Id, [property: JsonPropertyName("name")] string? Name, [property: JsonPropertyName("access")] bool? Access);

    /// <summary>
    /// Represents a simplified reference to a Space, typically used within a CuTask context.
    /// </summary>
    /// <param name="Id">The ID of the space.</param>
    public record TaskSpaceReference([property: JsonPropertyName("id")] string Id);

    /// <summary>
    /// Represents a CuTask in ClickUp.
    /// </summary>
    /// <param name="Id">The unique identifier of the task.</param>
    /// <param name="CustomId">The custom identifier of the task, if any.</param>
    /// <param name="CustomItemId">A custom item ID, possibly related to integrations or specific configurations.</param>
    /// <param name="Name">The name or title of the task.</param>
    /// <param name="TextContent">The plain text content of the task's description.</param>
    /// <param name="Description">The Markdown formatted description of the task.</param>
    /// <param name="MarkdownDescription">Explicitly the Markdown description, if the API provides it separately from <see cref="Description"/>.</param>
    /// <param name="Status">The current status of the task.</param>
    /// <param name="OrderIndex">The order index of the task within its list, often a string representing a potentially large number.</param>
    /// <param name="DateCreated">The date the task was created, as a string (e.g., Unix timestamp in milliseconds).</param>
    /// <param name="DateUpdated">The date the task was last updated, as a string.</param>
    /// <param name="DateClosed">The date the task was closed, as a string, if applicable.</param>
    /// <param name="Archived">Indicates whether the task is archived.</param>
    /// <param name="Creator">The user who created the task.</param>
    /// <param name="Assignees">A list of users assigned to the task.</param>
    /// <param name="GroupAssignees">A list of user groups assigned to the task.</param>
    /// <param name="Watchers">A list of users watching the task.</param>
    /// <param name="Checklists">A list of checklists associated with the task.</param>
    /// <param name="Tags">A list of tags applied to the task.</param>
    /// <param name="Parent">The identifier of the parent task, if this is a subtask.</param>
    /// <param name="Priority">The priority of the task.</param>
    /// <param name="DueDate">The due date of the task, as a string.</param>
    /// <param name="StartDate">The start date of the task, as a string.</param>
    /// <param name="Points">The estimated points for the task.</param>
    /// <param name="TimeEstimate">The estimated time for the task, in milliseconds.</param>
    /// <param name="TimeSpent">The time spent on the task, in milliseconds.</param>
    /// <param name="CustomFields">A list of custom field values for the task.</param>
    /// <param name="Dependencies">A list of dependencies for this task.</param>
    /// <param name="LinkedTasks">A list of tasks linked to this task.</param>
    /// <param name="TeamId">The identifier of the team (workspace) this task belongs to.</param>
    /// <param name="Url">The URL to access this task in the ClickUp application.</param>
    /// <param name="Sharing">Sharing options for the task.</param>
    /// <param name="PermissionLevel">The permission level of the current user for this task (e.g., "read", "write").</param>
    /// <param name="List">A reference to the list this task belongs to.</param>
    /// <param name="Folder">A reference to the folder this task belongs to.</param>
    /// <param name="Space">A reference to the space this task belongs to.</param>
    /// <param name="Project">A reference to the project (often synonymous with folder) this task belongs to.</param>
    public record CuTask
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("custom_id")] string? CustomId,
        [property: JsonPropertyName("custom_item_id")] long? CustomItemId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("text_content")] string? TextContent,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("markdown_description")] string? MarkdownDescription,
        [property: JsonPropertyName("status")] Status? Status,
        [property: JsonPropertyName("orderindex")] string? OrderIndex,
        [property: JsonPropertyName("date_created")] DateTimeOffset? DateCreated,
        [property: JsonPropertyName("date_updated")] DateTimeOffset? DateUpdated,
        [property: JsonPropertyName("date_closed")] DateTimeOffset? DateClosed,
        [property: JsonPropertyName("archived")] bool? Archived,
        [property: JsonPropertyName("creator")] User? Creator,
        [property: JsonPropertyName("assignees")] List<User>? Assignees,
        [property: JsonPropertyName("group_assignees")] List<UserGroup>? GroupAssignees,
        [property: JsonPropertyName("watchers")] List<User>? Watchers,
        [property: JsonPropertyName("checklists")] List<Checklist>? Checklists,
        [property: JsonPropertyName("tags")] List<Tag>? Tags,
        [property: JsonPropertyName("parent")] string? Parent,
        [property: JsonPropertyName("priority")] Priority? Priority,
        [property: JsonPropertyName("due_date")] DateTimeOffset? DueDate,
        [property: JsonPropertyName("start_date")] DateTimeOffset? StartDate,
        [property: JsonPropertyName("points")] double? Points,
        [property: JsonPropertyName("time_estimate")] int? TimeEstimate,
        [property: JsonPropertyName("time_spent")] int? TimeSpent,
        [property: JsonPropertyName("custom_fields")] List<CustomFieldValue>? CustomFields,
        [property: JsonPropertyName("dependencies")] List<Dependency>? Dependencies,
        [property: JsonPropertyName("linked_tasks")] List<TaskLink>? LinkedTasks,
        [property: JsonPropertyName("team_id")] string? TeamId,
        [property: JsonPropertyName("url")] string? Url,
        [property: JsonPropertyName("sharing")] SharingOptions? Sharing,
        [property: JsonPropertyName("permission_level")] string? PermissionLevel,

        [property: JsonPropertyName("list")] TaskListReference? List,
        [property: JsonPropertyName("folder")] TaskFolderReference? Folder,
        [property: JsonPropertyName("space")] TaskSpaceReference? Space,
        [property: JsonPropertyName("project")] TaskFolderReference? Project
    );

    /// <summary>
    /// Represents a User Group in ClickUp.
    /// </summary>
    /// <param name="Id">The unique identifier of the user group.</param>
    /// <param name="Name">The name of the user group.</param>
    /// <param name="TeamId">The identifier of the team (workspace) this group belongs to.</param>
    public record UserGroup([property: JsonPropertyName("id")] string Id, [property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("team_id")] string TeamId);

    /// <summary>
    /// Represents a link between two tasks.
    /// </summary>
    /// <param name="TaskId">The identifier of the current task.</param>
    /// <param name="LinkId">The identifier of the linked task.</param>
    public record TaskLink([property: JsonPropertyName("task_id")] string TaskId, [property: JsonPropertyName("link_id")] string LinkId);

    /// <summary>
    /// Represents sharing options for an entity like a CuTask.
    /// </summary>
    /// <param name="Public">Indicates if the entity is publicly shared.</param>
    /// <param name="PublicShareExpiresOn">The expiration date for the public share link.</param>
    /// <param name="PublicLink">The public share link URL.</param>
    public record SharingOptions([property: JsonPropertyName("public")] bool? Public, [property: JsonPropertyName("public_share_expires_on")] DateTimeOffset? PublicShareExpiresOn, [property: JsonPropertyName("public_link")] string? PublicLink);
}
