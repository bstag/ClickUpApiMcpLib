using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    /// <summary>
    /// Represents the request to create a new direct message (DM) chat channel.
    /// </summary>
    /// <param name="UserIds">A list of user IDs to include in the DM. For a 1-on-1 DM, this typically contains one user ID (the other party).</param>
    /// <param name="WorkspaceId">The ID of the workspace where the DM channel will be created.</param>
    public record ChatCreateDirectMessageChatChannelRequest
    (
        [property: JsonPropertyName("user_ids")] List<string> UserIds,
        [property: JsonPropertyName("workspace_id")] string WorkspaceId
    );
}
