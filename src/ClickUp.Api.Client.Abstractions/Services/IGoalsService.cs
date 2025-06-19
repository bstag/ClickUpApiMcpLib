using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Goals and Key Results operations in the ClickUp API
    // Based on endpoints like:
    // - GET /v2/team/{team_id}/goal
    // - POST /v2/team/{team_id}/goal
    // - GET /v2/goal/{goal_id}
    // - PUT /v2/goal/{goal_id}
    // - DELETE /v2/goal/{goal_id}
    // - POST /v2/goal/{goal_id}/key_result
    // - PUT /v2/key_result/{key_result_id}
    // - DELETE /v2/key_result/{key_result_id}

    public interface IGoalsService
    {
        /// <summary>
        /// Retrieves Goals for a specific Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="includeCompleted">Optional. Whether to include completed Goals.</param>
        /// <returns>A list of Goals for the Workspace.</returns>
        Task<IEnumerable<object>> GetGoalsAsync(double workspaceId, bool? includeCompleted = null);
        // Note: Return type should be a DTO that includes both 'goals' and 'folders' arrays from the response (e.g., GetGoalsResponseDto).

        /// <summary>
        /// Creates a new Goal in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace (team_id).</param>
        /// <param name="createGoalRequest">Details of the Goal to create.</param>
        /// <returns>The created Goal.</returns>
        Task<object> CreateGoalAsync(double workspaceId, object createGoalRequest);
        // Note: createGoalRequest should be CreateGoalRequest, return type should be GoalDto (wrapped in a 'goal' property in response).

        /// <summary>
        /// Retrieves details of a specific Goal.
        /// </summary>
        /// <param name="goalId">The UUID of the Goal.</param>
        /// <returns>Details of the Goal.</returns>
        Task<object> GetGoalAsync(string goalId);
        // Note: Return type should be GoalDto (wrapped in a 'goal' property in response).

        /// <summary>
        /// Updates a Goal.
        /// </summary>
        /// <param name="goalId">The UUID of the Goal.</param>
        /// <param name="updateGoalRequest">Details for updating the Goal.</param>
        /// <returns>The updated Goal.</returns>
        Task<object> UpdateGoalAsync(string goalId, object updateGoalRequest);
        // Note: updateGoalRequest should be UpdateGoalRequest, return type should be GoalDto (wrapped in a 'goal' property in response).

        /// <summary>
        /// Deletes a Goal.
        /// </summary>
        /// <param name="goalId">The UUID of the Goal to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteGoalAsync(string goalId);
        // Note: API returns 200 with an empty object.

        /// <summary>
        /// Adds a Target (Key Result) to a Goal.
        /// </summary>
        /// <param name="goalId">The UUID of the Goal.</param>
        /// <param name="createKeyResultRequest">Details of the Key Result to create.</param>
        /// <returns>The created Key Result.</returns>
        Task<object> CreateKeyResultAsync(string goalId, object createKeyResultRequest);
        // Note: createKeyResultRequest should be CreateKeyResultRequest, return type should be KeyResultDto (wrapped in a 'key_result' property in response).

        /// <summary>
        /// Updates a Target (Key Result).
        /// </summary>
        /// <param name="keyResultId">The UUID of the Key Result.</param>
        /// <param name="editKeyResultRequest">Details for editing the Key Result.</param>
        /// <returns>The updated Key Result.</returns>
        Task<object> EditKeyResultAsync(string keyResultId, object editKeyResultRequest);
        // Note: editKeyResultRequest should be EditKeyResultRequest, return type should be KeyResultDto (wrapped in a 'key_result' property in response).

        /// <summary>
        /// Deletes a Target (Key Result) from a Goal.
        /// </summary>
        /// <param name="keyResultId">The UUID of the Key Result to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteKeyResultAsync(string keyResultId);
        // Note: API returns 200 with an empty object.
    }
}
