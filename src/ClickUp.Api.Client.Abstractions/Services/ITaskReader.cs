using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Parameters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines operations for reading and retrieving task information.
    /// </summary>
    public interface ITaskReader
    {
        /// <summary>
        /// Retrieves Tasks from a specific List with optional filtering and pagination.
        /// </summary>
        /// <param name="listId">The unique identifier of the List containing the Tasks.</param>
        /// <param name="configureParameters">An optional action to configure the <see cref="GetTasksRequestParameters"/> for filtering, sorting, and pagination.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IPagedResult{CuTask}"/> with the list of Tasks and pagination details.</returns>
        Task<IPagedResult<CuTask>> GetTasksAsync(
            string listId,
            System.Action<GetTasksRequestParameters>? configureParameters = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details of a specific Task by its ID.
        /// </summary>
        /// <param name="taskId">The unique identifier of the Task to retrieve.</param>
        /// <param name="requestModel">An object containing options such as custom task ID handling, inclusion of subtasks, Markdown description, and comment pagination.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the details of the requested <see cref="CuTask"/>.</returns>
        Task<CuTask> GetTaskAsync(
            string taskId,
            GetTaskRequest requestModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves Tasks from a Workspace (Team) based on a comprehensive set of filters.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="configureParameters">An action to configure the <see cref="GetTasksRequestParameters"/> for filtering, sorting, and pagination.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IPagedResult{CuTask}"/> with the list of matching Tasks and pagination details.</returns>
        Task<IPagedResult<CuTask>> GetFilteredTeamTasksAsync(
            string workspaceId,
            System.Action<GetTasksRequestParameters>? configureParameters = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all Tasks within a specific List, automatically handling pagination using <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="listId">The unique identifier of the List.</param>
        /// <param name="requestModel">An object containing various filtering and sorting options for retrieving Tasks from the list.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>An asynchronous stream of <see cref="CuTask"/> objects from the specified List.</returns>
        IAsyncEnumerable<CuTask> GetTasksAsyncEnumerableAsync(
            string listId,
            GetTasksRequestParameters requestModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all Tasks from a Workspace (Team) based on a comprehensive set of filters, automatically handling pagination using <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="requestModel">An object containing various filtering and sorting options for retrieving Tasks.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>An asynchronous stream of <see cref="CuTask"/> objects from the specified Workspace matching the filters.</returns>
        IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(
            string workspaceId,
            GetTasksRequestParameters requestModel,
            CancellationToken cancellationToken = default);
    }
}