using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;

namespace ClickUp.Api.Client.Abstractions.Services.Goals
{
    /// <summary>
    /// Interface for managing ClickUp Key Result (Target) operations.
    /// Provides methods for creating, updating, and deleting Key Results associated with Goals.
    /// </summary>
    /// <remarks>
    /// This interface focuses specifically on Key Result operations and follows the Interface Segregation Principle.
    /// Key Results are targets or metrics that measure progress toward achieving a Goal.
    /// Covered API Endpoints:
    /// - Key Results: `POST /goal/{goal_id}/key_result`, `PUT /key_result/{key_result_id}`, `DELETE /key_result/{key_result_id}`
    /// </remarks>
    public interface IKeyResultManager
    {
        /// <summary>
        /// Adds a new Target (Key Result) to an existing Goal.
        /// </summary>
        /// <param name="goalId">The unique identifier (UUID) of the Goal to which the Key Result will be added.</param>
        /// <param name="createKeyResultRequest">An object containing the details for the new Key Result, such as its name, type, and target value.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="KeyResult"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="goalId"/> or <paramref name="createKeyResultRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Goal with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to add Key Results to this Goal.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<KeyResult> CreateKeyResultAsync(
            string goalId,
            CreateKeyResultRequest createKeyResultRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing Target (Key Result).
        /// </summary>
        /// <param name="keyResultId">The unique identifier (UUID) of the Key Result to update.</param>
        /// <param name="editKeyResultRequest">An object containing the properties to update for the Key Result.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="KeyResult"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="keyResultId"/> or <paramref name="editKeyResultRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Key Result with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to edit this Key Result.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<KeyResult> EditKeyResultAsync(
            string keyResultId,
            EditKeyResultRequest editKeyResultRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specified Target (Key Result) from a Goal.
        /// </summary>
        /// <param name="keyResultId">The unique identifier (UUID) of the Key Result to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="keyResultId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Key Result with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this Key Result.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteKeyResultAsync(
            string keyResultId,
            CancellationToken cancellationToken = default);
    }
}