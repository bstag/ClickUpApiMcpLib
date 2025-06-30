using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Chat;
using System.Collections.Generic; // Added for IAsyncEnumerable
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class ChatFluentApi
{
    private readonly IChatService _chatService;

    public ChatFluentApi(IChatService chatService)
    {
        _chatService = chatService;
    }

    public ChatChannelsFluentGetRequest GetChatChannels(string workspaceId)
    {
        return new ChatChannelsFluentGetRequest(workspaceId, _chatService);
    }

    /// <summary>
    /// Retrieves all messages for a specific channel asynchronously, handling pagination.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace.</param>
    /// <param name="channelId">The ID of the channel.</param>
    /// <param name="contentFormat">Optional content format for the messages (e.g., "text/plain", "text/md").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Models.Entities.Chat.ChatMessage"/>.</returns>
    public async IAsyncEnumerable<Models.Entities.Chat.ChatMessage> GetChannelMessagesAsyncEnumerableAsync(
        string workspaceId,
        string channelId,
        string? contentFormat = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = null;
        const int limit = 100; // Max limit as per API docs for efficient fetching

        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            var response = await _chatService.GetChatMessagesAsync(
                workspaceId,
                channelId,
                cursor,
                limit,
                contentFormat,
                cancellationToken);

            if (response?.Data != null)
            {
                foreach (var message in response.Data)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return message;
                }
            }

            cursor = response?.Meta?.NextCursor;
        } while (!string.IsNullOrEmpty(cursor));
    }
}
