using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Space entity
using System.Collections.Generic; // For List
using ClickUp.Api.Client.Models.Common; // For Status

namespace ClickUp.Api.Client.Models.ResponseModels.Spaces
{
    // GetSpaceResponse is typically a full Space object.
    // We can make it inherit from Space or wrap it.
    // Inheriting reduces redundancy if the structure is identical.
    public record GetSpaceResponse
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
    ) : Space(Id, Name, Private, Color, Avatar, AdminCanManage, Archived, Members, Statuses, MultipleAssignees, Features, TeamId, DefaultListSettings);
}
