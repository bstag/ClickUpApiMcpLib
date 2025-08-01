using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services.Tasks
{
    /// <summary>
    /// Handles task relationship operations such as merging tasks.
    /// Implements the Single Responsibility Principle by focusing only on task relationship management.
    /// </summary>
    public class TaskRelationshipService : ITaskRelationshipService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<TaskRelationshipService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskRelationshipService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public TaskRelationshipService(IApiConnection apiConnection, ILogger<TaskRelationshipService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<TaskRelationshipService>.Instance;
        }

        /// <inheritdoc />
        public async Task<CuTask> MergeTasksAsync(
            string targetTaskId,
            MergeTasksRequest mergeTasksRequest,
            CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(targetTaskId, nameof(targetTaskId));
            if (mergeTasksRequest == null)
            {
                throw new ArgumentNullException(nameof(mergeTasksRequest));
            }
            if (mergeTasksRequest.SourceTaskIds == null || !mergeTasksRequest.SourceTaskIds.Any())
            {
                throw new ArgumentException("MergeTasksRequest must contain at least one source task ID.", nameof(mergeTasksRequest.SourceTaskIds));
            }

            _logger.LogInformation("Merging {SourceTaskCount} tasks into target task ID: {TargetTaskId}", 
                mergeTasksRequest.SourceTaskIds.Count(), targetTaskId);

            CuTask? lastMergedTargetTask = null;

            // Iterate through each source task and merge it into the target task.
            // The ClickUp API endpoint for merging is POST /task/{source_task_id}/merge
            // with a body like { "target_task_id": "the_actual_target_task_id" }.
            // The response is the updated target task.
            // API documentation for this endpoint states "Custom Task IDs are not supported".
            foreach (var sourceTaskId in mergeTasksRequest.SourceTaskIds)
            {
                if (string.IsNullOrWhiteSpace(sourceTaskId))
                {
                    _logger.LogWarning("Skipping empty or whitespace source task ID during merge operation into target {TargetTaskId}.", targetTaskId);
                    continue;
                }

                var endpoint = UrlBuilderHelper.CreateBuilder()
                    .WithPathSegments("task", sourceTaskId, "merge", targetTaskId)
                    .Build();

                var payload = new { target_task_id = targetTaskId };

                _logger.LogDebug("Attempting to merge source task {SourceTaskId} into target task {TargetTaskId}", 
                    sourceTaskId, targetTaskId);
                    
                var updatedTargetTask = await _apiConnection.PostAsync<object, CuTask>(endpoint, payload, cancellationToken);

                if (updatedTargetTask == null)
                {
                    // If any merge operation fails to return the task, consider it an issue.
                    _logger.LogError("API connection returned null response when merging source task '{SourceTaskId}' into target task '{TargetTaskId}'.", 
                        sourceTaskId, targetTaskId);
                    throw new InvalidOperationException($"API connection returned null response when merging source task '{sourceTaskId}' into target task '{targetTaskId}'.");
                }
                
                lastMergedTargetTask = updatedTargetTask; // Keep track of the latest state of the target task.
                _logger.LogInformation("Successfully merged source task {SourceTaskId} into target task {TargetTaskId}. Updated target task ID: {UpdatedTargetTaskId}", 
                    sourceTaskId, targetTaskId, updatedTargetTask.Id);
            }

            if (lastMergedTargetTask == null)
            {
                // This would happen if SourceTaskIds was empty or contained only invalid (empty/whitespace) IDs,
                // and no merge operations were attempted or all skipped.
                _logger.LogWarning("No merge operations were successfully performed for target task '{TargetTaskId}'. This could be due to an empty or invalid list of source task IDs.", targetTaskId);
                // Depending on strictness, we might throw or return a fetched version of targetTaskId.
                // For now, if no merge actually happened and returned a task, it's an issue.
                throw new InvalidOperationException($"No merge operations were successfully performed for target task '{targetTaskId}', or the target task could not be retrieved after attempting merges. Ensure SourceTaskIds are valid.");
            }

            _logger.LogInformation("Successfully completed merging {SourceTaskCount} tasks into target task {TargetTaskId}", 
                mergeTasksRequest.SourceTaskIds.Count(), targetTaskId);
            return lastMergedTargetTask;
        }
    }
}