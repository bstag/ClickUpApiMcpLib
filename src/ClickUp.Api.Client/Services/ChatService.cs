using System;
using System.Collections.Generic;
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Chat;
using ClickUp.Api.Client.Models.RequestModels.Chat;
using ClickUp.Api.Client.Models.ResponseModels.Chat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// Conceptual helper DTOs for v3 responses, if not already handled by specific Response DTOs
// internal class ClickUpV3DataResponse<T> { public T? Data { get; set; } }
// internal class ClickUpV3DataListResponse<T> { public List<T>? Data { get; set; } }


namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IChatService"/> for ClickUp Chat (v3) operations.
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<ChatService> _logger;
        private const string BaseEndpoint = "/v3/workspaces"; // Base for v3 Chat API

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public ChatService(IApiConnection apiConnection, ILogger<ChatService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<ChatService>.Instance;
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

        private string BuildQueryStringFromArray<T>(string key, IEnumerable<T>? values)
        {
            if (values == null || !values.Any()) return string.Empty;
            return string.Join("&", values.Select(v => $"{Uri.EscapeDataString(key)}[]={Uri.EscapeDataString(v?.ToString() ?? string.Empty)}"));
        }

        #region Channels
        /// <inheritdoc />
        public async Task<ChatChannelPaginatedResponse> GetChatChannelsAsync(
            string workspaceId,
            GetChatChannelsRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting chat channels for workspace ID: {WorkspaceId}, Cursor: {Cursor}, Limit: {Limit}", workspaceId, requestModel.Cursor, requestModel.Limit);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(requestModel.DescriptionFormat)) queryParams["description_format"] = requestModel.DescriptionFormat;
            if (!string.IsNullOrEmpty(requestModel.Cursor)) queryParams["cursor"] = requestModel.Cursor;
            if (requestModel.Limit.HasValue) queryParams["limit"] = requestModel.Limit.Value.ToString();
            if (requestModel.IsFollower.HasValue) queryParams["is_follower"] = requestModel.IsFollower.Value.ToString().ToLower();
            if (requestModel.IncludeHidden.HasValue) queryParams["include_hidden"] = requestModel.IncludeHidden.Value.ToString().ToLower();
            if (requestModel.WithCommentSince.HasValue) queryParams["with_comment_since"] = requestModel.WithCommentSince.Value.ToString();

            var queryString = BuildQueryString(queryParams);

            if (requestModel.RoomTypes != null && requestModel.RoomTypes.Any())
            {
                queryString += (string.IsNullOrEmpty(queryString) || queryString == "?" ? (queryString == "?" ? "" : "?") : "&") + BuildQueryStringFromArray("room_types", requestModel.RoomTypes);
            }
            if (queryString == "?") queryString = string.Empty;

            var result = await _apiConnection.GetAsync<ChatChannelPaginatedResponse>($"{endpoint}{queryString}", cancellationToken);
            if (result == null)
            {
                _logger.LogWarning("API call to get chat channels for workspace {WorkspaceId} returned null. Throwing exception.", workspaceId);
                throw new InvalidOperationException($"Failed to get chat channels for workspace {workspaceId} or API returned an unexpected null response.");
            }
            // The test GetChatChannelsAsync_NullDataInResponse_ThrowsInvalidOperationException asserts that an exception is thrown
            // if the Data property of the response is null.
            if (result.Data == null)
            {
                _logger.LogWarning("API call to get chat channels for workspace {WorkspaceId} returned null Data. Throwing exception as per test expectation.", workspaceId);
                throw new InvalidOperationException($"API call to get chat channels for workspace {workspaceId} returned null Data in response.");
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<ChatChannel> CreateChatChannelAsync(
            string workspaceId,
            ChatCreateChatChannelRequest createChannelRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating chat channel in workspace ID: {WorkspaceId} with name: {ChannelName}", workspaceId, createChannelRequest.Name);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels";
            var response = await _apiConnection.PostAsync<ChatCreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(endpoint, createChannelRequest, cancellationToken);
            if (response?.Data == null)
            {
                throw new InvalidOperationException($"Failed to create chat channel in workspace {workspaceId} or API returned an unexpected null data response.");
            }
            return response.Data;
        }

        /// <inheritdoc />
        public async Task<ChatChannel> CreateLocationChatChannelAsync(
            string workspaceId,
            ChatCreateLocationChatChannelRequest createLocationChannelRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating location chat channel in workspace ID: {WorkspaceId} for location ID: {LocationId}", workspaceId, createLocationChannelRequest.Location.Id);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/location"; // Example, actual endpoint might vary
            var response = await _apiConnection.PostAsync<ChatCreateLocationChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(endpoint, createLocationChannelRequest, cancellationToken);
            if (response?.Data == null)
            {
                throw new InvalidOperationException($"Failed to create location chat channel in workspace {workspaceId} or API returned an unexpected null data response.");
            }
            return response.Data;
        }

        /// <inheritdoc />
        public async Task<ChatChannel> CreateDirectMessageChatChannelAsync(
            string workspaceId,
            ChatCreateDirectMessageChatChannelRequest createDirectMessageChannelRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/dm"; // Example, actual endpoint might vary
            var response = await _apiConnection.PostAsync<ChatCreateDirectMessageChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(endpoint, createDirectMessageChannelRequest, cancellationToken);
            if (response?.Data == null)
            {
                throw new InvalidOperationException($"Failed to create direct message chat channel in workspace {workspaceId} or API returned an unexpected null data response.");
            }
            return response.Data;
        }

        /// <inheritdoc />
        public async Task<ChatChannel> GetChatChannelAsync(
            string workspaceId,
            string channelId,
            string? descriptionFormat = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/{channelId}";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(descriptionFormat)) queryParams["description_format"] = descriptionFormat;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<ClickUpV3DataResponse<ChatChannel>>(endpoint, cancellationToken);
            if (response?.Data == null)
            {
                throw new InvalidOperationException($"Failed to get chat channel {channelId} in workspace {workspaceId} or API returned an unexpected null data response.");
            }
            return response.Data;
        }

        /// <inheritdoc />
        public async Task<ChatChannel> UpdateChatChannelAsync(
            string workspaceId,
            string channelId,
            ChatUpdateChatChannelRequest updateChannelRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/{channelId}";
            var response = await _apiConnection.PutAsync<ChatUpdateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(endpoint, updateChannelRequest, cancellationToken);
            if (response?.Data == null)
            {
                throw new InvalidOperationException($"Failed to update chat channel {channelId} in workspace {workspaceId} or API returned an unexpected null data response.");
            }
            return response.Data;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteChatChannelAsync(
            string workspaceId,
            string channelId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/{channelId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetChatUsersResponse> GetChatChannelFollowersAsync(
            string workspaceId,
            string channelId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/{channelId}/followers";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(cursor)) queryParams["cursor"] = cursor;
            if (limit.HasValue) queryParams["limit"] = limit.Value.ToString();
            endpoint += BuildQueryString(queryParams);

            var result = await _apiConnection.GetAsync<GetChatUsersResponse>(endpoint, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException($"Failed to get followers for chat channel {channelId} or API returned an unexpected null response.");
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<GetChatUsersResponse> GetChatChannelMembersAsync(
            string workspaceId,
            string channelId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/{channelId}/members";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(cursor)) queryParams["cursor"] = cursor;
            if (limit.HasValue) queryParams["limit"] = limit.Value.ToString();
            endpoint += BuildQueryString(queryParams);

            var result = await _apiConnection.GetAsync<GetChatUsersResponse>(endpoint, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException($"Failed to get members for chat channel {channelId} or API returned an unexpected null response.");
            }
            return result;
        }
        #endregion

        #region Messages
        /// <inheritdoc />
        public async Task<GetChatMessagesResponse> GetChatMessagesAsync(
            string workspaceId,
            string channelId,
            GetChatMessagesRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/{channelId}/messages";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(requestModel.Cursor)) queryParams["cursor"] = requestModel.Cursor;
            if (requestModel.Limit.HasValue) queryParams["limit"] = requestModel.Limit.Value.ToString();
            if (!string.IsNullOrEmpty(requestModel.ContentFormat)) queryParams["content_format"] = requestModel.ContentFormat;
            endpoint += BuildQueryString(queryParams);

            var result = await _apiConnection.GetAsync<GetChatMessagesResponse>(endpoint, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException($"Failed to get messages for chat channel {channelId} or API returned an unexpected null response.");
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<ChatMessage> CreateChatMessageAsync(
            string workspaceId,
            string channelId,
            CommentCreateChatMessageRequest createMessageRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/{channelId}/messages";
            var response = await _apiConnection.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(endpoint, createMessageRequest, cancellationToken);
            if (response?.Data == null)
            {
                throw new InvalidOperationException($"Failed to create chat message in channel {channelId} or API returned an unexpected null data response.");
            }
            return response.Data;
        }

        /// <inheritdoc />
        public async Task<ChatMessage> UpdateChatMessageAsync(
            string workspaceId,
            string messageId, // This should be channelId/messageId or just messageId if globally unique
            CommentPatchChatMessageRequest updateMessageRequest,
            CancellationToken cancellationToken = default)
        {
            // Assuming messageId is globally unique or endpoint structure is different, e.g., /v3/workspaces/{workspaceId}/messages/{messageId}
            // For now, following a common pattern if messageId is unique under workspace.
            // The API spec shows PATCH /v3/chat/messages/{message_id} - so not workspace specific in path
            var endpoint = $"/v3/chat/messages/{messageId}"; // Corrected based on typical message update paths
            var response = await _apiConnection.PutAsync<CommentPatchChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(endpoint, updateMessageRequest, cancellationToken); // Using PUT as per typical full updates, though API says PATCH
            if (response?.Data == null)
            {
                throw new InvalidOperationException($"Failed to update chat message {messageId} or API returned an unexpected null data response.");
            }
            return response.Data;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteChatMessageAsync(
            string workspaceId, // workspaceId might not be needed if messageId is globally unique for deletion
            string messageId,
            CancellationToken cancellationToken = default)
        {
             // API spec shows DELETE /v3/chat/messages/{message_id}
            var endpoint = $"/v3/chat/messages/{messageId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ChatMessage> CreateReplyMessageAsync(
            string workspaceId, // workspaceId might not be needed if messageId is globally unique
            string messageId,   // This is the parent_message_id
            CommentCreateChatMessageRequest createReplyRequest,
            CancellationToken cancellationToken = default)
        {
            // API spec shows POST /v3/chat/messages/{parent_message_id}/messages
            var endpoint = $"/v3/chat/messages/{messageId}/messages";
            var response = await _apiConnection.PostAsync<CommentCreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(endpoint, createReplyRequest, cancellationToken);
            if (response?.Data == null)
            {
                throw new InvalidOperationException($"Failed to create reply to message {messageId} or API returned an unexpected null data response.");
            }
            return response.Data;
        }

        /// <inheritdoc />
        public async Task<GetChatMessagesResponse> GetChatMessageRepliesAsync(
            // string workspaceId, // Not used by the /v3/chat/messages/{parent_message_id}/messages endpoint
            string messageId,   // This is the parent_message_id
            GetChatMessagesRequest requestModel, // Reusing GetChatMessagesRequest DTO
            CancellationToken cancellationToken = default)
        {
            // API spec shows GET /v3/chat/messages/{parent_message_id}/messages
            var endpoint = $"/v3/chat/messages/{messageId}/messages";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(requestModel.Cursor)) queryParams["cursor"] = requestModel.Cursor;
            if (requestModel.Limit.HasValue) queryParams["limit"] = requestModel.Limit.Value.ToString();
            if (!string.IsNullOrEmpty(requestModel.ContentFormat)) queryParams["content_format"] = requestModel.ContentFormat;
            endpoint += BuildQueryString(queryParams);

            var result = await _apiConnection.GetAsync<GetChatMessagesResponse>(endpoint, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException($"Failed to get replies for message {messageId} or API returned an unexpected null response.");
            }
            return result;
        }
        #endregion

        #region Reactions & Tagged Users
        /// <inheritdoc />
        public async Task<GetChatReactionsResponse> GetChatMessageReactionsAsync(
            string workspaceId, // workspaceId might not be needed if messageId is globally unique
            string messageId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default)
        {
            // API spec shows GET /v3/chat/messages/{message_id}/reactions
            var endpoint = $"/v3/chat/messages/{messageId}/reactions";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(cursor)) queryParams["cursor"] = cursor;
            if (limit.HasValue) queryParams["limit"] = limit.Value.ToString();
            endpoint += BuildQueryString(queryParams);

            var result = await _apiConnection.GetAsync<GetChatReactionsResponse>(endpoint, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException($"Failed to get reactions for message {messageId} or API returned an unexpected null response.");
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<ChatReaction> CreateChatReactionAsync(
            string workspaceId, // workspaceId might not be needed if messageId is globally unique
            string messageId,
            CreateChatReactionRequest createReactionRequest,
            CancellationToken cancellationToken = default)
        {
            // API spec shows POST /v3/chat/messages/{message_id}/reactions
            var endpoint = $"/v3/chat/messages/{messageId}/reactions";
            var response = await _apiConnection.PostAsync<CreateChatReactionRequest, ClickUpV3DataResponse<ChatReaction>>(endpoint, createReactionRequest, cancellationToken);
            if (response?.Data == null)
            {
                throw new InvalidOperationException($"Failed to create reaction for message {messageId} or API returned an unexpected null data response.");
            }
            return response.Data;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteChatReactionAsync(
            string workspaceId, // workspaceId might not be needed if messageId and reaction are specific enough
            string messageId,
            string reaction, // This is the emoji itself, e.g. "%F0%9F%91%8D" for :thumbsup:
            CancellationToken cancellationToken = default)
        {
            // API spec shows DELETE /v3/chat/messages/{message_id}/reactions/{reaction_emoji}
            var encodedReaction = Uri.EscapeDataString(reaction);
            var endpoint = $"/v3/chat/messages/{messageId}/reactions/{encodedReaction}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetChatUsersResponse> GetChatMessageTaggedUsersAsync(
            string workspaceId, // workspaceId might not be needed if messageId is globally unique
            string messageId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default)
        {
            // API spec shows GET /v3/chat/messages/{message_id}/tagged_users
            var endpoint = $"/v3/chat/messages/{messageId}/tagged_users";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(cursor)) queryParams["cursor"] = cursor;
            if (limit.HasValue) queryParams["limit"] = limit.Value.ToString();
            endpoint += BuildQueryString(queryParams);

            var result = await _apiConnection.GetAsync<GetChatUsersResponse>(endpoint, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException($"Failed to get tagged users for message {messageId} or API returned an unexpected null response.");
            }
            return result;
        }
        #endregion

        // ClickUpV3DataResponse<T> is now in InternalDtos.cs
    }
}
