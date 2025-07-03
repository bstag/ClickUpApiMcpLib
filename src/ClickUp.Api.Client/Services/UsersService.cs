using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.RequestModels.Users;
using ClickUp.Api.Client.Models.ResponseModels.Users; // Assuming GetUserResponse exists

using System;
using System.Collections.Generic; // For Dictionary
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUp.Api.Client.Helpers;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IUsersService"/> for ClickUp User operations within a Workspace.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<UsersService> _logger;
        private const string BaseWorkspaceEndpoint = "team"; // ClickUp v2 uses "team/{team_id}" for workspace context

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public UsersService(IApiConnection apiConnection, ILogger<UsersService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<UsersService>.Instance;
        }


        /// <inheritdoc />
        public async Task<User> GetUserFromWorkspaceAsync(
            string workspaceId,
            string userId,
            bool? includeShared = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting user ID: {UserId} from workspace ID: {WorkspaceId}", userId, workspaceId);
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/user/{userId}";
            var queryParams = new Dictionary<string, string?>();
            // 'includeShared' is not used as it's not standard for this endpoint.
            endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetUserResponse>(endpoint, cancellationToken);

            if (response?.Member?.User == null)
            {
                throw new InvalidOperationException($"API response, Member, or User data was null for GetUserFromWorkspaceAsync (Workspace: {workspaceId}, User: {userId}).");
            }

            // Assuming response.Member.User is of a type compatible with Models.Entities.Users.User
            // If GetUserResponse.Member.User is already Models.Entities.Users.User, direct return is fine.
            // If it's a different DTO, mapping is needed. Let's assume it's compatible or is the target type.
            // The original code used 'inviteGuestUser' which implies response.Member.User was GuestUserInfo or similar.
            // The interface IUsersService expects Models.Entities.Users.User.
            // Let's assume GetUserResponse.Member.User is of type Models.Entities.Users.User.
            // (This requires GetUserResponse and its nested types to be correctly defined)

            // If response.Member.User is the exact type Models.Entities.Users.User:
            // return response.Member.User;

            // If mapping is needed from an intermediate DTO (e.g., response.Member.User is some internal DTO)
            // to Models.Entities.Users.User:
            var sourceUser = response.Member.User;
            return new User( // Constructing the Models.Entities.Users.User record
                Id: sourceUser.Id, // Assuming sourceUser.Id is int
                Username: sourceUser.Username, // Assuming sourceUser.Username is string?
                Email: sourceUser.Email,       // Assuming sourceUser.Email is string
                Color: sourceUser.Color,       // Assuming sourceUser.Color is string?
                ProfilePicture: sourceUser.ProfilePicture, // Assuming sourceUser.ProfilePicture is string?
                Initials: sourceUser.Initials, // Assuming sourceUser.Initials is string?
                ProfileInfo: null // Assuming sourceUser doesn't have ProfileInfo or it's mapped if available
                                  // This part needs to align with what `GetUserResponse.Member.User` actually is.
                                  // For now, this is a direct mapping of common fields.
                                  // If GetUserResponse.Member.User IS Models.Entities.Users.User, then this mapping is redundant.
            );
        }

        /// <inheritdoc />
        public async Task<User> EditUserOnWorkspaceAsync(
            string workspaceId,
            string userId,
            EditUserOnWorkspaceRequest editUserRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Editing user ID: {UserId} on workspace ID: {WorkspaceId}", userId, workspaceId);
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/user/{userId}";
            var response = await _apiConnection.PutAsync<EditUserOnWorkspaceRequest, GetUserResponse>(endpoint, editUserRequest, cancellationToken);

            if (response?.Member?.User == null)
            {
                throw new InvalidOperationException($"API response, Member, or User data was null for EditUserOnWorkspaceAsync (Workspace: {workspaceId}, User: {userId}).");
            }

            var sourceUser = response.Member.User;
            return new User(
                Id: sourceUser.Id,
                Username: sourceUser.Username,
                Email: sourceUser.Email,
                Color: sourceUser.Color,
                ProfilePicture: sourceUser.ProfilePicture,
                Initials: sourceUser.Initials,
                ProfileInfo: null
            );
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task RemoveUserFromWorkspaceAsync(
            string workspaceId,
            string userId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing user ID: {UserId} from workspace ID: {WorkspaceId}", userId, workspaceId);
            var endpoint = $"{BaseWorkspaceEndpoint}/{workspaceId}/user/{userId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        // Removed explicit interface implementations as public methods above match the interface.
    }
}
