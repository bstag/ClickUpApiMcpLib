using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Parameters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Composite implementation of <see cref="ITasksService"/> that delegates to specialized task services.
    /// This maintains backward compatibility while implementing the Single Responsibility Principle
    /// through service composition.
    /// </summary>
    public class TaskService : ITasksService
    {
        private readonly ITaskCrudService _crudService;
        private readonly ITaskQueryService _queryService;
        private readonly ITaskRelationshipService _relationshipService;
        private readonly ITaskTimeTrackingService _timeTrackingService;
        private readonly ILogger<TaskService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskService"/> class.
        /// </summary>
        /// <param name="crudService">The CRUD operations service.</param>
        /// <param name="queryService">The query and filtering service.</param>
        /// <param name="relationshipService">The task relationship service.</param>
        /// <param name="timeTrackingService">The time tracking service.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if any required service is null.</exception>
        public TaskService(
            ITaskCrudService crudService,
            ITaskQueryService queryService,
            ITaskRelationshipService relationshipService,
            ITaskTimeTrackingService timeTrackingService,
            ILogger<TaskService> logger)
        {
            _crudService = crudService ?? throw new ArgumentNullException(nameof(crudService));
            _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
            _relationshipService = relationshipService ?? throw new ArgumentNullException(nameof(relationshipService));
            _timeTrackingService = timeTrackingService ?? throw new ArgumentNullException(nameof(timeTrackingService));
            _logger = logger ?? NullLogger<TaskService>.Instance;
        }

        #region CRUD Operations - Delegated to TaskCrudService

        /// <inheritdoc />
        public async Task<CuTask> CreateTaskAsync(
            string listId,
            CreateTaskRequest createTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating CreateTaskAsync to TaskCrudService");
            return await _crudService.CreateTaskAsync(listId, createTaskRequest, customTaskIds, teamId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CuTask> GetTaskAsync(
            string taskId,
            GetTaskRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetTaskAsync to TaskCrudService");
            return await _crudService.GetTaskAsync(taskId, requestModel, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CuTask> UpdateTaskAsync(
            string taskId,
            UpdateTaskRequest updateTaskRequest,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating UpdateTaskAsync to TaskCrudService");
            return await _crudService.UpdateTaskAsync(taskId, updateTaskRequest, customTaskIds, teamId, cancellationToken);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteTaskAsync(
            string taskId,
            DeleteTaskRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating DeleteTaskAsync to TaskCrudService");
            await _crudService.DeleteTaskAsync(taskId, requestModel, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CuTask> CreateTaskFromTemplateAsync(
            string listId,
            string templateId,
            CreateTaskFromTemplateRequest createTaskFromTemplateRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating CreateTaskFromTemplateAsync to TaskCrudService");
            return await _crudService.CreateTaskFromTemplateAsync(listId, templateId, createTaskFromTemplateRequest, cancellationToken);
        }

        #endregion

        #region Query Operations - Delegated to TaskQueryService

        /// <inheritdoc />
        public async Task<IPagedResult<CuTask>> GetTasksAsync(
            string listId,
            Action<GetTasksRequestParameters>? configureParameters = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetTasksAsync to TaskQueryService");
            return await _queryService.GetTasksAsync(listId, configureParameters, cancellationToken);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<CuTask> GetTasksAsyncEnumerableAsync(
            string listId,
            GetTasksRequestParameters parameters,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetTasksAsyncEnumerableAsync to TaskQueryService");
            return _queryService.GetTasksAsyncEnumerableAsync(listId, parameters, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IPagedResult<CuTask>> GetFilteredTeamTasksAsync(
            string workspaceId,
            Action<GetTasksRequestParameters>? configureParameters = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetFilteredTeamTasksAsync to TaskQueryService");
            return await _queryService.GetFilteredTeamTasksAsync(workspaceId, configureParameters, cancellationToken);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<CuTask> GetFilteredTeamTasksAsyncEnumerableAsync(
            string workspaceId,
            GetTasksRequestParameters parameters,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetFilteredTeamTasksAsyncEnumerableAsync to TaskQueryService");
            return _queryService.GetFilteredTeamTasksAsyncEnumerableAsync(workspaceId, parameters, cancellationToken);
        }

        #endregion

        #region Relationship Operations - Delegated to TaskRelationshipService

        /// <inheritdoc />
        public async Task<CuTask> MergeTasksAsync(
            string targetTaskId,
            MergeTasksRequest mergeTasksRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating MergeTasksAsync to TaskRelationshipService");
            return await _relationshipService.MergeTasksAsync(targetTaskId, mergeTasksRequest, cancellationToken);
        }

        #endregion

        #region Time Tracking Operations - Delegated to TaskTimeTrackingService

        /// <inheritdoc />
        public async Task<TaskTimeInStatusResponse> GetTaskTimeInStatusAsync(
            string taskId,
            GetTaskTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetTaskTimeInStatusAsync to TaskTimeTrackingService");
            return await _timeTrackingService.GetTaskTimeInStatusAsync(taskId, requestModel, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetBulkTasksTimeInStatusResponse> GetBulkTasksTimeInStatusAsync(
            GetBulkTasksTimeInStatusRequest requestModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Delegating GetBulkTasksTimeInStatusAsync to TaskTimeTrackingService");
            return await _timeTrackingService.GetBulkTasksTimeInStatusAsync(requestModel, cancellationToken);
        }

        #endregion
    }
}
