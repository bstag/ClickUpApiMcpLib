using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class TaskCommentsFluentGetRequest
{
    private readonly GetTaskCommentsRequest _request;
    private readonly string _taskId;
    private readonly ICommentsService _commentsService;

    public TaskCommentsFluentGetRequest(string taskId, ICommentsService commentsService)
    {
        _taskId = taskId;
        _commentsService = commentsService;
        _request = new GetTaskCommentsRequest(taskId);
    }

    public TaskCommentsFluentGetRequest WithCustomTaskIds(bool customTaskIds)
    {
        _request.CustomTaskIds = customTaskIds;
        return this;
    }

    public TaskCommentsFluentGetRequest WithTeamId(string teamId)
    {
        _request.TeamId = teamId;
        return this;
    }

    public TaskCommentsFluentGetRequest WithStart(long start)
    {
        _request.Start = start;
        return this;
    }

    public IAsyncEnumerable<Comment> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        // _request already contains TaskId, CustomTaskIds, TeamId, Start, and StartId (if set via a new WithStartId method if added)
        // The service method GetTaskCommentsStreamAsync now takes (string taskId, GetTaskCommentsRequest requestModel, ...)
        return _commentsService.GetTaskCommentsStreamAsync(
            _taskId, // Still pass taskId directly as it's part of the path for the service method
            _request,  // Pass the fully configured request DTO
            cancellationToken
        );
    }
}