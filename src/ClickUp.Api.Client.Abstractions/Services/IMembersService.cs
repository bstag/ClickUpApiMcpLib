using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Members;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Members operations.
    /// </summary>
    /// <remarks>
    /// This service focuses on retrieving members associated with specific items like Tasks and Lists.
    /// It provides insights into who has explicit access to these entities.
    /// Covered API Endpoints:
    /// - `GET /task/{task_id}/member`: Retrieves members of a specific Task.
    /// - `GET /list/{list_id}/member`: Retrieves members of a specific List.
    /// </remarks>
    public interface IMembersService
    {
        /// <summary>
        /// Retrieves a list of members who have explicit access to a specific Task.
        /// This does not include users who have access through inherited Hierarchy permissions unless they are also explicitly added to the task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the Task for which to retrieve members.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Member"/> objects associated with the Task.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Task with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access members for this Task.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<IEnumerable<Member>> GetTaskMembersAsync(
            string taskId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of Workspace members who have explicit access to a specific List.
        /// </summary>
        /// <param name="listId">The unique identifier of the List for which to retrieve members.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Member"/> objects associated with the List.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access members for this List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<Member>> GetListMembersAsync(
            string listId,
            CancellationToken cancellationToken = default);
    }
}
