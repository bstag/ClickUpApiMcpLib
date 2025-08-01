using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Views;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines the contract for basic CRUD operations on ClickUp Views.
    /// Implements the Single Responsibility Principle by focusing only on view creation, reading, updating, and deletion.
    /// </summary>
    public interface IViewCrudService
    {
        /// <summary>
        /// Retrieves a specific view by its ID.
        /// </summary>
        /// <param name="viewId">The ID of the view to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the view response.</returns>
        Task<GetViewResponse> GetViewAsync(
            string viewId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing view.
        /// </summary>
        /// <param name="viewId">The ID of the view to update.</param>
        /// <param name="updateViewRequest">The request containing the updated view data.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated view response.</returns>
        Task<UpdateViewResponse> UpdateViewAsync(
            string viewId,
            UpdateViewRequest updateViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a view.
        /// </summary>
        /// <param name="viewId">The ID of the view to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        System.Threading.Tasks.Task DeleteViewAsync(
            string viewId,
            CancellationToken cancellationToken = default);
    }
}