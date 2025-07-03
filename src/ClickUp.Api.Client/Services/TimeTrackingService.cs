using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Common.ValueObjects;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.Parameters; // For GetTimeEntriesRequestParameters
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using ClickUp.Api.Client.Models.ResponseModels.TimeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ITimeTrackingService"/> for ClickUp Time Tracking operations.
    /// </summary>
    public class TimeTrackingService : ITimeTrackingService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<TimeTrackingService> _logger;
        private const string BaseEndpoint = "team";

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public TimeTrackingService(IApiConnection apiConnection, ILogger<TimeTrackingService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<TimeTrackingService>.Instance;
        }

        private string BuildQueryString(Dictionary<string, string?> queryParams) // Accepts nullable string values
        {
            if (queryParams == null || !queryParams.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder(); // Start empty, will add '?' if params exist
            var first = true;
            foreach (var kvp in queryParams)
            {
                if (kvp.Value == null) // Skip parameters with null values
                {
                    continue;
                }

                if (first)
                {
                    sb.Append('?');
                    first = false;
                }
                else
                {
                    sb.Append('&');
                }
                // Values should be URL-encoded. Assuming ToDictionary methods on parameter objects do not do this,
                // or do it inconsistently. It's safer to encode here.
                sb.Append($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
            }
            return sb.ToString();
        }

        /// <inheritdoc />
        public async Task<IPagedResult<TimeEntry>> GetTimeEntriesAsync(
            string workspaceId,
            Action<GetTimeEntriesRequestParameters>? configureParameters = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workspaceId)) throw new ArgumentNullException(nameof(workspaceId));

            var parameters = new GetTimeEntriesRequestParameters();
            configureParameters?.Invoke(parameters);

            int currentPage = parameters.Page ?? 0;
            parameters.Page = currentPage; // Ensure Page is set for ToDictionary

            _logger.LogInformation("Getting time entries for workspace ID: {WorkspaceId}, Parameters: {@Parameters}", workspaceId, parameters);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries";

            var queryDictString = parameters.ToDictionary();
            var queryDictNullable = queryDictString.ToDictionary(kvp => kvp.Key, kvp => (string?)kvp.Value);
            var fullEndpoint = endpoint + BuildQueryString(queryDictNullable); // Use BuildQueryString with converted dict
            var response = await _apiConnection.GetAsync<GetTimeEntriesResponse>(fullEndpoint, cancellationToken); // Corrected GetAsync call

            var items = response?.Data ?? Enumerable.Empty<TimeEntry>();

            const int assumedPageSizeWhenFull = 100;
            bool hasNextPage = items.Count() == assumedPageSizeWhenFull; // Heuristic as API doesn't provide total/last_page

            return new PagedResult<TimeEntry>(
                items,
                currentPage,
                items.Count(), // This is items per page, not total items
                hasNextPage
            );
        }

        /// <inheritdoc />
        public async Task<TimeEntry> CreateTimeEntryAsync(
            string workspaceId,
            CreateTimeEntryRequest createTimeEntryRequest,
            bool? customTaskIds = null, // These are query params for Create
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating time entry in workspace ID: {WorkspaceId}, Description: {Description}", workspaceId, createTimeEntryRequest.Description);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamIdForCustomTaskIds)) queryParams["team_id"] = teamIdForCustomTaskIds;
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(endpoint, createTimeEntryRequest, cancellationToken);
            if (responseWrapper?.Data == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty data response when creating time entry in workspace {workspaceId}.");
            }
            return responseWrapper.Data;
        }

        /// <inheritdoc />
        public async Task<TimeEntry> GetTimeEntryAsync(
            string workspaceId,
            string timerId, // This is the time entry ID
            bool? includeTaskTags = null,
            bool? includeLocationNames = null,
            bool? includeApprovalHistory = null, // These seem like query params for a single time entry GET
            bool? includeApprovalDetails = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting time entry ID: {TimerId} in workspace ID: {WorkspaceId}", timerId, workspaceId);
            // The endpoint for a single time entry is usually just /time_entries/{timer_id} or team/{team_id}/time_entries/{timer_id}
            // Let's assume team/{team_id}/time_entries/{timer_id}
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/{timerId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeTaskTags.HasValue) queryParams["include_task_tags"] = includeTaskTags.Value.ToString().ToLower();
            if (includeLocationNames.HasValue) queryParams["include_location_names"] = includeLocationNames.Value.ToString().ToLower();
            // Add other include params if API supports them
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.GetAsync<GetTimeEntryResponse>(endpoint, cancellationToken);
            if (responseWrapper?.Data == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty data response when getting time entry {timerId} in workspace {workspaceId}.");
            }
            return responseWrapper.Data;
        }

        /// <inheritdoc />
        public async Task<TimeEntry> UpdateTimeEntryAsync(
            string workspaceId,
            string timerId,
            UpdateTimeEntryRequest updateTimeEntryRequest,
            bool? customTaskIds = null, // Query params for Update
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating time entry ID: {TimerId} in workspace ID: {WorkspaceId}", timerId, workspaceId);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/{timerId}";
             var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamIdForCustomTaskIds)) queryParams["team_id"] = teamIdForCustomTaskIds;
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(endpoint, updateTimeEntryRequest, cancellationToken);
            if (responseWrapper?.Data == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty data response when updating time entry {timerId} in workspace {workspaceId}.");
            }
            return responseWrapper.Data;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteTimeEntryAsync(
            string workspaceId,
            string timerId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting time entry ID: {TimerId} in workspace ID: {WorkspaceId}", timerId, workspaceId);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/{timerId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TimeEntryHistory>> GetTimeEntryHistoryAsync(
            string workspaceId,
            string timerId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting time entry history for timer ID: {TimerId} in workspace ID: {WorkspaceId}", timerId, workspaceId);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/{timerId}/history";
            var response = await _apiConnection.GetAsync<GetTimeEntryHistoryResponse>(endpoint, cancellationToken);
            // Assuming GetTimeEntryHistoryResponse has a 'History' or 'Data' property that is IEnumerable<TimeEntryHistory>
            return response?.History ?? Enumerable.Empty<TimeEntryHistory>();
        }

        /// <inheritdoc />
        public async Task<TimeEntry?> GetRunningTimeEntryAsync(
            string workspaceId,
            string? assigneeUserId = null, // This is a query param: assignee_user_id
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting running time entry for workspace ID: {WorkspaceId}, Assignee: {AssigneeUserId}", workspaceId, assigneeUserId);
            // Endpoint for current timer: GET /team/{team_id}/time_entries/current
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/current";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(assigneeUserId)) queryParams["assignee_user_id"] = assigneeUserId;
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.GetAsync<GetTimeEntryResponse>(endpoint, cancellationToken); // API returns {"data": ...} (or null if no timer)
            return responseWrapper?.Data; // Return null if responseWrapper or its Data is null
        }

        /// <inheritdoc />
        public async Task<TimeEntry> StartTimeEntryAsync(
            string workspaceId,
            StartTimeEntryRequest startTimeEntryRequest,
            bool? customTaskIds = null, // Query params for Start
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting time entry in workspace ID: {WorkspaceId}, Task ID: {TaskId}", workspaceId, startTimeEntryRequest.TaskId);
            // Endpoint for starting timer: POST /team/{team_id}/time_entries/start
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/start";
             var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamIdForCustomTaskIds)) queryParams["team_id"] = teamIdForCustomTaskIds;
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(endpoint, startTimeEntryRequest, cancellationToken);
            if (responseWrapper?.Data == null)
            {
                throw new InvalidOperationException($"API connection returned null or empty data response when starting time entry in workspace {workspaceId}.");
            }
            return responseWrapper.Data;
        }

        /// <inheritdoc />
        public async Task<TimeEntry> StopTimeEntryAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Stopping time entry in workspace ID: {WorkspaceId}", workspaceId);
            // Endpoint for stopping timer: POST /team/{team_id}/time_entries/stop
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/stop";
            // This endpoint usually doesn't require a request body.
            var responseWrapper = await _apiConnection.PostAsync<object, GetTimeEntryResponse>(endpoint, new object(), cancellationToken);
            if (responseWrapper?.Data == null)
            {
                // If stopping a timer that wasn't running, API might return error or specific response.
                // Assuming for now that a successful stop returns the stopped entry.
                throw new InvalidOperationException($"API connection returned null or empty data response when stopping time entry in workspace {workspaceId}.");
            }
            return responseWrapper.Data;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TaskTag>> GetAllTimeEntryTagsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting all time entry tags for workspace ID: {WorkspaceId}", workspaceId);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/tags";
            var response = await _apiConnection.GetAsync<GetAllTimeEntryTagsResponse>(endpoint, cancellationToken);
            return response?.Data ?? Enumerable.Empty<TaskTag>();
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task AddTagsToTimeEntriesAsync(
            string workspaceId,
            AddTagsFromTimeEntriesRequest addTagsRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding tags to time entries in workspace ID: {WorkspaceId}", workspaceId);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/tags";
            await _apiConnection.PostAsync(endpoint, addTagsRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task RemoveTagsFromTimeEntriesAsync(
            string workspaceId,
            RemoveTagsFromTimeEntriesRequest removeTagsRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing tags from time entries in workspace ID: {WorkspaceId}", workspaceId);
            // IApiConnection.DeleteAsync is void and doesn't take a body.
            // This requires a DeleteAsync<TRequest> or similar, or a custom SendAsync call.
            // For now, this specific method cannot be fully implemented with current IApiConnection.
            // throw new NotImplementedException("RemoveTagsFromTimeEntriesAsync requires IApiConnection.DeleteAsync with a request body capability.");
            // As a workaround if API allows DELETE with body and IApiConnection doesn't support it directly:
            // One might have to construct HttpRequestMessage manually and use a more generic Send method if available.
            // Given the constraint to use existing IApiConnection methods if possible:
            // If the API can take these as query parameters instead of body for DELETE, that would be an alternative.
            // ClickUp API Spec for "Remove Tags From Time Entries" says DELETE with a body.
            // This means a new method is needed in IApiConnection: DeleteAsync<TRequest>(string endpoint, TRequest payload, CancellationToken token)

            // For this subtask, I will add the conceptual new method to IApiConnection and ApiConnection.
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/tags";
            await _apiConnection.DeleteAsync(endpoint, removeTagsRequest, cancellationToken); // Assumes new overload
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task ChangeTimeEntryTagNameAsync(
            string workspaceId,
            UpdateTimeEntryRequest changeTagNameRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Changing time entry tag name in workspace ID: {WorkspaceId}", workspaceId);
            // Endpoint: PUT /team/{team_id}/time_entries/tags
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/tags";
            await _apiConnection.PutAsync(endpoint, changeTagNameRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TimeEntry> GetTimeEntriesAsyncEnumerableAsync(
            string workspaceId,
            GetTimeEntriesRequestParameters parameters, // Changed from individual params + GetTimeEntriesRequest
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workspaceId)) throw new ArgumentNullException(nameof(workspaceId));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            _logger.LogInformation("Getting time entries as async enumerable for workspace ID: {WorkspaceId}, Parameters: {@Parameters}", workspaceId, parameters);

            var currentParameters = new GetTimeEntriesRequestParameters // Mutable copy for pagination
            {
                TimeRange = parameters.TimeRange,
                AssigneeUserId = parameters.AssigneeUserId,
                TaskId = parameters.TaskId,
                ListId = parameters.ListId,
                FolderId = parameters.FolderId,
                SpaceId = parameters.SpaceId,
                IncludeTaskTags = parameters.IncludeTaskTags,
                IncludeLocationNames = parameters.IncludeLocationNames,
                CustomTaskIds = parameters.CustomTaskIds,
                TeamIdForCustomTaskIds = parameters.TeamIdForCustomTaskIds
                // Page handled by loop
            };

            int currentPage = parameters.Page ?? 0;
            bool lastPageReached;

            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                currentParameters.Page = currentPage;

                _logger.LogDebug("Fetching page {CurrentPage} for time entries in workspace ID {WorkspaceId} via async enumerable.", currentPage, workspaceId);

                var pagedResult = await GetTimeEntriesAsync(workspaceId, cfg => {
                    cfg.TimeRange = currentParameters.TimeRange;
                    cfg.AssigneeUserId = currentParameters.AssigneeUserId;
                    cfg.TaskId = currentParameters.TaskId;
                    cfg.ListId = currentParameters.ListId;
                    cfg.FolderId = currentParameters.FolderId;
                    cfg.SpaceId = currentParameters.SpaceId;
                    cfg.IncludeTaskTags = currentParameters.IncludeTaskTags;
                    cfg.IncludeLocationNames = currentParameters.IncludeLocationNames;
                    cfg.Page = currentParameters.Page;
                    cfg.CustomTaskIds = currentParameters.CustomTaskIds;
                    cfg.TeamIdForCustomTaskIds = currentParameters.TeamIdForCustomTaskIds;
                }, cancellationToken).ConfigureAwait(false);

                if (pagedResult?.Items != null && pagedResult.Items.Any())
                {
                    foreach (var entry in pagedResult.Items)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        yield return entry;
                    }
                    lastPageReached = !pagedResult.HasNextPage;
                }
                else
                {
                    lastPageReached = true;
                }

                if (!lastPageReached)
                {
                    currentPage++;
                }
            } while (!lastPageReached);
            _logger.LogInformation("Finished streaming time entries for workspace ID: {WorkspaceId}", workspaceId);
        }

        public Task ChangeTimeEntryTagNameAsync(string workspaceId, ChangeTagNamesFromTimeEntriesRequest changeTagNamesRequest, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
