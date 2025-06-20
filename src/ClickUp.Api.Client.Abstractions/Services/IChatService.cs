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
        /// <param name="createChannelRequest">Details for creating the Channel.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created or existing <see cref="ChatChannel"/>.</returns>
        Task<ChatChannel> CreateChatChannelAsync(
            string workspaceId,
            CreateChatChannelRequest createChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a Channel on a Space, Folder, or List.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="createLocationChannelRequest">Details for creating the location-based Channel.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created or existing <see cref="ChatChannel"/>.</returns>
        Task<ChatChannel> CreateLocationChatChannelAsync(
            string workspaceId,
            CreateLocationChatChannelRequest createLocationChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Direct Message or Group Direct Message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="createDirectMessageChannelRequest">User IDs for the DM/Group DM.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created or existing Direct Message <see cref="ChatChannel"/>.</returns>
        Task<ChatChannel> CreateDirectMessageChatChannelAsync(
            string workspaceId,
            CreateDirectMessageChatChannelRequest createDirectMessageChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific Channel by its ID.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel.</param>
        /// <param name="descriptionFormat">Optional. Format of the Channel Description.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="ChatChannel"/>.</returns>
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
        /// <param name="updateChannelRequest">Details for updating the Channel.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="ChatChannel"/>.</returns>
        Task<ChatChannel> UpdateChatChannelAsync(
            string workspaceId,
            string channelId,
            UpdateChatChannelRequest updateChannelRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
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
        Task<ChatMessage> CreateChatMessageAsync(
            string workspaceId,
            string channelId,
            CreateChatMessageRequest createMessageRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message to update.</param>
        /// <param name="updateMessageRequest">Details for updating the message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="ChatMessage"/>.</returns>
        Task<ChatMessage> UpdateChatMessageAsync(
            string workspaceId,
            string messageId,
            UpdateChatMessageRequest updateMessageRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
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
        Task<ChatMessage> CreateReplyMessageAsync(
            string workspaceId,
            string messageId,
            CreateChatMessageRequest createReplyRequest, // Assuming replies use the same request DTO as new messages
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
        Task<GetChatUsersResponse> GetChatMessageTaggedUsersAsync(
            string workspaceId,
            string messageId,
            string? cursor = null,
            int? limit = null,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
