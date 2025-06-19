using System.Threading;
using System.Threading.Tasks;
// Assuming a placeholder for request/response models for now
// using ClickUp.Net.Models;

namespace ClickUp.Net.Abstractions.Services
{
    /// <summary>
    /// Interface for services interacting with ClickUp Authorization.
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Gets an access token.
        /// </summary>
        /// <param name="clientId">OAuth app client ID.</param>
        /// <param name="clientSecret">OAuth app client secret.</param>
        /// <param name="code">Code from redirect URL.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the access token response.</returns>
        Task<object> GetAccessTokenAsync(string clientId, string clientSecret, string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the authorized user's details.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the authorized user response.</returns>
        Task<object> GetAuthorizedUserAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the authorized user's Workspaces (Teams).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A placeholder for the authorized teams response.</returns>
        Task<object> GetAuthorizedTeamsAsync(CancellationToken cancellationToken = default);
    }
}
