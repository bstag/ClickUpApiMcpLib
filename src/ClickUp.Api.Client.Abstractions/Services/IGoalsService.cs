using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Goals; // Assuming Goal and KeyResult DTOs are here
using ClickUp.Api.Client.Models.RequestModels.Goals; // Assuming Request DTOs are here
using ClickUp.Api.Client.Models.ResponseModels.Goals; // For GetGoalsResponse

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Goals and Key Results operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/team/{team_id}/goal
    /// - POST /v2/team/{team_id}/goal
    /// - GET /v2/goal/{goal_id}
    /// - PUT /v2/goal/{goal_id}
    /// - DELETE /v2/goal/{goal_id}
    /// - POST /v2/goal/{goal_id}/key_result
    /// - PUT /v2/key_result/{key_result_id}
    /// - DELETE /v2/key_result/{key_result_id}
    /// </remarks>
    public interface IGoalsService
    {
        /// <summary>
        /// Retrieves Goals for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="includeCompleted">Optional. Whether to include completed Goals.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="GetGoalsResponse"/> object containing lists of goals and goal folders.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the workspace with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access goals in this workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<GetGoalsResponse> GetGoalsAsync(
            string workspaceId,
            bool? includeCompleted = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Goal in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (Team).</param>
        /// <param name="createGoalRequest">Details of the Goal to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="Goal"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createGoalRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the workspace with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create goals in this workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Goal> CreateGoalAsync(
            string workspaceId,
            CreateGoalRequest createGoalRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves details of a specific Goal.
        /// </summary>
        /// <param name="goalId">The UUID of the Goal.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="Goal"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="goalId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the goal with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this goal.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Goal> GetGoalAsync(
            string goalId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a Goal.
        /// </summary>
        /// <param name="goalId">The UUID of the Goal.</param>
        /// <param name="updateGoalRequest">Details for updating the Goal.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="Goal"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="goalId"/> or <paramref name="updateGoalRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the goal with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this goal.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Goal> UpdateGoalAsync(
            string goalId,
            UpdateGoalRequest updateGoalRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Goal.
        /// </summary>
        /// <param name="goalId">The UUID of the Goal to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="goalId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the goal with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this goal.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task DeleteGoalAsync(
            string goalId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a Target (Key Result) to a Goal.
        /// </summary>
        /// <param name="goalId">The UUID of the Goal.</param>
        /// <param name="createKeyResultRequest">Details of the Key Result to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="KeyResult"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="goalId"/> or <paramref name="createKeyResultRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the goal with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to add key results to this goal.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<KeyResult> CreateKeyResultAsync(
            string goalId,
            CreateKeyResultRequest createKeyResultRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a Target (Key Result).
        /// </summary>
        /// <param name="keyResultId">The UUID of the Key Result.</param>
        /// <param name="editKeyResultRequest">Details for editing the Key Result.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="KeyResult"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="keyResultId"/> or <paramref name="editKeyResultRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the key result with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to edit this key result.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<KeyResult> EditKeyResultAsync( // Method name was EditKeyResult in original
            string keyResultId,
            EditKeyResultRequest editKeyResultRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Target (Key Result) from a Goal.
        /// </summary>
        /// <param name="keyResultId">The UUID of the Key Result to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="keyResultId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the key result with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this key result.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task DeleteKeyResultAsync(
            string keyResultId,
            CancellationToken cancellationToken = default);
    }
}
