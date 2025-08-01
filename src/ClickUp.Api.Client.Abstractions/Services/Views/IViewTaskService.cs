using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.Common.Pagination;

namespace ClickUp.Api.Client.Abstractions.Services.Views
{
    /// <summary>
    /// Service interface for ClickUp View task operations.
    /// Handles retrieving tasks within specific views.
    /// </summary>
    /// <remarks>
    /// This interface follows the Interface Segregation Principle by focusing solely on task operations within views.
    /// Covered API Endpoints:
    /// - View Tasks: `GET /v2/view/{view_id}/task`
    /// </remarks>
    public interface IViewTaskService
    {
        /// <summary>
        /// Retrieves tasks that are visible within a specific View, with pagination.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <param name="request">An object containing pagination parameters, specifically the page number.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IPagedResult{T}"/> of <see cref="CuTask"/> objects and pagination information.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="viewId"/> or <paramref name="request"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<IPagedResult<CuTask>> GetViewTasksAsync(
            string viewId,
            GetViewTasksRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all tasks that are visible within a specific View, automatically handling pagination.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>An asynchronous enumerable of <see cref="CuTask"/> objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="viewId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        IAsyncEnumerable<CuTask> GetViewTasksAsyncEnumerableAsync(
            string viewId,
            CancellationToken cancellationToken = default);
    }
}