using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Chat;
using ClickUp.Api.Client.Models.ResponseModels.Chat;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentGetChatChannelsRequest
{
    private readonly GetChatChannelsRequest _request = new();
    private readonly string _workspaceId;
    private readonly IChatService _chatService;

    public FluentGetChatChannelsRequest(string workspaceId, IChatService chatService)
    {
        _workspaceId = workspaceId;
        _chatService = chatService;
    }

    public FluentGetChatChannelsRequest WithDescriptionFormat(string descriptionFormat)
    {
        _request.DescriptionFormat = descriptionFormat;
        return this;
    }

    public FluentGetChatChannelsRequest WithCursor(string cursor)
    {
        _request.Cursor = cursor;
        return this;
    }

    public FluentGetChatChannelsRequest WithLimit(int limit)
    {
        _request.Limit = limit;
        return this;
    }

    public FluentGetChatChannelsRequest WithIsFollower(bool isFollower)
    {
        _request.IsFollower = isFollower;
        return this;
    }

    public FluentGetChatChannelsRequest WithIncludeHidden(bool includeHidden)
    {
        _request.IncludeHidden = includeHidden;
        return this;
    }

    public FluentGetChatChannelsRequest WithWithCommentSince(long withCommentSince)
    {
        _request.WithCommentSince = withCommentSince;
        return this;
    }

    public FluentGetChatChannelsRequest WithRoomTypes(IEnumerable<string> roomTypes)
    {
        _request.RoomTypes = roomTypes;
        return this;
    }

    public async Task<ChatChannelPaginatedResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _chatService.GetChatChannelsAsync(
            _workspaceId,
            _request.DescriptionFormat,
            _request.Cursor,
            _request.Limit,
            _request.IsFollower,
            _request.IncludeHidden,
            _request.WithCommentSince,
            _request.RoomTypes,
            cancellationToken
        );
    }
}