using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Spaces;
using ClickUp.Api.Client.Models.RequestModels.Spaces;

namespace ClickUp.Api.Client.Abstractions.Services.Spaces
{
    /// <summary>
    /// Interface for writing ClickUp Space operations (Create, Update, Delete).
    /// This interface follows the Interface Segregation Principle by focusing solely on write operations.
    /// </summary>
    public interface ISpaceWriter
    {
        /// <summary>
        /// Creates a new Space within a specified Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) where the new Space will be created.</param>
        /// <param name="createSpaceRequest">An object containing the details for the new Space, such as its name and enabled features.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Space"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createSpaceRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create Spaces in this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Space> CreateSpaceAsync(
            string workspaceId,
            CreateSpaceRequest createSpaceRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the properties of an existing Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space to update.</param>
        /// <param name="updateSpaceRequest">An object containing the properties to update for the Space.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Space"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="updateSpaceRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Space> UpdateSpaceAsync(
            string spaceId,
            UpdateSpaceRequest updateSpaceRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specified Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteSpaceAsync(
            string spaceId,
            CancellationToken cancellationToken = default);
    }
}