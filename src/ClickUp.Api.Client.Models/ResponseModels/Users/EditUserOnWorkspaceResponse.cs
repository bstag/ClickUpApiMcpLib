using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.ResponseModels.Users;

/// <summary>
/// Represents the response model for editing a user on a workspace.
/// </summary>
public record class EditUserOnWorkspaceResponse
(
    [property: JsonPropertyName("member")]
    GetUserResponseMember Member
);
