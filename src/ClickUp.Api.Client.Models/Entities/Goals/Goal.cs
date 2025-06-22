using System.Collections.Generic;
using System.Text.Json.Serialization;

using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User and Member

namespace ClickUp.Api.Client.Models.Entities.Goals
{
    /// <summary>
    /// Represents a Goal in ClickUp.
    /// </summary>
    /// <param name="Id">The unique identifier of the goal.</param>
    /// <param name="PrettyId">A user-friendly, human-readable identifier for the goal.</param>
    /// <param name="Name">The name of the goal.</param>
    /// <param name="TeamId">The identifier of the team (workspace) this goal belongs to.</param>
    /// <param name="CreatorUser">The user who created the goal.</param>
    /// <param name="OwnerUser">The primary owner of the goal.</param>
    /// <param name="Color">The color associated with the goal.</param>
    /// <param name="DateCreated">The date the goal was created, as a string (e.g., Unix timestamp in milliseconds).</param>
    /// <param name="StartDate">The start date of the goal, as a string.</param>
    /// <param name="DueDate">The due date of the goal, as a string.</param>
    /// <param name="Description">The description of the goal.</param>
    /// <param name="Private">Indicates whether the goal is private.</param>
    /// <param name="Archived">Indicates whether the goal is archived.</param>
    /// <param name="MultipleOwners">Indicates whether the goal can have multiple owners.</param>
    /// <param name="EditorToken">An editor token for the goal, possibly for external editing access.</param>
    /// <param name="DateUpdated">The date the goal was last updated, as a string.</param>
    /// <param name="LastUpdate">The date of the last update activity on the goal, as a string.</param>
    /// <param name="FolderId">The identifier of the folder this goal belongs to, if any.</param>
    /// <param name="Pinned">Indicates whether the goal is pinned.</param>
    /// <param name="Owners">A list of users who own this goal.</param>
    /// <param name="KeyResultCount">The number of key results associated with this goal.</param>
    /// <param name="Members">A list of members associated with this goal.</param>
    /// <param name="GroupMembers">A list of group members associated with this goal.</param>
    /// <param name="PercentCompleted">The percentage of the goal that has been completed.</param>
    /// <param name="History">A list of historical events related to the goal. The objects may need a more specific type.</param>
    /// <param name="PrettyUrl">A user-friendly URL for accessing the goal.</param>
    public record Goal
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("pretty_id")] string? PrettyId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("team_id")] string TeamId,
        [property: JsonPropertyName("creator")] User? CreatorUser,
        [property: JsonPropertyName("owner")] User? OwnerUser,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("date_created")] DateTimeOffset? DateCreated,
        [property: JsonPropertyName("start_date")] DateTimeOffset? StartDate,
        [property: JsonPropertyName("due_date")] DateTimeOffset? DueDate,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("private")] bool Private,
        [property: JsonPropertyName("archived")] bool Archived,
        [property: JsonPropertyName("multiple_owners")] bool MultipleOwners,
        [property: JsonPropertyName("editor_token")] string? EditorToken,
        [property: JsonPropertyName("date_updated")] DateTimeOffset? DateUpdated,
        [property: JsonPropertyName("last_update")] DateTimeOffset? LastUpdate,
        [property: JsonPropertyName("folder_id")] string? FolderId,
        [property: JsonPropertyName("pinned")] bool Pinned,
        [property: JsonPropertyName("owners")] List<User>? Owners,
        [property: JsonPropertyName("key_result_count")] int KeyResultCount,
        [property: JsonPropertyName("members")] List<Member>? Members,
        [property: JsonPropertyName("group_members")] List<Member>? GroupMembers,
        [property: JsonPropertyName("percent_completed")] int PercentCompleted,
        [property: JsonPropertyName("history")] List<object>? History,
        [property: JsonPropertyName("pretty_url")] string? PrettyUrl
    );
}
