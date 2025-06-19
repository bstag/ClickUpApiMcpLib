using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks;

/// <summary>
/// Represents the response model for getting task members.
/// </summary>
public record class GetTaskMembersResponse
(
    [property: JsonPropertyName("members")]
    List<User> Members
);
