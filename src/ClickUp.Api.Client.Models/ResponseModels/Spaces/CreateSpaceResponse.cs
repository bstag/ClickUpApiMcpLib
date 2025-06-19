using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Spaces; // For Space entity

namespace ClickUp.Api.Client.Models.ResponseModels.Spaces
{
    public record CreateSpaceResponse
    (
        [property: JsonPropertyName("id")] string Id, // Often responses return the ID directly
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("private")] bool Private,
        [property: JsonPropertyName("color")] string? Color,
        [property: JsonPropertyName("avatar")] string? Avatar,
        [property: JsonPropertyName("admin_can_manage")] bool? AdminCanManage,
        [property: JsonPropertyName("archived")] bool? Archived,
        [property: JsonPropertyName("statuses")] List<ClickUp.Api.Client.Models.Common.Status>? Statuses, // Full Status from Common
        [property: JsonPropertyName("multiple_assignees")] bool MultipleAssignees,
        [property: JsonPropertyName("features")] Features Features,
        [property: JsonPropertyName("team_id")] string? TeamId,
        [property: JsonPropertyName("members")] List<MemberSummary>? Members // Using MemberSummary from Space.cs
        // This is essentially a full Space object.
        // Depending on API, it might be identical to GetSpaceResponse or just `public Space Space { get; init; }`
        // For now, defining it as a full Space-like object.
        // If API guarantees full Space object, can be simplified to:
        // [property: JsonPropertyName("space")] Space Space
    ) : Space(Id, Name, Private, Color, Avatar, AdminCanManage, Archived, Members, Statuses, MultipleAssignees, Features, TeamId, null); // Assuming DefaultListSettings is not part of create response.
}
