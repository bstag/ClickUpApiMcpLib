using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ChatSimpleUser

namespace ClickUp.Api.Client.Models.ResponseModels.Chat;

/// <summary>
/// Represents the paginated response for getting users in a chat.
/// </summary>
/// <param name="Data">A list of <see cref="ChatSimpleUser"/> objects for the current page.</param>
/// <param name="Meta">Metadata for pagination, including cursors for next and previous pages.</param>
public record GetChatUsersResponse
(
    [property: JsonPropertyName("data")]
    List<ChatSimpleUser> Data,

    [property: JsonPropertyName("meta")]
    GetChatUsersResponse.ResponseMeta Meta
)
{
    /// <summary>
    /// Represents pagination metadata for chat user responses.
    /// </summary>
    /// <param name="NextCursor">Cursor to retrieve the next page of users. Null if no more pages.</param>
    /// <param name="PreviousCursor">Cursor to retrieve the previous page of users. Null if no previous page.</param>
    public record ResponseMeta(
        [property: JsonPropertyName("next_cursor")]
        string? NextCursor,

        [property: JsonPropertyName("previous_cursor")]
        string? PreviousCursor
    );
}
