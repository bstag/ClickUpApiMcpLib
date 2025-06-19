using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Chat
{
    public record ChatCreateDirectMessageChatChannelRequest
    (
        [property: JsonPropertyName("user_ids")] List<string> UserIds, // List of user IDs to include in the DM (usually one for 1-on-1 DM)
        [property: JsonPropertyName("workspace_id")] string WorkspaceId // Usually required
    );
}
