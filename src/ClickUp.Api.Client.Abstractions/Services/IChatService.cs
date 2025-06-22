using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Chat;
using ClickUp.Api.Client.Models.RequestModels.Chat;
using ClickUp.Api.Client.Models.ResponseModels.Chat;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Chat operations (v3 API, experimental).
    /// </summary>
    /// <remarks>
    /// The Chat API is experimental and subject to change.
    /// This service provides methods for managing chat channels, messages, reactions, and related entities.
    /// Covered API Endpoints (non-exhaustive list):
    /// - Channels: `GET /workspace/{workspace_id}/channel`, `POST /workspace/{workspace_id}/channel`, `POST /workspace/{workspace_id}/channel/location`, `POST /workspace/{workspace_id}/channel/dm`, `GET /workspace/{workspace_id}/channel/{channel_id}`, `PATCH /workspace/{workspace_id}/channel/{channel_id}`, `DELETE /workspace/{workspace_id}/channel/{channel_id}`, `GET /workspace/{workspace_id}/channel/{channel_id}/follower`, `GET /workspace/{workspace_id}/channel/{channel_id}/member`
    /// - Messages: `GET /workspace/{workspace_id}/channel/{channel_id}/message`, `POST /workspace/{workspace_id}/channel/{channel_id}/message`, `PATCH /workspace/{workspace_id}/message/{message_id}`, `DELETE /workspace/{workspace_id}/message/{message_id}`, `POST /workspace/{workspace_id}/message/{message_id}/reply`, `GET /workspace/{workspace_id}/message/{message_id}/reply`
    /// - Reactions: `GET /workspace/{workspace_id}/message/{message_id}/reaction`, `POST /workspace/{workspace_id}/message/{message_id}/reaction`, `DELETE /workspace/{workspace_id}/message/{message_id}/reaction/{reaction}`
    /// - Tagged Users: `GET /workspace/{workspace_id}/message/{message_id}/user_tagged`
    /// </remarks>
    public interface IChatService
    {
        #region Channels
        /// <summary>
        /// Retrieves a paginated list of Chat Channels within a specified Workspace.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="descriptionFormat">Optional. The desired format for the channel description (e.g., "text/plain", "text/md").</param>
        /// <param name="cursor">Optional. The cursor for pagination, used to fetch the next set of results.</param>
        /// <param name="limit">Optional. The maximum number of channels to return per page.</param>
        /// <param name="isFollower">Optional. If true, filters channels to those the authenticated user is following.</param>
        /// <param name="includeHidden">Optional. If true, includes Direct Message (DM) or Group DM channels that have been explicitly closed by the user.</param>
        /// <param name="withCommentSince">Optional. A Unix timestamp (in milliseconds). If provided, only returns channels that have had comments since this time.</param>
        /// <param name="roomTypes">Optional. An enumerable of strings specifying the types of channels to return (e.g., "CHANNEL", "DM", "GROUP_DM").</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ChatChannelPaginatedResponse"/> object with the list of channels and pagination information.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access channels in this workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<ChatChannelPaginatedResponse> GetChatChannelsAsync(
            string workspaceId,
            string? descriptionFormat = null,
            string? cursor = null,
            int? limit = null,
            bool? isFollower = null,
            bool? includeHidden = null,
            long? withCommentSince = null,
            IEnumerable<string>? roomTypes = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Chat Channel within a Workspace that is not tied to a specific location (like a Space, Folder, or List).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace where the channel will be created.</param>
        /// <param name="createChatChannelRequest">An object containing the details for the new channel, such as its name and description.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created or existing <see cref="ChatChannel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createChatChannelRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create channels in this workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ChatChannel> CreateChatChannelAsync(
            string workspaceId,
            ChatCreateChatChannelRequest createChatChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Chat Channel that is associated with a specific location (Space, Folder, or List) within a Workspace.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="createLocationChatChannelRequest">An object containing details for the new location-based channel, including location ID and type.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created or existing <see cref="ChatChannel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createLocationChatChannelRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create channels for the specified location.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ChatChannel> CreateLocationChatChannelAsync(
            string workspaceId,
            ChatCreateLocationChatChannelRequest createLocationChatChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Direct Message (DM) or Group Direct Message (Group DM) channel.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="createDirectMessageChatChannelRequest">An object containing the list of user IDs to be included in the DM or Group DM.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created or existing DM/Group DM <see cref="ChatChannel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createDirectMessageChatChannelRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create DMs/Group DMs.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ChatChannel> CreateDirectMessageChatChannelAsync(
            string workspaceId,
            ChatCreateDirectMessageChatChannelRequest createDirectMessageChatChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details of a specific Chat Channel by its ID.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="channelId">The unique identifier of the Channel to retrieve.</param>
        /// <param name="descriptionFormat">Optional. The desired format for the channel description.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the details of the requested <see cref="ChatChannel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="channelId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this channel.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ChatChannel> GetChatChannelAsync(
            string workspaceId,
            string channelId,
            string? descriptionFormat = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the properties of an existing Chat Channel.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="channelId">The unique identifier of the Channel to update.</param>
        /// <param name="updateChatChannelRequest">An object containing the properties to update for the channel.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="ChatChannel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="channelId"/>, or <paramref name="updateChatChannelRequest"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this channel.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ChatChannel> UpdateChatChannelAsync(
            string workspaceId,
            string channelId,
            ChatUpdateChatChannelRequest updateChatChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specified Chat Channel.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="channelId">The unique identifier of the Channel to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="channelId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this channel.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteChatChannelAsync(
            string workspaceId,
            string channelId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of users who are followers of a specific Chat Channel.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="channelId">The unique identifier of the Channel.</param>
        /// <param name="cursor">Optional. The cursor for pagination.</param>
        /// <param name="limit">Optional. The maximum number of followers to return per page.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetChatUsersResponse"/> object with the list of followers (<see cref="ChatUser"/>) and pagination information.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="channelId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetChatUsersResponse> GetChatChannelFollowersAsync(
            string workspaceId,
            string channelId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of users who are members of a specific Chat Channel.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="channelId">The unique identifier of the Channel.</param>
        /// <param name="cursor">Optional. The cursor for pagination.</param>
        /// <param name="limit">Optional. The maximum number of members to return per page.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetChatUsersResponse"/> object with the list of members (<see cref="ChatUser"/>) and pagination information.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="channelId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetChatUsersResponse> GetChatChannelMembersAsync(
            string workspaceId,
            string channelId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default);
        #endregion

        #region Messages
        /// <summary>
        /// Retrieves a paginated list of messages for a specified Chat Channel.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="channelId">The unique identifier of the Channel from which to retrieve messages.</param>
        /// <param name="cursor">Optional. The cursor for pagination.</param>
        /// <param name="limit">Optional. The maximum number of messages to return per page.</param>
        /// <param name="contentFormat">Optional. The desired format for the message content.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetChatMessagesResponse"/> object with the list of messages (<see cref="ChatMessage"/>) and pagination information.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="channelId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access messages in this channel.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetChatMessagesResponse> GetChatMessagesAsync(
            string workspaceId,
            string channelId,
            string? cursor = null,
            int? limit = null,
            string? contentFormat = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a new message to a specified Chat Channel.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="channelId">The unique identifier of the Channel where the message will be sent.</param>
        /// <param name="createMessageRequest">An object containing the details of the message to be sent, such as its content.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="ChatMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="channelId"/>, or <paramref name="createMessageRequest"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to send messages to this channel.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ChatMessage> CreateChatMessageAsync(
            string workspaceId,
            string channelId,
            CommentCreateChatMessageRequest createMessageRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing message in a Chat Channel.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="messageId">The unique identifier of the message to update.</param>
        /// <param name="updateMessageRequest">An object containing the properties to update for the message.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="ChatMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="messageId"/>, or <paramref name="updateMessageRequest"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this message.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ChatMessage> UpdateChatMessageAsync(
            string workspaceId,
            string messageId,
            CommentPatchChatMessageRequest updateMessageRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specified message from a Chat Channel.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="messageId">The unique identifier of the message to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="messageId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this message.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteChatMessageAsync(
            string workspaceId,
            string messageId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a reply to an existing message, effectively starting or continuing a thread.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="messageId">The unique identifier of the parent message to which this reply is being created.</param>
        /// <param name="createReplyRequest">An object containing the details of the reply message to be sent.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created reply <see cref="ChatMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="messageId"/>, or <paramref name="createReplyRequest"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the parent message with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to reply to this message.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ChatMessage> CreateReplyMessageAsync(
            string workspaceId,
            string messageId,
            CommentCreateChatMessageRequest createReplyRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of replies for a specified parent message.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="messageId">The unique identifier of the parent message for which to retrieve replies.</param>
        /// <param name="cursor">Optional. The cursor for pagination.</param>
        /// <param name="limit">Optional. The maximum number of replies to return per page.</param>
        /// <param name="contentFormat">Optional. The desired format for the message content.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetChatMessagesResponse"/> object with the list of reply messages (<see cref="ChatMessage"/>) and pagination information.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="messageId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the parent message with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access replies for this message.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetChatMessagesResponse> GetChatMessageRepliesAsync(
            string workspaceId,
            string messageId,
            string? cursor = null,
            int? limit = null,
            string? contentFormat = null,
            CancellationToken cancellationToken = default);
        #endregion

        #region Reactions & Tagged Users
        /// <summary>
        /// Retrieves a paginated list of reactions for a specified message.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="messageId">The unique identifier of the message for which to retrieve reactions.</param>
        /// <param name="cursor">Optional. The cursor for pagination.</param>
        /// <param name="limit">Optional. The maximum number of reactions to return per page.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetChatReactionsResponse"/> object with the list of reactions (<see cref="ChatReaction"/>) and pagination information.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="messageId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access reactions for this message.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetChatReactionsResponse> GetChatMessageReactionsAsync(
            string workspaceId,
            string messageId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a reaction to a specified message.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="messageId">The unique identifier of the message to which the reaction will be added.</param>
        /// <param name="createReactionRequest">An object containing the details of the reaction, typically the emoji code.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="ChatReaction"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="messageId"/>, or <paramref name="createReactionRequest"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to react to this message.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ChatReaction> CreateChatReactionAsync(
            string workspaceId,
            string messageId,
            CreateChatReactionRequest createReactionRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specific reaction from a message.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="messageId">The unique identifier of the message from which the reaction will be deleted.</param>
        /// <param name="reaction">The name or emoji code of the reaction to delete (e.g., ":thumbsup:"). This typically corresponds to the `reaction` field in the <see cref="ChatReaction"/> object.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="messageId"/>, or <paramref name="reaction"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message or the specified reaction on that message is not found.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this reaction (e.g., it's not their own reaction).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteChatReactionAsync(
            string workspaceId,
            string messageId,
            string reaction,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of users who were tagged in a specific message.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="messageId">The unique identifier of the message from which to retrieve tagged users.</param>
        /// <param name="cursor">Optional. The cursor for pagination.</param>
        /// <param name="limit">Optional. The maximum number of tagged users to return per page.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetChatUsersResponse"/> object with the list of tagged users (<see cref="ChatUser"/>) and pagination information.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="messageId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<GetChatUsersResponse> GetChatMessageTaggedUsersAsync(
            string workspaceId,
            string messageId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
