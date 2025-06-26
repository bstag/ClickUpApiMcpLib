using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.ResponseModels.Chat;
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
}
