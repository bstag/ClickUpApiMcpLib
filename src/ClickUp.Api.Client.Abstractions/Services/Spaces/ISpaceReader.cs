using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Spaces;

namespace ClickUp.Api.Client.Abstractions.Services.Spaces
{
    /// <summary>
    /// Interface for reading ClickUp Space operations.
    /// This interface follows the Interface Segregation Principle by focusing solely on read operations.
    /// </summary>
    public interface ISpaceReader
    {
        /// <summary>
        /// Retrieves all Spaces available within a specific Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) from which to retrieve Spaces.</param>
        /// <param name="archived">Optional. If set to <c>true</c>, includes archived Spaces in the results. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Space"/> objects found in the Workspace.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Spaces in this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<IEnumerable<Space>> GetSpacesAsync(
            string workspaceId,
            bool? archived = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details of a specific Space by its ID.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space to retrieve.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the details of the requested <see cref="Space"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Space> GetSpaceAsync(
            string spaceId,
            CancellationToken cancellationToken = default);
    }
}