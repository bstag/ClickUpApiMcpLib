﻿using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.ResponseModels.Goals;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Goals and Key Results (Targets) operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for managing Goals and their associated Key Results (Targets) within a Workspace.
    /// It allows for creating, retrieving, updating, and deleting Goals and Key Results.
    /// Covered API Endpoints:
    /// - Goals: `GET /team/{team_id}/goal`, `POST /team/{team_id}/goal`, `GET /goal/{goal_id}`, `PUT /goal/{goal_id}`, `DELETE /goal/{goal_id}`
    /// - Key Results: `POST /goal/{goal_id}/key_result`, `PUT /key_result/{key_result_id}`, `DELETE /key_result/{key_result_id}`
    /// </remarks>
    public interface IGoalsService
    {
        /// <summary>
        /// Retrieves all Goals for a specific Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) from which to retrieve Goals.</param>
        /// <param name="includeCompleted">Optional. If set to <c>true</c>, includes completed Goals in the results. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetGoalsResponse"/> object, which includes lists of <see cref="Goal"/> objects and related goal folders.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Goals in this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<GetGoalsResponse> GetGoalsAsync(
            string workspaceId,
            bool? includeCompleted = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Goal within a specified Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team) where the new Goal will be created.</param>
        /// <param name="createGoalRequest">An object containing the details for the new Goal, such as its name, description, due date, and owners.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Goal"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createGoalRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Workspace with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create Goals in this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Goal> CreateGoalAsync(
            string workspaceId,
            CreateGoalRequest createGoalRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details of a specific Goal by its ID.
        /// </summary>
        /// <param name="goalId">The unique identifier (UUID) of the Goal to retrieve.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the details of the requested <see cref="Goal"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="goalId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Goal with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this Goal.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Goal> GetGoalAsync(
            string goalId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the properties of an existing Goal.
        /// </summary>
        /// <param name="goalId">The unique identifier (UUID) of the Goal to update.</param>
        /// <param name="updateGoalRequest">An object containing the properties to update for the Goal.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Goal"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="goalId"/> or <paramref name="updateGoalRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Goal with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this Goal.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Goal> UpdateGoalAsync(
            string goalId,
            UpdateGoalRequest updateGoalRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specified Goal.
        /// </summary>
        /// <param name="goalId">The unique identifier (UUID) of the Goal to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="goalId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Goal with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this Goal.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteGoalAsync(
            string goalId,
            CancellationToken cancellationToken = default);

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
