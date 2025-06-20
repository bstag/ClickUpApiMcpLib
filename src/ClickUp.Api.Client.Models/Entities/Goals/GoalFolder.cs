using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common; // For User and Member

namespace ClickUp.Api.Client.Models.Entities.Goals
{
    public record GoalFolder
    (
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("team_id")] string TeamId, // Assuming string, could be int
        [property: JsonPropertyName("private")] bool Private,
        [property: JsonPropertyName("date_created")] string? DateCreated, // Assuming string, could be DateTimeOffset
        [property: JsonPropertyName("creator")] ComUser? CreatorUser, // Changed from Creator
        [property: JsonPropertyName("goal_count")] int GoalCount,
        [property: JsonPropertyName("members")] List<Member> Members,
        [property: JsonPropertyName("goals")] List<Goal> Goals, // List of Goal entities within this folder
        [property: JsonPropertyName("group_members")] List<Member>? GroupMembers // From Folder2 schema
    );
}
