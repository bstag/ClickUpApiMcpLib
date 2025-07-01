using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Chat;
using ClickUp.Api.Client.Models.ResponseModels.Chat;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class ChatChannelFluentQueryRequest
{
    private readonly GetChatChannelsRequest _request = new();
    private readonly string _workspaceId;
    private readonly IChatService _chatService;

    public ChatChannelFluentQueryRequest(string workspaceId, IChatService chatService)
    {
        _workspaceId = workspaceId;
        _chatService = chatService;
    }

    public ChatChannelFluentQueryRequest WithDescriptionFormat(string descriptionFormat)
    {
        _request.DescriptionFormat = descriptionFormat;
        return this;
    }

    public ChatChannelFluentQueryRequest WithCursor(string cursor)
    {
        _request.Cursor = cursor;
        return this;
    }

    public ChatChannelFluentQueryRequest WithLimit(int limit)
    {
        _request.Limit = limit;
        return this;
    }

    public ChatChannelFluentQueryRequest WithIsFollower(bool isFollower)
    {
        _request.IsFollower = isFollower;
        return this;
    }

    public ChatChannelFluentQueryRequest WithIncludeHidden(bool includeHidden)
    {
        _request.IncludeHidden = includeHidden;
        return this;
    }

    public ChatChannelFluentQueryRequest WithWithCommentSince(long withCommentSince)
    {
        _request.WithCommentSince = withCommentSince;
        return this;
    }

    public ChatChannelFluentQueryRequest WithRoomTypes(IEnumerable<string> roomTypes)
    {
        _request.RoomTypes = roomTypes;
        return this;
    }

    public async Task<ChatChannelPaginatedResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _chatService.GetChatChannelsAsync(
            _workspaceId,
            _request, // Pass the DTO directly
            cancellationToken
        );
    }
}