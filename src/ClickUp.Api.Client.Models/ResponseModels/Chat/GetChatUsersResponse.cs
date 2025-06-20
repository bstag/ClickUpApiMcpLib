using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ChatSimpleUser

namespace ClickUp.Api.Client.Models.ResponseModels.Chat;

public record GetChatUsersResponse
(
    [property: JsonPropertyName("data")]
    List<ChatSimpleUser> Data,

    [property: JsonPropertyName("meta")]
    GetChatUsersResponse.ResponseMeta Meta
)
{
    public record ResponseMeta(
        [property: JsonPropertyName("next_cursor")]
        string? NextCursor,

        [property: JsonPropertyName("previous_cursor")]
        string? PreviousCursor
    );
}
