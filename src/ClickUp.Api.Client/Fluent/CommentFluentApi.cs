using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.ResponseModels.Comments;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class CommentFluentApi
{
    private readonly ICommentsService _commentsService;

    public CommentFluentApi(ICommentsService commentsService)
    {
        _commentsService = commentsService;
    }

    public TaskCommentsFluentGetRequest GetTaskComments(string taskId)
    {
        return new TaskCommentsFluentGetRequest(taskId, _commentsService);
    }

    public async Task<CreateCommentResponse> CreateTaskCommentAsync(string taskId, CreateTaskCommentRequest createCommentRequest, bool? customTaskIds = null, string? teamId = null, CancellationToken cancellationToken = default)
    {
        return await _commentsService.CreateTaskCommentAsync(taskId, createCommentRequest, customTaskIds, teamId, cancellationToken);
    }
}
