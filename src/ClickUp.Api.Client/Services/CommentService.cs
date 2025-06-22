using System;
using System.Collections.Generic;
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Comments;
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.ResponseModels.Comments; // For CreateCommentResponse and potential GetCommentsResponse
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ICommentsService"/> for ClickUp Comment operations.
    /// </summary>
    public class CommentService : ICommentsService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<CommentService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public CommentService(IApiConnection apiConnection, ILogger<CommentService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<CommentService>.Instance;
        }

        private string BuildQueryString(Dictionary<string, string?> queryParams)
        {
            if (queryParams == null || !queryParams.Any(kvp => kvp.Value != null))
            {
                return string.Empty;
            }

            var sb = new StringBuilder("?");
            foreach (var kvp in queryParams)
            {
                if (kvp.Value != null)
                {
                    if (sb.Length > 1) sb.Append('&');
                    sb.Append($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
            }
            return sb.ToString();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Comment>> GetTaskCommentsAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            long? start = null,
            string? startId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting task comments for task ID: {TaskId}, Start: {Start}, StartId: {StartId}", taskId, start, startId);
            var endpoint = $"task/{taskId}/comment";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            if (start.HasValue) queryParams["start"] = start.Value.ToString();
            if (!string.IsNullOrEmpty(startId)) queryParams["start_id"] = startId;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetTaskCommentsResponse>(endpoint, cancellationToken);
            return response?.Comments ?? Enumerable.Empty<Comment>();
        }

        /// <inheritdoc />
        public async Task<CreateCommentResponse> CreateTaskCommentAsync(
            string taskId,
            CreateCommentRequest createCommentRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating task comment for task ID: {TaskId}", taskId);
            var endpoint = $"task/{taskId}/comment";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.PostAsync<CreateCommentRequest, CreateCommentResponse>(endpoint, createCommentRequest, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response for creating task comment on task {taskId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Comment>> GetChatViewCommentsAsync(
            string viewId,
            long? start = null,
            string? startId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting chat view comments for view ID: {ViewId}, Start: {Start}, StartId: {StartId}", viewId, start, startId);
            var endpoint = $"view/{viewId}/comment";
            var queryParams = new Dictionary<string, string?>();
            if (start.HasValue) queryParams["start"] = start.Value.ToString();
            if (!string.IsNullOrEmpty(startId)) queryParams["start_id"] = startId;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetListCommentsResponse>(endpoint, cancellationToken);
            return response?.Comments ?? Enumerable.Empty<Comment>();
        }

        /// <inheritdoc />
        public async Task<CreateCommentResponse> CreateChatViewCommentAsync(
            string viewId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating chat view comment for view ID: {ViewId}", viewId);
            var endpoint = $"view/{viewId}/comment";
            var response = await _apiConnection.PostAsync<CreateCommentRequest, CreateCommentResponse>(endpoint, createCommentRequest, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response for creating chat view comment on view {viewId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Comment>> GetListCommentsAsync(
            string listId,
            long? start = null,
            string? startId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting list comments for list ID: {ListId}, Start: {Start}, StartId: {StartId}", listId, start, startId);
            var endpoint = $"list/{listId}/comment";
            var queryParams = new Dictionary<string, string?>();
            if (start.HasValue) queryParams["start"] = start.Value.ToString();
            if (!string.IsNullOrEmpty(startId)) queryParams["start_id"] = startId;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetListCommentsResponse>(endpoint, cancellationToken);
            return response?.Comments ?? Enumerable.Empty<Comment>();
        }

        /// <inheritdoc />
        public async Task<CreateCommentResponse> CreateListCommentAsync(
            string listId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating list comment for list ID: {ListId}", listId);
            var endpoint = $"list/{listId}/comment";
            var response = await _apiConnection.PostAsync<CreateCommentRequest, CreateCommentResponse>(endpoint, createCommentRequest, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response for creating list comment on list {listId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<Comment> UpdateCommentAsync(
            string commentId,
            UpdateCommentRequest updateCommentRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating comment ID: {CommentId}", commentId);
            var endpoint = $"comment/{commentId}";
            var comment = await _apiConnection.PutAsync<UpdateCommentRequest, Comment>(endpoint, updateCommentRequest, cancellationToken);
            if (comment == null)
            {
                throw new InvalidOperationException($"API connection returned null response for updating comment {commentId}.");
            }
            return comment;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteCommentAsync(
            string commentId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting comment ID: {CommentId}", commentId);
            var endpoint = $"comment/{commentId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Comment>> GetThreadedCommentsAsync( // API is GET /comment/{comment_id} (Get Comment) which includes replies.
                                                                           // Or if specific endpoint for replies exists, use that.
                                                                           // The provided API ref was GET /comment/{comment_id}/reply, this is not standard.
                                                                           // Let's assume GET /comment/{comment_id} and we extract replies from the Comment DTO.
                                                                           // Or, if the endpoint is specifically for replies, the response DTO might be different.
                                                                           // For now, assuming it returns a list of comments that are replies.
            string commentId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting threaded comments for comment ID: {CommentId}", commentId);
             // The API doc suggests "GET /v2/comment/{comment_id}/reply" but this is not standard.
             // More likely, one gets a comment and its 'replies' field, or this endpoint is a special case.
             // Let's assume it's a direct endpoint that returns a list of comments.
            var endpoint = $"comment/{commentId}/reply"; // This endpoint is hypothetical based on original comments.
                                                         // Standard GET /comment/{comment_id} would return the comment itself, which might contain replies.
                                                         // If this specific endpoint exists and returns a list of comments:
            var response = await _apiConnection.GetAsync<GetListCommentsResponse>(endpoint, cancellationToken); // Assuming {"comments": [...]}
            return response?.Comments ?? Enumerable.Empty<Comment>();
        }

        /// <inheritdoc />
        public async Task<CreateCommentResponse> CreateThreadedCommentAsync( // POST /v2/comment/{comment_id}/reply
            string commentId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating threaded comment for comment ID: {CommentId}", commentId);
            var endpoint = $"comment/{commentId}/reply";
            var response = await _apiConnection.PostAsync<CreateCommentRequest, CreateCommentResponse>(endpoint, createCommentRequest, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response for creating threaded comment on comment {commentId}.");
            }
            return response;
        }

        Task ICommentsService.UpdateCommentAsync(string commentId, UpdateCommentRequest updateCommentRequest, CancellationToken cancellationToken)
        {
            return UpdateCommentAsync(commentId, updateCommentRequest, cancellationToken);
        }

        async Task<CreateCommentResponse> ICommentsService.CreateThreadedCommentAsync(string commentId, CreateCommentRequest createCommentRequest, CancellationToken cancellationToken)
        {
            return await CreateThreadedCommentAsync(commentId, createCommentRequest, cancellationToken);
        }
    }
}
