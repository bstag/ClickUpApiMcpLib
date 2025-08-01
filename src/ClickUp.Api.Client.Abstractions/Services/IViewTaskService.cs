using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Views;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines the contract for view task operations.
    /// Implements the Single Responsibility Principle by focusing only on retrieving tasks from views.
    /// </summary>
    public interface IViewTaskService
    {
        /// <summary>
        /// Retrieves tasks from a specific view with pagination support.
        /// </summary>
        /// <param name="viewId">The ID of the view.</param>
        /// <param name="request">The request containing pagination parameters.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a paged result of tasks.</returns>
        Task<IPagedResult<CuTask>> GetViewTasksAsync(
            string viewId,
            GetViewTasksRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all tasks that are visible within a specific view, automatically handling pagination.
        /// </summary>
        /// <param name="viewId">The ID of the view.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>An asynchronous enumerable of tasks.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="viewId"/> is null or empty.</exception>
        IAsyncEnumerable<CuTask> GetViewTasksAsyncEnumerableAsync(
            string viewId,
            CancellationToken cancellationToken = default);
    }
}