using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.ResponseModels.Lists;

/// <summary>
/// Represents the response model for getting list members.
/// </summary>
public record class GetListMembersResponse
(
    [property: JsonPropertyName("members")]
    List<User> Members
);
