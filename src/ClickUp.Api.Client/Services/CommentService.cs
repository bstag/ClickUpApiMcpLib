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
using ClickUp.Api.Client.Models.RequestModels.Comments;
using ClickUp.Api.Client.Models.ResponseModels.Comments; // For CreateCommentResponse and potential GetCommentsResponse

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ICommentsService"/> for ClickUp Comment operations.
    /// </summary>
    public class CommentService : ICommentsService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public CommentService(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
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
        public async Task<IEnumerable<Comment>?> GetTaskCommentsAsync(
            string taskId,
            bool? customTaskIds = null,
            string? teamId = null,
            long? start = null,
            string? startId = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"task/{taskId}/comment";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            if (start.HasValue) queryParams["start"] = start.Value.ToString();
            if (!string.IsNullOrEmpty(startId)) queryParams["start_id"] = startId;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetCommentsResponse>(endpoint, cancellationToken); // Assuming API returns {"comments": [...]}
            return response?.Comments;
        }

        /// <inheritdoc />
        public async Task<CreateCommentResponse?> CreateTaskCommentAsync(
            string taskId,
            CreateCommentRequest createCommentRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"task/{taskId}/comment";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamId)) queryParams["team_id"] = teamId;
            endpoint += BuildQueryString(queryParams);

            return await _apiConnection.PostAsync<CreateCommentRequest, CreateCommentResponse>(endpoint, createCommentRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Comment>?> GetChatViewCommentsAsync(
            string viewId,
            long? start = null,
            string? startId = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"view/{viewId}/comment";
            var queryParams = new Dictionary<string, string?>();
            if (start.HasValue) queryParams["start"] = start.Value.ToString();
            if (!string.IsNullOrEmpty(startId)) queryParams["start_id"] = startId;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetCommentsResponse>(endpoint, cancellationToken); // Assuming API returns {"comments": [...]}
            return response?.Comments;
        }

        /// <inheritdoc />
        public async Task<CreateCommentResponse?> CreateChatViewCommentAsync(
            string viewId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"view/{viewId}/comment";
            return await _apiConnection.PostAsync<CreateCommentRequest, CreateCommentResponse>(endpoint, createCommentRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Comment>?> GetListCommentsAsync(
            string listId,
            long? start = null,
            string? startId = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"list/{listId}/comment";
            var queryParams = new Dictionary<string, string?>();
            if (start.HasValue) queryParams["start"] = start.Value.ToString();
            if (!string.IsNullOrEmpty(startId)) queryParams["start_id"] = startId;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetCommentsResponse>(endpoint, cancellationToken); // Assuming API returns {"comments": [...]}
            return response?.Comments;
        }

        /// <inheritdoc />
        public async Task<CreateCommentResponse?> CreateListCommentAsync(
            string listId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"list/{listId}/comment";
            return await _apiConnection.PostAsync<CreateCommentRequest, CreateCommentResponse>(endpoint, createCommentRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task UpdateCommentAsync(
            string commentId,
            UpdateCommentRequest updateCommentRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"comment/{commentId}";
            await _apiConnection.PutAsync(endpoint, updateCommentRequest, cancellationToken); // Assuming UpdateComment returns no significant body or just status
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteCommentAsync(
            string commentId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"comment/{commentId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Comment>?> GetThreadedCommentsAsync( // API is GET /comment/{comment_id} (Get Comment) which includes replies.
                                                                           // Or if specific endpoint for replies exists, use that.
                                                                           // The provided API ref was GET /comment/{comment_id}/reply, this is not standard.
                                                                           // Let's assume GET /comment/{comment_id} and we extract replies from the Comment DTO.
                                                                           // Or, if the endpoint is specifically for replies, the response DTO might be different.
                                                                           // For now, assuming it returns a list of comments that are replies.
            string commentId,
            CancellationToken cancellationToken = default)
        {
             // The API doc suggests "GET /v2/comment/{comment_id}/reply" but this is not standard.
             // More likely, one gets a comment and its 'replies' field, or this endpoint is a special case.
             // Let's assume it's a direct endpoint that returns a list of comments.
            var endpoint = $"comment/{commentId}/reply"; // This endpoint is hypothetical based on original comments.
                                                         // Standard GET /comment/{comment_id} would return the comment itself, which might contain replies.
                                                         // If this specific endpoint exists and returns a list of comments:
            var response = await _apiConnection.GetAsync<GetCommentsResponse>(endpoint, cancellationToken); // Assuming {"comments": [...]}
            return response?.Comments;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task CreateThreadedCommentAsync( // POST /v2/comment/{comment_id}/reply
            string commentId,
            CreateCommentRequest createCommentRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"comment/{commentId}/reply";
            // API doc says "200 with empty object". So PostAsync without TResponse.
            await _apiConnection.PostAsync(endpoint, createCommentRequest, cancellationToken);
        }
    }
}
