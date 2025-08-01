using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.Parameters;
using ClickUp.Api.Client.Models.Common.Pagination;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines the contract for complex queries, filtering, and pagination operations on ClickUp tasks.
    /// Follows the Single Responsibility Principle by focusing only on task querying and filtering operations.
    /// </summary>
    public interface ITaskQueryService
    {
        /// <summary>
        /// Gets tasks from a list with optional filtering and pagination.
        /// </summary>
        /// <param name="listId">The ID of the list to get tasks from.</param>
        /// <param name="configureParameters">Optional action to configure query parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A paged result containing the tasks.</returns>
        Task<IPagedResult<CuTask>> GetTasksAsync(
            string listId,
            Action<GetTasksRequestParameters>? configureParameters = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets tasks from a list as an async enumerable, automatically handling pagination.
        /// </summary>
        /// <param name="listId">The ID of the list to get tasks from.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An async enumerable of tasks.</returns>
        IAsyncEnumerable<CuTask> GetTasksAsyncEnumerableAsync(
            string listId,
            GetTasksRequestParameters parameters,
            [EnumeratorCancellation] CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets filtered tasks from a team/workspace with optional filtering and pagination.
        /// </summary>
        /// <param name="workspaceId">The ID of the workspace to get tasks from.</param>
        /// <param name="configureParameters">Optional action to configure query parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A paged result containing the filtered tasks.</returns>
        Task<IPagedResult<CuTask>> GetFilteredTeamTasksAsync(
            string workspaceId,
            Action<GetTasksRequestParameters>? configureParameters = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets filtered tasks from a team/workspace as an async enumerable, automatically handling pagination.
        /// </summary>
        /// <param name="workspaceId">The ID of the workspace to get tasks from.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An async enumerable of filtered tasks.</returns>
        IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(
            string workspaceId,
            GetTasksRequestParameters parameters,
            [EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}