using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class FluentGetTaskCommentsRequest
{
    private readonly GetTaskCommentsRequest _request;
    private readonly string _taskId;
    private readonly ICommentsService _commentsService;

    public FluentGetTaskCommentsRequest(string taskId, ICommentsService commentsService)
    {
        _taskId = taskId;
        _commentsService = commentsService;
        _request = new GetTaskCommentsRequest(taskId);
    }

    public FluentGetTaskCommentsRequest WithCustomTaskIds(bool customTaskIds)
    {
        _request.CustomTaskIds = customTaskIds;
        return this;
    }

    public FluentGetTaskCommentsRequest WithTeamId(string teamId)
    {
        _request.TeamId = teamId;
        return this;
    }

    public FluentGetTaskCommentsRequest WithStart(long start)
    {
        _request.Start = start;
        return this;
    }

    public IAsyncEnumerable<Comment> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        return _commentsService.GetTaskCommentsStreamAsync(
            _taskId,
            _request.CustomTaskIds,
            _request.TeamId,
            _request.Start,
            cancellationToken
        );
    }
}