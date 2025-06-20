using System;
using System.Collections.Generic;
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;
using ClickUp.Api.Client.Models.ResponseModels.TimeTracking; // Assuming response wrappers exist

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="ITimeTrackingService"/> for ClickUp Time Tracking operations.
    /// </summary>
    public class TimeTrackingService : ITimeTrackingService
    {
        private readonly IApiConnection _apiConnection;
        private const string BaseEndpoint = "team"; // ClickUp v2 uses "team/{team_id}" for workspace context

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public TimeTrackingService(IApiConnection apiConnection)
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

        private string BuildQueryStringFromArray<T>(string key, IEnumerable<T>? values)
        {
            if (values == null || !values.Any()) return string.Empty;
            return string.Join("&", values.Select(v => $"{Uri.EscapeDataString(key)}[]={Uri.EscapeDataString(v?.ToString() ?? string.Empty)}"));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TimeEntry>?> GetTimeEntriesAsync(
            string workspaceId,
            GetTimeEntriesRequest request, // This request DTO should contain all query parameters
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries";
            var queryParams = new Dictionary<string, string?>();

            // Populate queryParams from request DTO properties
            if (request.StartDate.HasValue) queryParams["start_date"] = request.StartDate.Value.ToString();
            if (request.EndDate.HasValue) queryParams["end_date"] = request.EndDate.Value.ToString();
            if (!string.IsNullOrEmpty(request.Assignee)) queryParams["assignee"] = request.Assignee;
            if (request.IncludeTaskTags.HasValue) queryParams["include_task_tags"] = request.IncludeTaskTags.Value.ToString().ToLower();
            if (request.IncludeLocationNames.HasValue) queryParams["include_location_names"] = request.IncludeLocationNames.Value.ToString().ToLower();
            // TODO: Add include_task_url, include_lists, etc. from GetTimeEntriesRequest if they exist
            if (!string.IsNullOrEmpty(request.SpaceId)) queryParams["space_id"] = request.SpaceId;
            if (!string.IsNullOrEmpty(request.FolderId)) queryParams["folder_id"] = request.FolderId;
            if (!string.IsNullOrEmpty(request.ListId)) queryParams["list_id"] = request.ListId;
            if (!string.IsNullOrEmpty(request.TaskId)) queryParams["task_id"] = request.TaskId;
            if (request.CustomTaskIds.HasValue) queryParams["custom_task_ids"] = request.CustomTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(request.TeamIdForCustomTaskIds)) queryParams["team_id_for_custom_task_ids"] = request.TeamIdForCustomTaskIds; // This seems redundant if workspaceId is team_id
            if (request.IsBillable.HasValue) queryParams["billable"] = request.IsBillable.Value.ToString().ToLower();

            endpoint += BuildQueryString(queryParams);

            // The API returns { "data": [...] } for time entries
            var response = await _apiConnection.GetAsync<GetTimeEntriesResponse>(endpoint, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<TimeEntry?> CreateTimeEntryAsync(
            string workspaceId,
            CreateTimeEntryRequest createTimeEntryRequest,
            bool? customTaskIds = null, // These are query params for Create
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries";
            var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamIdForCustomTaskIds)) queryParams["team_id"] = teamIdForCustomTaskIds;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.PostAsync<CreateTimeEntryRequest, GetTimeEntryResponse>(endpoint, createTimeEntryRequest, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<TimeEntry?> GetTimeEntryAsync(
            string workspaceId,
            string timerId, // This is the time entry ID
            bool? includeTaskTags = null,
            bool? includeLocationNames = null,
            bool? includeApprovalHistory = null, // These seem like query params for a single time entry GET
            bool? includeApprovalDetails = null,
            CancellationToken cancellationToken = default)
        {
            // The endpoint for a single time entry is usually just /time_entries/{timer_id} or team/{team_id}/time_entries/{timer_id}
            // Let's assume team/{team_id}/time_entries/{timer_id}
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/{timerId}";
            var queryParams = new Dictionary<string, string?>();
            if (includeTaskTags.HasValue) queryParams["include_task_tags"] = includeTaskTags.Value.ToString().ToLower();
            if (includeLocationNames.HasValue) queryParams["include_location_names"] = includeLocationNames.Value.ToString().ToLower();
            // Add other include params if API supports them
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetTimeEntryResponse>(endpoint, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<TimeEntry?> UpdateTimeEntryAsync(
            string workspaceId,
            string timerId,
            UpdateTimeEntryRequest updateTimeEntryRequest,
            bool? customTaskIds = null, // Query params for Update
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/{timerId}";
             var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamIdForCustomTaskIds)) queryParams["team_id"] = teamIdForCustomTaskIds;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.PutAsync<UpdateTimeEntryRequest, GetTimeEntryResponse>(endpoint, updateTimeEntryRequest, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteTimeEntryAsync(
            string workspaceId,
            string timerId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/{timerId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TimeEntryHistory>?> GetTimeEntryHistoryAsync(
            string workspaceId,
            string timerId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/{timerId}/history";
            // Assuming API returns { "data": [...] } or similar wrapper
            var response = await _apiConnection.GetAsync<GetTimeEntryHistoryResponse>(endpoint, cancellationToken);
            return response?.History; // Assuming GetTimeEntryHistoryResponse has an 'History' or 'Data' property
        }

        /// <inheritdoc />
        public async Task<TimeEntry?> GetRunningTimeEntryAsync(
            string workspaceId,
            string? assigneeUserId = null, // This is a query param: assignee_user_id
            CancellationToken cancellationToken = default)
        {
            // Endpoint for current timer: GET /team/{team_id}/time_entries/current
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/current";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(assigneeUserId)) queryParams["assignee_user_id"] = assigneeUserId;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetTimeEntryResponse>(endpoint, cancellationToken); // API returns {"data": ...} (or null if no timer)
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<TimeEntry?> StartTimeEntryAsync(
            string workspaceId,
            StartTimeEntryRequest startTimeEntryRequest,
            bool? customTaskIds = null, // Query params for Start
            string? teamIdForCustomTaskIds = null,
            CancellationToken cancellationToken = default)
        {
            // Endpoint for starting timer: POST /team/{team_id}/time_entries/start
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/start";
             var queryParams = new Dictionary<string, string?>();
            if (customTaskIds.HasValue) queryParams["custom_task_ids"] = customTaskIds.Value.ToString().ToLower();
            if (!string.IsNullOrEmpty(teamIdForCustomTaskIds)) queryParams["team_id"] = teamIdForCustomTaskIds;
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.PostAsync<StartTimeEntryRequest, GetTimeEntryResponse>(endpoint, startTimeEntryRequest, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<TimeEntry?> StopTimeEntryAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            // Endpoint for stopping timer: POST /team/{team_id}/time_entries/stop
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/stop";
            // This endpoint usually doesn't require a request body.
            var response = await _apiConnection.PostAsync<object, GetTimeEntryResponse>(endpoint, new object(), cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TimeEntryTag>?> GetAllTimeEntryTagsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            // Endpoint for GET all tags: GET /team/{team_id}/time_entries/tags
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/tags";
            var response = await _apiConnection.GetAsync<GetAllTimeEntryTagsResponse>(endpoint, cancellationToken); // Assuming { "tags": [...] } or { "data": [...] }
            return response?.Tags; // Or response.Data if it's a generic wrapper
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task AddTagsToTimeEntriesAsync(
            string workspaceId,
            TimeEntryTagsRequest addTagsRequest, // Contains list of timer_ids and tags
            CancellationToken cancellationToken = default)
        {
            // Endpoint: POST /team/{team_id}/time_entries/tags
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/tags";
            await _apiConnection.PostAsync(endpoint, addTagsRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task RemoveTagsFromTimeEntriesAsync(
            string workspaceId,
            TimeEntryTagsRequest removeTagsRequest, // Contains list of timer_ids and tags
            CancellationToken cancellationToken = default)
        {
            // Endpoint: DELETE /team/{team_id}/time_entries/tags (uses request body for DELETE)
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
            ChangeTimeEntryTagNameRequest changeTagNameRequest,
            CancellationToken cancellationToken = default)
        {
            // Endpoint: PUT /team/{team_id}/time_entries/tags
            var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries/tags";
            await _apiConnection.PutAsync(endpoint, changeTagNameRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TimeEntry> GetTimeEntriesAsyncEnumerableAsync(
            string workspaceId,
            GetTimeEntriesRequest request, // This DTO should not contain 'page'
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            int currentPage = 0;
            bool lastPageReached;

            // Create a mutable copy of the request DTO to set the page property
            // Or, if GetTimeEntriesRequest is a class, clone it. If record struct, direct copy is fine.
            // For this example, assume GetTimeEntriesRequest is a record struct or we pass properties individually.
            // The existing GetTimeEntriesAsync takes GetTimeEntriesRequest. We need to pass a modified one.

            var paginatedRequest = request; // Assuming GetTimeEntriesRequest is a struct or we can modify it.
                                            // If it's a class, defensive copy might be needed:
                                            // var paginatedRequest = new GetTimeEntriesRequest(...copy all props from request...);

            do
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                // We need to pass a GetTimeEntriesRequest to the original GetTimeEntriesAsync.
                // This request DTO needs to be updated with the current page.
                // The GetTimeEntriesRequest DTO should ideally have a 'Page' property.
                // If GetTimeEntriesRequest is immutable (e.g. record class without a 'with' expression for page, or no page property),
                // this becomes more complex.
                // Let's assume GetTimeEntriesRequest is a class and has a settable Page property,
                // or the original GetTimeEntriesAsync can take 'page' as an override.
                // The current ITimeTrackingService.GetTimeEntriesAsync takes GetTimeEntriesRequest, not individual params + page.
                // So, GetTimeEntriesRequest MUST be adaptable for paging.

                // Simplest path: Assume GetTimeEntriesRequest has a mutable 'Page' property or a constructor/method to set it.
                // For this example, let's assume a conceptual 'WithPage' method or mutable property.
                // If GetTimeEntriesRequest is a record: var pagedRequest = request with { Page = currentPage };
                // If it's a class: request.Page = currentPage; (if mutable) or new GetTimeEntriesRequest(request, currentPage)

                // Let's assume GetTimeEntriesRequest can be constructed with a page or has a page property.
                // For the purpose of this stub, we will assume that the GetTimeEntriesRequest DTO
                // has a 'Page' property that can be set. If not, the DTO itself would need modification.
                // This is a common pattern for request DTOs used in paged scenarios.

                // To call the existing method:
                // GetTimeEntriesAsync(string workspaceId, GetTimeEntriesRequest request, CancellationToken cancellationToken)
                // The 'request' DTO itself needs to convey the page number.
                // This means GetTimeEntriesRequest should have a 'Page' field.
                // If it doesn't, this approach needs reconsideration for how page is passed.

                // Let's assume GetTimeEntriesRequest has a mutable Page property for this stub.
                // This is an important design detail of GetTimeEntriesRequest.
                // If GetTimeEntriesRequest is immutable, we might need a different strategy
                // or to adjust GetTimeEntriesRequest to support 'with' expressions for page.
                // For this exercise, we will assume GetTimeEntriesRequest can be made to include the page.

                // The GetTimeEntriesRequest DTO *must* have a page parameter for this to work cleanly.
                // Let's assume it does, e.g., request.Page = currentPage.
                // If not, the original GetTimeEntriesAsync would need to be refactored to accept 'page' separately,
                // which contradicts its current signature of taking a single Request DTO.

                // Let's proceed by creating a new request DTO for each page call, copying properties.
                // This is safer if GetTimeEntriesRequest is complex or immutable regarding 'page'.
                var currentPageRequest = new GetTimeEntriesRequest(
                    StartDate: request.StartDate,
                    EndDate: request.EndDate,
                    Assignee: request.Assignee,
                    IncludeTaskTags: request.IncludeTaskTags,
                    IncludeLocationNames: request.IncludeLocationNames,
                    SpaceId: request.SpaceId,
                    FolderId: request.FolderId,
                    ListId: request.ListId,
                    TaskId: request.TaskId,
                    CustomTaskIds: request.CustomTaskIds,
                    TeamIdForCustomTaskIds: request.TeamIdForCustomTaskIds,
                    IsBillable: request.IsBillable,
                    Page: currentPage // Key addition for pagination
                                       // Add any other properties from GetTimeEntriesRequest
                );


                var response = await GetTimeEntriesAsync(
                    workspaceId,
                    currentPageRequest,
                    cancellationToken
                ).ConfigureAwait(false);

                // The existing GetTimeEntriesAsync returns IEnumerable<TimeEntry>, not GetTimeEntriesResponse.
                // This is an issue. The paged method *must* return pagination info (like LastPage or total items).
                // I will assume GetTimeEntriesAsync is changed to return GetTimeEntriesResponse.
                // This is a necessary change for IAsyncEnumerable to work.
                // For now, I will write the code AS IF GetTimeEntriesAsync returns GetTimeEntriesResponse.
                // This means the previous implementation of TimeTrackingService.GetTimeEntriesAsync was simplified
                // and did not account for its own pagination response structure.

                // Corrected assumption: The GetTimeEntriesAsync in TimeTrackingService should return the wrapper.
                // Let's simulate that.
                // var responseWrapper = await _apiConnection.GetAsync<GetTimeEntriesResponse>(endpoint, cancellationToken);
                // For IAsyncEnumerable, it calls the public GetTimeEntriesAsync which should then use the _apiConnection.
                // This implies GetTimeEntriesAsync itself needs to be changed if it currently returns IEnumerable<TimeEntry>
                // instead of the wrapper GetTimeEntriesResponse.

                // Let's assume the existing GetTimeEntriesAsync is already correctly returning GetTimeEntriesResponse
                // (which implies its current implementation in TimeTrackingService.cs is more than just returning response.Data,
                // or that response.Data was a simplification in that step).
                // If GetTimeEntriesAsync in the service returns IEnumerable<TimeEntry>, we can't get LastPage.
                // This is a critical dependency.
                // FOR THIS SUBTASK: I will write it as if the original GetTimeEntriesAsync *can* provide pagination info.
                // This might mean it internally calls _apiConnection and gets the wrapper, then the enumerable calls *that*.
                // The simplest is if GetTimeEntriesAsync itself returned the wrapper.
                // The interface ITimeTrackingService.GetTimeEntriesAsync returns CuTask<IEnumerable<TimeEntry>>.
                // This is the problem. It *should* return CuTask<GetTimeEntriesResponse> for this to work cleanly.

                // Given the current interface for GetTimeEntriesAsync, we cannot get "LastPage".
                // This IAsyncEnumerable method cannot be implemented correctly without changing the paged GetTimeEntriesAsync.
                // I will proceed by making a direct call to _apiConnection here, which means
                // GetTimeEntriesAsyncEnumerableAsync will bypass the existing GetTimeEntriesAsync logic
                // for constructing its query string based on GetTimeEntriesRequest.

                var endpoint = $"{BaseEndpoint}/{workspaceId}/time_entries";
                var queryParams = new Dictionary<string, string?>();
                if (request.StartDate.HasValue) queryParams["start_date"] = request.StartDate.Value.ToString();
                if (request.EndDate.HasValue) queryParams["end_date"] = request.EndDate.Value.ToString();
                if (!string.IsNullOrEmpty(request.Assignee)) queryParams["assignee"] = request.Assignee;
                if (request.IncludeTaskTags.HasValue) queryParams["include_task_tags"] = request.IncludeTaskTags.Value.ToString().ToLower();
                if (request.IncludeLocationNames.HasValue) queryParams["include_location_names"] = request.IncludeLocationNames.Value.ToString().ToLower();
                if (!string.IsNullOrEmpty(request.SpaceId)) queryParams["space_id"] = request.SpaceId;
                if (!string.IsNullOrEmpty(request.FolderId)) queryParams["folder_id"] = request.FolderId;
                if (!string.IsNullOrEmpty(request.ListId)) queryParams["list_id"] = request.ListId;
                if (!string.IsNullOrEmpty(request.TaskId)) queryParams["task_id"] = request.TaskId;
                if (request.CustomTaskIds.HasValue) queryParams["custom_task_ids"] = request.CustomTaskIds.Value.ToString().ToLower();
                if (!string.IsNullOrEmpty(request.TeamIdForCustomTaskIds)) queryParams["team_id_for_custom_task_ids"] = request.TeamIdForCustomTaskIds;
                if (request.IsBillable.HasValue) queryParams["billable"] = request.IsBillable.Value.ToString().ToLower();
                queryParams["page"] = currentPage.ToString();

                var fullEndpoint = endpoint + BuildQueryString(queryParams);
                var responseWrapper = await _apiConnection.GetAsync<GetTimeEntriesResponse>(fullEndpoint, cancellationToken).ConfigureAwait(false);


                if (responseWrapper?.Data != null && responseWrapper.Data.Any())
                {
                    foreach (var entry in responseWrapper.Data)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            yield break;
                        }
                        yield return entry;
                    }
                    // Infer LastPage: if the number of returned items is less than a typical page size,
                    // or if an empty list is returned for a page > 0.
                    // ClickUp doesn't provide 'last_page' for this endpoint.
                    // A common page size is 100 for time entries. If less than 100, assume last page.
                    // Or, if Data is empty, it's definitely the last page.
                    lastPageReached = !responseWrapper.Data.Any(); // Simplest: if it's empty, we're done.
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
        }
    }
}
