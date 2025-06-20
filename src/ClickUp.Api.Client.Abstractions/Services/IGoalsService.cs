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
        Task<KeyResult> CreateKeyResultAsync(
            string goalId,
            CreateKeyResultRequest createKeyResultRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a Target (Key Result).
        /// </summary>
        /// <param name="keyResultId">The UUID of the Key Result.</param>
        /// <param name="updateKeyResultRequest">Details for editing the Key Result.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="KeyResult"/>.</returns>
        Task<KeyResult> EditKeyResultAsync( // Method name was EditKeyResult in original
            string keyResultId,
            UpdateKeyResultRequest updateKeyResultRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Target (Key Result) from a Goal.
        /// </summary>
        /// <param name="keyResultId">The UUID of the Key Result to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task DeleteKeyResultAsync(
            string keyResultId,
            CancellationToken cancellationToken = default);
    }
}
