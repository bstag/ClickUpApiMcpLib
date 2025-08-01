using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using ClickUp.Api.Client.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services.Tasks
{
    /// <summary>
    /// Handles time tracking and time-related operations for ClickUp tasks.
    /// Implements the Single Responsibility Principle by focusing only on task time tracking functionality.
    /// </summary>
    public class TaskTimeTrackingService : ITaskTimeTrackingService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<TaskTimeTrackingService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskTimeTrackingService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public TaskTimeTrackingService(IApiConnection apiConnection, ILogger<TaskTimeTrackingService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<TaskTimeTrackingService>.Instance;
        }

        /// <inheritdoc />
        public async Task<TaskTimeInStatusResponse> GetTaskTimeInStatusAsync(
            string taskId,
            GetTaskTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(taskId, nameof(taskId));
            if (requestModel == null) throw new ArgumentNullException(nameof(requestModel));
            if (!string.IsNullOrEmpty(requestModel.TeamId)) ValidationHelper.ValidateId(requestModel.TeamId, nameof(requestModel.TeamId));

            _logger.LogInformation("Getting task time in status for task ID: {TaskId}", taskId);
            
            var endpoint = UrlBuilderHelper.CreateBuilder()
                .WithPathSegments("task", taskId, "time_in_status")
                .WithQueryParameter("custom_task_ids", requestModel.CustomTaskIds?.ToString().ToLower())
                .WithQueryParameter("team_id", requestModel.TeamId)
                .Build();

            var response = await _apiConnection.GetAsync<TaskTimeInStatusResponse>(endpoint, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting task time in status for task {taskId}.");
            }
            
            _logger.LogDebug("Successfully retrieved time in status for task {TaskId}", taskId);
            return response;
        }

        /// <inheritdoc />
        public async Task<GetBulkTasksTimeInStatusResponse> GetBulkTasksTimeInStatusAsync(
            GetBulkTasksTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            if (requestModel == null) throw new ArgumentNullException(nameof(requestModel));
            if (requestModel.TaskIds == null || !requestModel.TaskIds.Any())
            {
                throw new ArgumentException("Task IDs collection cannot be null or empty.", nameof(requestModel.TaskIds));
            }
            if (!string.IsNullOrEmpty(requestModel.TeamId)) ValidationHelper.ValidateId(requestModel.TeamId, nameof(requestModel.TeamId));

            _logger.LogInformation("Getting bulk tasks time in status for {TaskIdsCount} task IDs", requestModel.TaskIds.Count());
            
            // The API for this endpoint expects task_ids as a comma-separated string.
            var taskIdsString = string.Join(",", requestModel.TaskIds);
            
            var endpoint = UrlBuilderHelper.CreateBuilder()
                .WithPathSegments("task", "bulk_time_in_status", "task_ids")
                .WithQueryParameter("task_ids", taskIdsString)
                .WithQueryParameter("custom_task_ids", requestModel.CustomTaskIds?.ToString().ToLower())
                .WithQueryParameter("team_id", requestModel.TeamId)
                .Build();

            var response = await _apiConnection.GetAsync<GetBulkTasksTimeInStatusResponse>(endpoint, cancellationToken);
            if (response == null)
            {
                throw new InvalidOperationException("API connection returned null response when getting bulk tasks time in status.");
            }
            
            _logger.LogDebug("Successfully retrieved bulk time in status for {TaskIdsCount} tasks", requestModel.TaskIds.Count());
            return response;
        }
    }
}