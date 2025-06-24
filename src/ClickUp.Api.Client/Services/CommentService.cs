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
        public async IAsyncEnumerable<Comment> GetListCommentsStreamAsync(
            string listId,
            long? start = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Streaming list comments for list ID: {ListId}, Start: {Start}", listId, start);
            string? currentStartId = null;
            bool hasMore = true;
            long? currentStartTimestamp = start;

            while (hasMore)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Fetching next page of list comments for list ID: {ListId}, Start: {Start}, StartId: {StartId}", listId, currentStartTimestamp, currentStartId);

                var commentsPage = await GetListCommentsAsync(
                    listId,
                    currentStartTimestamp,
                    currentStartId,
                    cancellationToken).ConfigureAwait(false);

                if (commentsPage == null || !commentsPage.Any())
                {
                    _logger.LogDebug("No more list comments found for list ID: {ListId}", listId);
                    hasMore = false;
                }
                else
                {
                    Comment? lastComment = null;
                    foreach (var comment in commentsPage)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        yield return comment;
                        lastComment = comment;
                    }
                    currentStartId = lastComment?.Id;

                    if (currentStartTimestamp.HasValue)
                    {
                        currentStartTimestamp = null;
                    }
                }
            }
            _logger.LogInformation("Finished streaming list comments for list ID: {ListId}", listId);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Comment> GetChatViewCommentsStreamAsync(
            string viewId,
            long? start = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Streaming chat view comments for view ID: {ViewId}, Start: {Start}", viewId, start);
            string? currentStartId = null;
            bool hasMore = true;
            long? currentStartTimestamp = start;

            while (hasMore)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Fetching next page of chat view comments for view ID: {ViewId}, Start: {Start}, StartId: {StartId}", viewId, currentStartTimestamp, currentStartId);

                var commentsPage = await GetChatViewCommentsAsync(
                    viewId,
                    currentStartTimestamp,
                    currentStartId,
                    cancellationToken).ConfigureAwait(false);

                if (commentsPage == null || !commentsPage.Any())
                {
                    _logger.LogDebug("No more chat view comments found for view ID: {ViewId}", viewId);
                    hasMore = false;
                }
                else
                {
                    Comment? lastComment = null;
                    foreach (var comment in commentsPage)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        yield return comment;
                        lastComment = comment;
                    }
                    currentStartId = lastComment?.Id;

                    if (currentStartTimestamp.HasValue)
                    {
                        currentStartTimestamp = null;
                    }
                }
            }
            _logger.LogInformation("Finished streaming chat view comments for view ID: {ViewId}", viewId);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Comment> GetTaskCommentsStreamAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            long? start = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Streaming task comments for task ID: {TaskId}, Start: {Start}", taskId, start);
            string? currentStartId = null;
            bool hasMore = true;

            // The 'start' (timestamp) parameter is only used for the first request according to ClickUp docs.
            // For subsequent pages, 'start_id' is used.
            long? currentStartTimestamp = start;

            while (hasMore)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Fetching next page of task comments for task ID: {TaskId}, Start: {Start}, StartId: {StartId}", taskId, currentStartTimestamp, currentStartId);

                var request = new GetTaskCommentsRequest(taskId)
                {
                    CustomTaskIds = customTaskIds,
                    TeamId = teamId,
                    Start = currentStartTimestamp, // Use the current start timestamp
                    StartId = currentStartId
                };
                var commentsPage = await GetTaskCommentsAsync(request, cancellationToken).ConfigureAwait(false);

                if (commentsPage == null || !commentsPage.Any())
                {
                    _logger.LogDebug("No more comments found for task ID: {TaskId}", taskId);
                    hasMore = false;
                }
                else
                {
                    Comment? lastComment = null;
                    foreach (var comment in commentsPage)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        yield return comment;
                        lastComment = comment;
                    }
                    currentStartId = lastComment?.Id; // Prepare for the next iteration using the ID of the last comment seen

                    // After the first successful fetch, we should rely on start_id, not the initial 'start' timestamp.
                    if (currentStartTimestamp.HasValue)
                    {
                        currentStartTimestamp = null;
                    }

                    // If the number of comments returned is less than a typical page limit (e.g. 100, though API doesn't specify page size for comments),
                    // it might indicate the end, but the reliable way is an empty response.
                    // For now, we continue as long as commentsPage has items.
                }
            }
            _logger.LogInformation("Finished streaming task comments for task ID: {TaskId}", taskId);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Comment>> GetTaskCommentsAsync(
            GetTaskCommentsRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting task comments for task ID: {TaskId}, Start: {Start}, StartId: {StartId}", requestModel.TaskId, requestModel.Start, requestModel.StartId);
            var endpoint = $"task/{requestModel.TaskId}/comment";
            var queryParams = new Dictionary<string, string?>();
            if (requestModel.CustomTaskIds.HasValue) queryParams["custom_task_ids"] = requestModel.CustomTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(requestModel.TeamId)) queryParams["team_id"] = requestModel.TeamId;
            if (requestModel.Start.HasValue) queryParams["start"] = requestModel.Start.Value.ToString();
            if (!string.IsNullOrEmpty(requestModel.StartId)) queryParams["start_id"] = requestModel.StartId;
            endpoint += BuildQueryString(queryParams);

            // The interface ICommentsService.GetTaskCommentsAsync returns Task<IEnumerable<Comment>>
            // but the actual API call might return a wrapper like GetTaskCommentsResponse which contains a list of comments.
            // Assuming _apiConnection.GetAsync correctly deserializes to GetTaskCommentsResponse.
            var responseWrapper = await _apiConnection.GetAsync<GetTaskCommentsResponse>(endpoint, cancellationToken);
            return responseWrapper?.Comments ?? Enumerable.Empty<Comment>();
        }

        /// <inheritdoc />
        public async Task<CreateCommentResponse> CreateTaskCommentAsync(
            string taskId,
            CreateTaskCommentRequest createCommentRequest, // Changed to CreateTaskCommentRequest
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

            var response = await _apiConnection.PostAsync<CreateTaskCommentRequest, CreateCommentResponse>(endpoint, createCommentRequest, cancellationToken); // Changed generic type
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
