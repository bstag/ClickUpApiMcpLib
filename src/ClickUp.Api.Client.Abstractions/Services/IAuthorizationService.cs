using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ClickUp.Api.Client.Models.Entities.Users;
using ClickUp.Api.Client.Models.Entities.WorkSpaces;
using ClickUp.Api.Client.Models.ResponseModels.Authorization;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp authorization operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for OAuth 2.0 authentication flows
    /// and for retrieving information about the currently authenticated user and their workspaces (teams).
    /// Endpoints covered:
    /// - `POST /oauth/token`: Exchanges an authorization code for an access token.
    /// - `GET /user`: Retrieves the authenticated user's details.
    /// - `GET /team`: Retrieves the workspaces (teams) accessible to the authenticated user.
    /// </remarks>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Exchanges an authorization code obtained from the ClickUp OAuth 2.0 flow for an access token.
        /// </summary>
        /// <param name="clientId">The client ID of your ClickUp OAuth application.</param>
        /// <param name="clientSecret">The client secret of your ClickUp OAuth application.</param>
        /// <param name="code">The authorization code received from ClickUp after user authorization.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetAccessTokenResponse"/> object, which includes the access token and related information.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="clientId"/>, <paramref name="clientSecret"/>, or <paramref name="code"/> is null or empty/whitespace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the API call fails due to invalid OAuth credentials or an invalid/expired authorization code.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures. Common derived types include <see cref="Models.Exceptions.ClickUpApiRateLimitException"/> or <see cref="Models.Exceptions.ClickUpApiRequestException"/>.</exception>
        Task<GetAccessTokenResponse> GetAccessTokenAsync(
            string clientId,
            string clientSecret,
            string code,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details of the user currently authenticated via Personal Access Token or OAuth access token.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="User"/> object with the authenticated user's details.</returns>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the current session is not authenticated (e.g., missing or invalid API token).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons, such as rate limiting. See inner exception and properties for details.</exception>
        Task<User> GetAuthorizedUserAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of Workspaces (formerly known as Teams) that the currently authenticated user has access to.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="ClickUpWorkspace"/> objects.</returns>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the current session is not authenticated (e.g., missing or invalid API token).</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons, such as rate limiting. See inner exception and properties for details.</exception>
        Task<IEnumerable<ClickUpWorkspace>> GetAuthorizedWorkspacesAsync(CancellationToken cancellationToken = default);
    }
}
