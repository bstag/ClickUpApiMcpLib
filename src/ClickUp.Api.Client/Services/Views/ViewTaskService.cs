using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services.Views
{
    /// <summary>
    /// Provides view task operations for ClickUp Views.
    /// Implements the Single Responsibility Principle by focusing only on retrieving tasks from views.
    /// </summary>
    public class ViewTaskService : IViewTaskService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<ViewTaskService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTaskService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        public ViewTaskService(IApiConnection apiConnection, ILogger<ViewTaskService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<ViewTaskService>.Instance;
        }

        /// <inheritdoc />
        public async Task<IPagedResult<CuTask>> GetViewTasksAsync(
            string viewId,
            GetViewTasksRequest request,
            CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(viewId, nameof(viewId));
            if (request == null) throw new ArgumentNullException(nameof(request));

            _logger.LogInformation("Getting tasks for view ID: {ViewId}", viewId);

            var endpoint = UrlBuilderHelper.CreateBuilder()
                .WithPathSegments("view", viewId, "task")
                .WithQueryParameter("page", request.Page.ToString())
                .Build();

            var result = await _apiConnection.GetAsync<PagedResult<CuTask>>(endpoint, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting tasks for view {viewId}.");
            }

            _logger.LogDebug("Successfully retrieved {TaskCount} tasks for view {ViewId}", result.Items?.Count ?? 0, viewId);
            return result;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CuTask> GetViewTasksAsyncEnumerableAsync(
            string viewId,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            ValidationHelper.ValidateId(viewId, nameof(viewId));

            _logger.LogInformation("Starting enumerable retrieval of tasks for view ID: {ViewId}", viewId);

            var request = new GetViewTasksRequest { Page = 0 };
            IPagedResult<CuTask> pagedResult;

            do
            {
                pagedResult = await GetViewTasksAsync(viewId, request, cancellationToken);

                foreach (var task in pagedResult.Items ?? new List<CuTask>())
                {
                    yield return task;
                }

                request.Page++;
            }
            while (pagedResult.HasNextPage);

            _logger.LogDebug("Completed enumerable retrieval of tasks for view {ViewId}", viewId);
        }
    }
}