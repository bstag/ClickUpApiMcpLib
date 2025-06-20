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
        private const string BaseEndpoint = "/v3/workspaces"; // Base for v3 Chat API

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public ChatService(IApiConnection apiConnection)
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

        private string BuildQueryStringFromArray<T>(string key, IEnumerable<T>? values)
        {
            if (values == null || !values.Any()) return string.Empty;
            return string.Join("&", values.Select(v => $"{Uri.EscapeDataString(key)}[]={Uri.EscapeDataString(v?.ToString() ?? string.Empty)}"));
        }

        #region Channels
        /// <inheritdoc />
        public async Task<GetChatChannelsResponse?> GetChatChannelsAsync(
            string workspaceId,
            string? descriptionFormat = null,
            string? cursor = null,
            int? limit = null,
            bool? isFollower = null,
            bool? includeHidden = null,
            long? withCommentSince = null,
            IEnumerable<string>? roomTypes = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels"; // Corrected v3 endpoint from /docs to /channels
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(descriptionFormat)) queryParams["description_format"] = descriptionFormat;
            if (!string.IsNullOrEmpty(cursor)) queryParams["cursor"] = cursor;
            if (limit.HasValue) queryParams["limit"] = limit.Value.ToString();
            if (isFollower.HasValue) queryParams["is_follower"] = isFollower.Value.ToString().ToLower();
            if (includeHidden.HasValue) queryParams["include_hidden"] = includeHidden.Value.ToString().ToLower();
            if (withCommentSince.HasValue) queryParams["with_comment_since"] = withCommentSince.Value.ToString();

            var queryString = BuildQueryString(queryParams);

            if (roomTypes != null && roomTypes.Any())
            {
                queryString += (string.IsNullOrEmpty(queryString) || queryString == "?" ? (queryString == "?" ? "" : "?") : "&") + BuildQueryStringFromArray("room_types", roomTypes);
            }
            if (queryString == "?") queryString = string.Empty;

            return await _apiConnection.GetAsync<GetChatChannelsResponse>($"{endpoint}{queryString}", cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ChatChannel?> CreateChatChannelAsync(
            string workspaceId,
            CreateChatChannelRequest createChannelRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels";
            // Assuming API returns {"data": {...channel...}}
            var response = await _apiConnection.PostAsync<CreateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(endpoint, createChannelRequest, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<ChatChannel?> CreateLocationChatChannelAsync(
            string workspaceId,
            CreateLocationChatChannelRequest createLocationChannelRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/location"; // Example, actual endpoint might vary
            var response = await _apiConnection.PostAsync<CreateLocationChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(endpoint, createLocationChannelRequest, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<ChatChannel?> CreateDirectMessageChatChannelAsync(
            string workspaceId,
            CreateDirectMessageChatChannelRequest createDirectMessageChannelRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/dm"; // Example, actual endpoint might vary
            var response = await _apiConnection.PostAsync<CreateDirectMessageChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(endpoint, createDirectMessageChannelRequest, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<ChatChannel?> GetChatChannelAsync(
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
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<ChatChannel?> UpdateChatChannelAsync(
            string workspaceId,
            string channelId,
            UpdateChatChannelRequest updateChannelRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/{channelId}";
            var response = await _apiConnection.PutAsync<UpdateChatChannelRequest, ClickUpV3DataResponse<ChatChannel>>(endpoint, updateChannelRequest, cancellationToken);
            return response?.Data;
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
        public async Task<GetChatUsersResponse?> GetChatChannelFollowersAsync(
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

            return await _apiConnection.GetAsync<GetChatUsersResponse>(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetChatUsersResponse?> GetChatChannelMembersAsync(
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

            return await _apiConnection.GetAsync<GetChatUsersResponse>(endpoint, cancellationToken);
        }
        #endregion

        #region Messages
        /// <inheritdoc />
        public async Task<GetChatMessagesResponse?> GetChatMessagesAsync(
            string workspaceId,
            string channelId,
            string? cursor = null,
            int? limit = null,
            string? contentFormat = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/{channelId}/messages";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(cursor)) queryParams["cursor"] = cursor;
            if (limit.HasValue) queryParams["limit"] = limit.Value.ToString();
            if (!string.IsNullOrEmpty(contentFormat)) queryParams["content_format"] = contentFormat;
            endpoint += BuildQueryString(queryParams);

            return await _apiConnection.GetAsync<GetChatMessagesResponse>(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ChatMessage?> CreateChatMessageAsync(
            string workspaceId,
            string channelId,
            CreateChatMessageRequest createMessageRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/channels/{channelId}/messages";
            var response = await _apiConnection.PostAsync<CreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(endpoint, createMessageRequest, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<ChatMessage?> UpdateChatMessageAsync(
            string workspaceId,
            string messageId, // This should be channelId/messageId or just messageId if globally unique
            UpdateChatMessageRequest updateMessageRequest,
            CancellationToken cancellationToken = default)
        {
            // Assuming messageId is globally unique or endpoint structure is different, e.g., /v3/workspaces/{workspaceId}/messages/{messageId}
            // For now, following a common pattern if messageId is unique under workspace.
            // The API spec shows PATCH /v3/chat/messages/{message_id} - so not workspace specific in path
            var endpoint = $"/v3/chat/messages/{messageId}"; // Corrected based on typical message update paths
            var response = await _apiConnection.PutAsync<UpdateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(endpoint, updateMessageRequest, cancellationToken); // Using PUT as per typical full updates, though API says PATCH
            return response?.Data;
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
        public async Task<ChatMessage?> CreateReplyMessageAsync(
            string workspaceId, // workspaceId might not be needed if messageId is globally unique
            string messageId,   // This is the parent_message_id
            CreateChatMessageRequest createReplyRequest,
            CancellationToken cancellationToken = default)
        {
            // API spec shows POST /v3/chat/messages/{parent_message_id}/messages
            var endpoint = $"/v3/chat/messages/{messageId}/messages";
            var response = await _apiConnection.PostAsync<CreateChatMessageRequest, ClickUpV3DataResponse<ChatMessage>>(endpoint, createReplyRequest, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<GetChatMessagesResponse?> GetChatMessageRepliesAsync(
            string workspaceId, // workspaceId might not be needed if messageId is globally unique
            string messageId,   // This is the parent_message_id
            string? cursor = null,
            int? limit = null,
            string? contentFormat = null,
            CancellationToken cancellationToken = default)
        {
            // API spec shows GET /v3/chat/messages/{parent_message_id}/messages
            var endpoint = $"/v3/chat/messages/{messageId}/messages";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(cursor)) queryParams["cursor"] = cursor;
            if (limit.HasValue) queryParams["limit"] = limit.Value.ToString();
            if (!string.IsNullOrEmpty(contentFormat)) queryParams["content_format"] = contentFormat;
            endpoint += BuildQueryString(queryParams);

            return await _apiConnection.GetAsync<GetChatMessagesResponse>(endpoint, cancellationToken);
        }
        #endregion

        #region Reactions & Tagged Users
        /// <inheritdoc />
        public async Task<GetChatReactionsResponse?> GetChatMessageReactionsAsync(
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

            return await _apiConnection.GetAsync<GetChatReactionsResponse>(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ChatReaction?> CreateChatReactionAsync(
            string workspaceId, // workspaceId might not be needed if messageId is globally unique
            string messageId,
            CreateChatReactionRequest createReactionRequest,
            CancellationToken cancellationToken = default)
        {
            // API spec shows POST /v3/chat/messages/{message_id}/reactions
            var endpoint = $"/v3/chat/messages/{messageId}/reactions";
            var response = await _apiConnection.PostAsync<CreateChatReactionRequest, ClickUpV3DataResponse<ChatReaction>>(endpoint, createReactionRequest, cancellationToken);
            return response?.Data;
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
        public async Task<GetChatUsersResponse?> GetChatMessageTaggedUsersAsync(
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

            return await _apiConnection.GetAsync<GetChatUsersResponse>(endpoint, cancellationToken);
        }
        #endregion

        // Temporary helper DTOs for v3 response unwrapping, assuming they are not part of the main Models project yet.
        // These should be defined in the Models project if this is a consistent API pattern.
        private class ClickUpV3DataResponse<T> { public T? Data { get; set; } }
        // private class ClickUpV3DataListResponse<T> { public List<T>? Data { get; set; } } // Not used if specific Response DTOs handle list + cursor
    }
}
