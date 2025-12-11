using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;

namespace ClickUp.Api.Client.Abstractions.Services.Views
{
    /// <summary>
    /// Service interface for ClickUp View management operations.
    /// Handles CRUD operations on individual views.
    /// </summary>
    /// <remarks>
    /// This interface follows the Interface Segregation Principle by focusing solely on view management operations
    /// (Create, Read, Update, Delete) for individual views.
    /// Covered API Endpoints:
    /// - View Details: `GET /v2/view/{view_id}`
    /// - Update View: `PUT /v2/view/{view_id}`
    /// - Delete View: `DELETE /v2/view/{view_id}`
    /// </remarks>
    public interface IViewManagementService
    {
        /// <summary>
        /// Retrieves details of a specific View by its ID.
        /// </summary>
        /// <param name="viewId">The ID of the View.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetViewResponse"/> object with the view's details.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="viewId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors, such as view not found.</exception>
        Task<GetViewResponse> GetViewAsync(
            string viewId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing View.
        /// </summary>
        /// <param name="viewId">The ID of the View to update.</param>
        /// <param name="updateViewRequest">An <see cref="UpdateViewRequest"/> object containing the updated details for the view.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="UpdateViewResponse"/> object with the updated view's details.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="viewId"/> or <paramref name="updateViewRequest"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        Task<UpdateViewResponse> UpdateViewAsync(
            string viewId,
            UpdateViewRequest updateViewRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a View.
        /// </summary>
        /// <param name="viewId">The ID of the View to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="viewId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown for API-side errors.</exception>
        System.Threading.Tasks.Task DeleteViewAsync(
            string viewId,
            CancellationToken cancellationToken = default);
    }
}