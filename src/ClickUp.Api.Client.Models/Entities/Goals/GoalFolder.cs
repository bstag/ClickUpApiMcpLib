using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Users; // For User and Member

namespace ClickUp.Api.Client.Models.Entities.Goals
{
    /// <summary>
    /// Represents a Folder for organizing Goals in ClickUp.
    /// </summary>
    /// <param name="Id">The unique identifier of the goal folder.</param>
    /// <param name="Name">The name of the goal folder.</param>
    /// <param name="TeamId">The identifier of the team (workspace) this folder belongs to.</param>
    /// <param name="Private">Indicates whether the folder is private.</param>
    /// <param name="DateCreated">The date the folder was created, as a string (e.g., Unix timestamp in milliseconds).</param>
    /// <param name="CreatorUser">The user who created the folder.</param>
    /// <param name="GoalCount">The number of goals contained within this folder.</param>
    /// <param name="Members">A list of members who have access to this folder.</param>
    /// <param name="Goals">A list of goals contained within this folder.</param>
    /// <param name="GroupMembers">A list of group members who have access to this folder.</param>
    public record GoalFolder
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("team_id")] string TeamId,
        [property: JsonPropertyName("private")] bool Private,
        [property: JsonPropertyName("date_created")] string? DateCreated,
        [property: JsonPropertyName("creator")] User? CreatorUser,
        [property: JsonPropertyName("goal_count")] int GoalCount,
        [property: JsonPropertyName("members")] List<Member> Members,
        [property: JsonPropertyName("goals")] List<Goal> Goals,
        [property: JsonPropertyName("group_members")] List<Member>? GroupMembers
    );
}
