using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Templates;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Templates operations.
    /// </summary>
    /// <remarks>
    /// This service focuses on retrieving Task Templates available within a Workspace.
    /// Other template types (e.g., List templates, Folder templates) are typically handled by their respective services.
    /// Covered API Endpoints:
    /// - `GET /team/{team_id}/taskTemplate`: Retrieves Task Templates for a Workspace.
    /// </remarks>
    public interface ITemplatesService
    {
        /// <summary>
        /// Retrieves a paginated list of Task Templates available in a specific Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) from which to retrieve Task Templates.</param>
        /// <param name="page">The page number of results to retrieve (0-indexed). This parameter is required by the ClickUp API for this endpoint.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="GetTaskTemplatesResponse"/> object,
        /// which includes a list of task templates available in the Workspace for the specified page.
        /// </returns>
        /// <remarks>
        /// The ClickUp API for this endpoint requires pagination and returns templates within a structured response.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="page"/> is less than 0.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Task Templates for this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<GetTaskTemplatesResponse> GetTaskTemplatesAsync(
            string workspaceId,
            int page,
            CancellationToken cancellationToken = default);
    }
}
