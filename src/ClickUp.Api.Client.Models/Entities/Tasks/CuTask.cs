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
    // Simplified references for List, Folder, Space within a CuTask to avoid circular dependencies or overly complex objects.
    // These should contain minimal info like ID and Name. The full entities are defined elsewhere.
    public record TaskListReference([property: JsonPropertyName("id")] string Id, [property: JsonPropertyName("name")] string? Name, [property: JsonPropertyName("access")] bool? Access);
    public record TaskFolderReference([property: JsonPropertyName("id")] string Id, [property: JsonPropertyName("name")] string? Name, [property: JsonPropertyName("access")] bool? Access);
    public record TaskSpaceReference([property: JsonPropertyName("id")] string Id);


    public record CuTask
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("custom_id")] string? CustomId,
        [property: JsonPropertyName("custom_item_id")] long? CustomItemId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("text_content")] string? TextContent, // Plain text description
        [property: JsonPropertyName("description")] string? Description, // Markdown description
        [property: JsonPropertyName("markdown_description")] string? MarkdownDescription, // Explicit markdown description if API distinguishes
        [property: JsonPropertyName("status")] Status? Status,
        [property: JsonPropertyName("orderindex")] string? OrderIndex, // Can be a large number string
        [property: JsonPropertyName("date_created")] string? DateCreated, // Timestamp as string
        [property: JsonPropertyName("date_updated")] string? DateUpdated, // Timestamp as string
        [property: JsonPropertyName("date_closed")] string? DateClosed, // Timestamp as string
        [property: JsonPropertyName("archived")] bool? Archived,
        [property: JsonPropertyName("creator")] User? Creator,
        [property: JsonPropertyName("assignees")] List<User>? Assignees,
        [property: JsonPropertyName("group_assignees")] List<UserGroup>? GroupAssignees, // Requires UserGroup record
        [property: JsonPropertyName("watchers")] List<User>? Watchers,
        [property: JsonPropertyName("checklists")] List<Checklist>? Checklists,
        [property: JsonPropertyName("tags")] List<Tag>? Tags,
        [property: JsonPropertyName("parent")] string? Parent, // ID of parent task
        [property: JsonPropertyName("priority")] Priority? Priority, // CuTask's priority object
        [property: JsonPropertyName("due_date")] string? DueDate, // Timestamp as string
        [property: JsonPropertyName("start_date")] string? StartDate, // Timestamp as string
        [property: JsonPropertyName("points")] double? Points, // Estimate points
        [property: JsonPropertyName("time_estimate")] int? TimeEstimate, // Milliseconds
        [property: JsonPropertyName("time_spent")] int? TimeSpent, // Milliseconds
        [property: JsonPropertyName("custom_fields")] List<CustomFieldValue>? CustomFields,
        [property: JsonPropertyName("dependencies")] List<Dependency>? Dependencies, // Assuming Dependency record exists
        [property: JsonPropertyName("linked_tasks")] List<TaskLink>? LinkedTasks, // Assuming TaskLink record for linked tasks
        [property: JsonPropertyName("team_id")] string? TeamId, // Workspace ID
        [property: JsonPropertyName("url")] string? Url,
        [property: JsonPropertyName("sharing")] SharingOptions? Sharing, // Requires SharingOptions record
        [property: JsonPropertyName("permission_level")] string? PermissionLevel, // e.g. "read", "write"

        // Location context
        [property: JsonPropertyName("list")] TaskListReference? List,
        [property: JsonPropertyName("folder")] TaskFolderReference? Folder,
        [property: JsonPropertyName("space")] TaskSpaceReference? Space,
        [property: JsonPropertyName("project")] TaskFolderReference? Project // "project" is often synonymous with "folder" in ClickUp API responses
    );

    // Placeholder for UserGroup if not already defined
    public record UserGroup([property: JsonPropertyName("id")] string Id, [property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("team_id")] string TeamId);
    // Placeholder for TaskLink
    public record TaskLink([property: JsonPropertyName("task_id")] string TaskId, [property: JsonPropertyName("link_id")] string LinkId);
    // Placeholder for SharingOptions
    public record SharingOptions([property: JsonPropertyName("public")] bool? Public, [property: JsonPropertyName("public_share_expires_on")] string? PublicShareExpiresOn, [property: JsonPropertyName("public_link")] string? PublicLink);
}
