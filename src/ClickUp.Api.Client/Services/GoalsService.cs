using System;
using System.Collections.Generic; // For Dictionary
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Goals;
using ClickUp.Api.Client.Models.RequestModels.Goals;
using ClickUp.Api.Client.Models.ResponseModels.Goals;
using ClickUp.Api.Client.Models.ResponseModels; // For potential generic wrappers if needed

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IGoalsService"/> for ClickUp Goal operations.
    /// </summary>
    public class GoalsService : IGoalsService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoalsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public GoalsService(IApiConnection apiConnection)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        }

        private string BuildQueryString(Dictionary<string, string?> queryParams)
        {
            if (queryParams == null || !queryParams.Any(kvp => kvp.Value != null))
            {
                return string.Empty;
            }

            var sb = new StringBuilder("?");
            foreach (var kvp in queryParams)
            {
                if (kvp.Value != null)
                {
                    if (sb.Length > 1) sb.Append('&');
                    sb.Append($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
            }
            return sb.ToString();
        }

        /// <inheritdoc />
        public async Task<GetGoalsResponse> GetGoalsAsync(
            string workspaceId,
            bool? includeCompleted = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/goal"; // team_id is workspaceId
            var queryParams = new Dictionary<string, string?>();
            if (includeCompleted.HasValue) queryParams["include_completed"] = includeCompleted.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetGoalsResponse>(endpoint, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting goals for workspace {workspaceId}.");
            }
            return response;
        }

        /// <inheritdoc />
        public async Task<Goal> CreateGoalAsync(
            string workspaceId,
            CreateGoalRequest createGoalRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/goal";
            var responseWrapper = await _apiConnection.PostAsync<CreateGoalRequest, GetGoalResponse>(endpoint, createGoalRequest, cancellationToken);
            if (responseWrapper?.Goal == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty goal response when creating goal in workspace {workspaceId}.");
            }
            return responseWrapper.Goal;
        }

        /// <inheritdoc />
        public async Task<Goal> GetGoalAsync(
            string goalId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"goal/{goalId}";
            var responseWrapper = await _apiConnection.GetAsync<GetGoalResponse>(endpoint, cancellationToken);
            if (responseWrapper?.Goal == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty goal response when getting goal {goalId}.");
            }
            return responseWrapper.Goal;
        }

        /// <inheritdoc />
        public async Task<Goal> UpdateGoalAsync(
            string goalId,
            UpdateGoalRequest updateGoalRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"goal/{goalId}";
            var responseWrapper = await _apiConnection.PutAsync<UpdateGoalRequest, GetGoalResponse>(endpoint, updateGoalRequest, cancellationToken);
            if (responseWrapper?.Goal == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty goal response when updating goal {goalId}.");
            }
            return responseWrapper.Goal;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteGoalAsync(
            string goalId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"goal/{goalId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<KeyResult> CreateKeyResultAsync(
            string goalId,
            CreateKeyResultRequest createKeyResultRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"goal/{goalId}/key_result";
            var responseWrapper = await _apiConnection.PostAsync<CreateKeyResultRequest, CreateKeyResultResponse>(endpoint, createKeyResultRequest, cancellationToken);
            if (responseWrapper?.KeyResult == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty key result response when creating key result for goal {goalId}.");
            }
            return responseWrapper.KeyResult;
        }

        /// <inheritdoc />
        public async Task<KeyResult> EditKeyResultAsync(
            string keyResultId,
            EditKeyResultRequest editKeyResultRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"key_result/{keyResultId}";
            var responseWrapper = await _apiConnection.PutAsync<EditKeyResultRequest, EditKeyResultResponse>(endpoint, editKeyResultRequest, cancellationToken);
            if (responseWrapper?.KeyResult == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty key result response when editing key result {keyResultId}.");
            }
            return responseWrapper.KeyResult;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteKeyResultAsync(
            string keyResultId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"key_result/{keyResultId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }
    }
}
