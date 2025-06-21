using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Chat; // Assuming Chat DTOs (ChatChannel, ChatMessage, ChatUser, ChatReaction) are here
using ClickUp.Api.Client.Models.RequestModels.Chat; // Assuming Request DTOs are here
using ClickUp.Api.Client.Models.ResponseModels.Chat; // Assuming Response DTOs (GetChatChannelsResponse etc.) are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Chat (Experimental) operations in the ClickUp API (v3).
    /// </summary>
    public interface IChatService
    {
        #region Channels
        /// <summary>
        /// Retrieves Channels in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="descriptionFormat">Optional. Format of the Channel Description (e.g., "text/plain", "text/md").</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <param name="isFollower">Optional. Filter to Channels the user is following.</param>
        /// <param name="includeHidden">Optional. Include DMs/Group DMs that have been explicitly closed.</param>
        /// <param name="withCommentSince">Optional. Only return Channels with comments since the given Unix timestamp (ms).</param>
        /// <param name="roomTypes">Optional. Types of Channels to return (e.g., "CHANNEL", "DM", "GROUP_DM").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="ChatChannelPaginatedResponse"/> object containing a paginated list of channels and next cursor.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access the workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// Creates a new Channel not tied to a specific location.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="createChatChannelRequest">Details for creating the Channel.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created or existing <see cref="ChatChannel"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createChatChannelRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<ChatChannel> CreateChatChannelAsync(
            string workspaceId,
            ChatCreateChatChannelRequest createChatChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a Channel on a Space, Folder, or List.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="createLocationChatChannelRequest">Details for creating the location-based Channel.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created or existing <see cref="ChatChannel"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createLocationChatChannelRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<ChatChannel> CreateLocationChatChannelAsync(
            string workspaceId,
            ChatCreateLocationChatChannelRequest createLocationChatChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Direct Message or Group Direct Message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="createDirectMessageChatChannelRequest">User IDs for the DM/Group DM.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created or existing Direct Message <see cref="ChatChannel"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createDirectMessageChatChannelRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<ChatChannel> CreateDirectMessageChatChannelAsync(
            string workspaceId,
            ChatCreateDirectMessageChatChannelRequest createDirectMessageChatChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific Channel by its ID.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel.</param>
        /// <param name="descriptionFormat">Optional. Format of the Channel Description.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="ChatChannel"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="channelId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this channel.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<ChatChannel> GetChatChannelAsync(
            string workspaceId,
            string channelId,
            string? descriptionFormat = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel to update.</param>
        /// <param name="updateChatChannelRequest">Details for updating the Channel.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="ChatChannel"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="channelId"/>, or <paramref name="updateChatChannelRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this channel.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<ChatChannel> UpdateChatChannelAsync(
            string workspaceId,
            string channelId,
            ChatUpdateChatChannelRequest updateChatChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="channelId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this channel.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task DeleteChatChannelAsync(
            string workspaceId,
            string channelId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves followers of a specific Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetChatUsersResponse"/> object containing a paginated list of channel followers (<see cref="ChatUser"/>) and next cursor.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="channelId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<GetChatUsersResponse> GetChatChannelFollowersAsync(
            string workspaceId,
            string channelId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves members of a specific Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetChatUsersResponse"/> object containing a paginated list of channel members (<see cref="ChatUser"/>) and next cursor.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="channelId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<GetChatUsersResponse> GetChatChannelMembersAsync(
            string workspaceId,
            string channelId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default);
        #endregion

        #region Messages
        /// <summary>
        /// Retrieves messages for a specified Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <param name="contentFormat">Optional. Format of the message content.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetChatMessagesResponse"/> object containing a paginated list of messages (<see cref="ChatMessage"/>) and next cursor.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="channelId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access messages in this channel.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<GetChatMessagesResponse> GetChatMessagesAsync(
            string workspaceId,
            string channelId,
            string? cursor = null,
            int? limit = null,
            string? contentFormat = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message to a Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel.</param>
        /// <param name="createMessageRequest">Details of the message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="ChatMessage"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="channelId"/>, or <paramref name="createMessageRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the channel with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to send messages to this channel.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<ChatMessage> CreateChatMessageAsync(
            string workspaceId,
            string channelId,
            CommentCreateChatMessageRequest createMessageRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message to update.</param>
        /// <param name="updateMessageRequest">Details for updating the message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="ChatMessage"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="messageId"/>, or <paramref name="updateMessageRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this message.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<ChatMessage> UpdateChatMessageAsync(
            string workspaceId,
            string messageId,
            CommentPatchChatMessageRequest updateMessageRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="messageId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this message.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task DeleteChatMessageAsync(
            string workspaceId,
            string messageId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a reply to a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the parent message.</param>
        /// <param name="createReplyRequest">Details of the reply message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created reply <see cref="ChatMessage"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="messageId"/>, or <paramref name="createReplyRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the parent message with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to reply to this message.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<ChatMessage> CreateReplyMessageAsync(
            string workspaceId,
            string messageId,
            CommentCreateChatMessageRequest createReplyRequest, // Assuming replies use the same request DTO as new messages
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves replies to a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the parent message.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <param name="contentFormat">Optional. Format of the message content.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetChatMessagesResponse"/> object containing a paginated list of reply messages (<see cref="ChatMessage"/>) and next cursor.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="messageId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the parent message with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access replies for this message.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// Retrieves reactions for a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetChatReactionsResponse"/> object containing a paginated list of reactions (<see cref="ChatReaction"/>) and next cursor.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="messageId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access reactions for this message.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<GetChatReactionsResponse> GetChatMessageReactionsAsync(
            string workspaceId,
            string messageId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a reaction to a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="createReactionRequest">The reaction emoji details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="ChatReaction"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="messageId"/>, or <paramref name="createReactionRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to react to this message.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<ChatReaction> CreateChatReactionAsync(
            string workspaceId,
            string messageId,
            CreateChatReactionRequest createReactionRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a message reaction.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="reaction">The name/emoji of the reaction to delete (e.g., ":thumbsup:").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="messageId"/>, or <paramref name="reaction"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message or reaction is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this reaction.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task DeleteChatReactionAsync(
            string workspaceId,
            string messageId,
            string reaction, // Typically, the reaction itself (emoji code) is used as an identifier here
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves users tagged in a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetChatUsersResponse"/> object containing a paginated list of tagged users (<see cref="ChatUser"/>) and next cursor.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="messageId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the message with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this information.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<GetChatUsersResponse> GetChatMessageTaggedUsersAsync(
            string workspaceId,
            string messageId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
