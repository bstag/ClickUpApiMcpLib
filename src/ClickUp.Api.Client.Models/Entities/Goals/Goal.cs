using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User and Member

namespace ClickUp.Api.Client.Models.Entities.Goals
{
    public record Goal
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("pretty_id")] string? PrettyId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("team_id")] string TeamId, // Assuming string, could be int
        [property: JsonPropertyName("creator")] User? CreatorUser, // Changed from "Creator" to avoid conflict
        [property: JsonPropertyName("owner")] User? OwnerUser, // Changed from "Owner" to avoid conflict
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("date_created")] string? DateCreated, // Assuming string, could be DateTimeOffset
        [property: JsonPropertyName("start_date")] string? StartDate, // Assuming string, could be DateTimeOffset
        [property: JsonPropertyName("due_date")] string? DueDate, // Assuming string, could be DateTimeOffset
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("private")] bool Private,
        [property: JsonPropertyName("archived")] bool Archived,
        [property: JsonPropertyName("multiple_owners")] bool MultipleOwners,
        [property: JsonPropertyName("editor_token")] string? EditorToken,
        [property: JsonPropertyName("date_updated")] string? DateUpdated, // Assuming string, could be DateTimeOffset
        [property: JsonPropertyName("last_update")] string? LastUpdate, // Assuming string, could be DateTimeOffset
        [property: JsonPropertyName("folder_id")] string? FolderId, // Nullable if not in a folder
        [property: JsonPropertyName("pinned")] bool Pinned,
        [property: JsonPropertyName("owners")] List<User>? Owners,
        [property: JsonPropertyName("key_result_count")] int KeyResultCount,
        [property: JsonPropertyName("members")] List<Member>? Members,
        [property: JsonPropertyName("group_members")] List<Member>? GroupMembers, // Assuming Member type
        [property: JsonPropertyName("percent_completed")] int PercentCompleted,
        [property: JsonPropertyName("history")] List<object>? History, // Type might be more specific, e.g. GoalHistoryEntry
        [property: JsonPropertyName("pretty_url")] string? PrettyUrl
    );
}
