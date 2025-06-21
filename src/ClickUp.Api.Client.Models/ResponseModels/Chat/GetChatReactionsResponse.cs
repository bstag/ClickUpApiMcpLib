using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Chat; // For ChatReaction

namespace ClickUp.Api.Client.Models.ResponseModels.Chat;

/// <summary>
/// Represents the paginated response for getting chat reactions.
/// </summary>
/// <param name="Data">A list of <see cref="ChatReaction"/> objects for the current page.</param>
/// <param name="Meta">Metadata for pagination, including cursors for next and previous pages.</param>
public record GetChatReactionsResponse
(
    [property: JsonPropertyName("data")]
    List<ChatReaction> Data,

    [property: JsonPropertyName("meta")]
    GetChatReactionsResponse.ResponseMeta Meta
)
{
    /// <summary>
    /// Represents pagination metadata for chat reaction responses.
    /// </summary>
    /// <param name="NextCursor">Cursor to retrieve the next page of reactions. Null if no more pages.</param>
    /// <param name="PreviousCursor">Cursor to retrieve the previous page of reactions. Null if no previous page.</param>
    public record ResponseMeta(
        [property: JsonPropertyName("next_cursor")]
        string? NextCursor,

        [property: JsonPropertyName("previous_cursor")]
        string? PreviousCursor
    );
}
