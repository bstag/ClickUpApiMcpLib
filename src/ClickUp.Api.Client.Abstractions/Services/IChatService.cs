using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Chat (Experimental) operations in the ClickUp API (v3)
    // Based on endpoints under the "Chat (Experimental)" tag

    public interface IChatService
    {
        #region Channels
        /// <summary>
        /// Retrieves Channels in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="descriptionFormat">Optional. Format of the Channel Description.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <param name="isFollower">Optional. Filter to Channels the user is following.</param>
        /// <param name="includeHidden">Optional. Include DMs/Group DMs that have been explicitly closed.</param>
        /// <param name="withCommentSince">Optional. Only return Channels with comments since the given timestamp.</param>
        /// <param name="roomTypes">Optional. Types of Channels to return (CHANNEL, DM, GROUP_DM).</param>
        /// <returns>A paginated list of Channels.</returns>
        Task<object> GetChatChannelsAsync(long workspaceId, string? descriptionFormat = null, string? cursor = null, int? limit = null, bool? isFollower = null, bool? includeHidden = null, long? withCommentSince = null, IEnumerable<string>? roomTypes = null);
        // Note: Return type should be a DTO containing 'data' (IEnumerable<ChatChannelDto>) and 'next_cursor'.

        /// <summary>
        /// Creates a new Channel not tied to a specific location.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="createChannelRequest">Details for creating the Channel.</param>
        /// <returns>The created or existing Channel.</returns>
        Task<object> CreateChatChannelAsync(long workspaceId, object createChannelRequest);
        // Note: createChannelRequest should be CreateChannelRequest. Return type should be ChatChannelDto (wrapped in 'data').

        /// <summary>
        /// Creates a Channel on a Space, Folder, or List.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="createLocationChannelRequest">Details for creating the location-based Channel.</param>
        /// <returns>The created or existing Channel.</returns>
        Task<object> CreateLocationChatChannelAsync(long workspaceId, object createLocationChannelRequest);
        // Note: createLocationChannelRequest should be CreateLocationChannelRequest. Return type should be ChatChannelDto (wrapped in 'data').

        /// <summary>
        /// Creates a new Direct Message or Group Direct Message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="createDirectMessageChannelRequest">User IDs for the DM/Group DM.</param>
        /// <returns>The created or existing Direct Message Channel.</returns>
        Task<object> CreateDirectMessageChatChannelAsync(long workspaceId, object createDirectMessageChannelRequest);
        // Note: createDirectMessageChannelRequest should be CreateDirectMessageChannelRequest. Return type should be ChatChannelDto (wrapped in 'data').

        /// <summary>
        /// Retrieves a specific Channel by its ID.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel.</param>
        /// <param name="descriptionFormat">Optional. Format of the Channel Description.</param>
        /// <returns>Details of the Channel.</returns>
        Task<object> GetChatChannelAsync(long workspaceId, string channelId, string? descriptionFormat = null);
        // Note: Return type should be ChatChannelDto (wrapped in 'data').

        /// <summary>
        /// Updates a Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel to update.</param>
        /// <param name="updateChannelRequest">Details for updating the Channel.</param>
        /// <returns>The updated Channel.</returns>
        Task<object> UpdateChatChannelAsync(long workspaceId, string channelId, object updateChannelRequest);
        // Note: updateChannelRequest should be UpdateChannelRequest. Return type should be ChatChannelDto (wrapped in 'data').

        /// <summary>
        /// Deletes a Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteChatChannelAsync(long workspaceId, string channelId);
        // Note: API returns 204 No Content.

        /// <summary>
        /// Retrieves followers of a specific Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <returns>A paginated list of Channel followers.</returns>
        Task<object> GetChatChannelFollowersAsync(long workspaceId, string channelId, string? cursor = null, int? limit = null);
        // Note: Return type should be a DTO with 'data' (IEnumerable<ChatUserDto>) and 'next_cursor'.

        /// <summary>
        /// Retrieves members of a specific Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <returns>A paginated list of Channel members.</returns>
        Task<object> GetChatChannelMembersAsync(long workspaceId, string channelId, string? cursor = null, int? limit = null);
        // Note: Return type should be a DTO with 'data' (IEnumerable<ChatUserDto>) and 'next_cursor'.
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
        /// <returns>A paginated list of messages.</returns>
        Task<object> GetChatMessagesAsync(long workspaceId, string channelId, string? cursor = null, int? limit = null, string? contentFormat = null);
        // Note: Return type should be a DTO with 'data' (IEnumerable<ChatMessageDto>) and 'next_cursor'.

        /// <summary>
        /// Sends a message to a Channel.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="channelId">The ID of the Channel.</param>
        /// <param name="createMessageRequest">Details of the message to send.</param>
        /// <returns>The created message.</returns>
        Task<object> CreateChatMessageAsync(long workspaceId, string channelId, object createMessageRequest);
        // Note: createMessageRequest should be CreateChatMessageRequest. Return type should be ChatMessageDto.

        /// <summary>
        /// Updates a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message to update.</param>
        /// <param name="patchMessageRequest">Details for updating the message.</param>
        /// <returns>The updated message.</returns>
        Task<object> UpdateChatMessageAsync(long workspaceId, string messageId, object patchMessageRequest);
        // Note: patchMessageRequest should be PatchChatMessageRequest. Return type should be ChatMessageDto.

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteChatMessageAsync(long workspaceId, string messageId);
        // Note: API returns 204 No Content.

        /// <summary>
        /// Creates a reply to a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the parent message.</param>
        /// <param name="createReplyRequest">Details of the reply message to send.</param>
        /// <returns>The created reply message.</returns>
        Task<object> CreateReplyMessageAsync(long workspaceId, string messageId, object createReplyRequest);
        // Note: createReplyRequest should be CreateChatMessageRequest (as it's similar). Return type should be ChatMessageDto.

        /// <summary>
        /// Retrieves replies to a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the parent message.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <param name="contentFormat">Optional. Format of the message content.</param>
        /// <returns>A paginated list of reply messages.</returns>
        Task<object> GetChatMessageRepliesAsync(long workspaceId, string messageId, string? cursor = null, int? limit = null, string? contentFormat = null);
        // Note: Return type should be a DTO with 'data' (IEnumerable<ChatMessageDto>) and 'next_cursor'.
        #endregion

        #region Reactions & Tagged Users
        /// <summary>
        /// Retrieves reactions for a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <returns>A paginated list of reactions.</returns>
        Task<object> GetChatMessageReactionsAsync(long workspaceId, string messageId, string? cursor = null, int? limit = null);
        // Note: Return type should be a DTO with 'data' (IEnumerable<ChatReactionDto>) and 'next_cursor'.

        /// <summary>
        /// Creates a reaction to a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="createReactionRequest">The reaction emoji name.</param>
        /// <returns>The created reaction.</returns>
        Task<object> CreateChatReactionAsync(long workspaceId, string messageId, object createReactionRequest);
        // Note: createReactionRequest should be CreateChatReactionRequest. Return type should be ChatReactionDto.

        /// <summary>
        /// Deletes a message reaction.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="reaction">The name of the reaction to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteChatReactionAsync(long workspaceId, string messageId, string reaction);
        // Note: API returns 204 No Content.

        /// <summary>
        /// Retrieves users tagged in a message.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="messageId">The ID of the message.</param>
        /// <param name="cursor">Optional. Cursor for pagination.</param>
        /// <param name="limit">Optional. Maximum number of results per page.</param>
        /// <returns>A paginated list of tagged users.</returns>
        Task<object> GetChatMessageTaggedUsersAsync(long workspaceId, string messageId, string? cursor = null, int? limit = null);
        // Note: Return type should be a DTO with 'data' (IEnumerable<ChatUserDto>) and 'next_cursor'.
        #endregion
    }
}
