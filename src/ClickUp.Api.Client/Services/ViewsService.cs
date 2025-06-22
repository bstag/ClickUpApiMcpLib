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
using ClickUp.Api.Client.Models.Entities.Views; // View, CuTask DTOs
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views; // GetViewTasksResponse and potential GetViewsResponse, GetViewResponse
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IViewsService"/> for ClickUp View operations.
    /// </summary>
    public class ViewsService : IViewsService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<ViewsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public ViewsService(IApiConnection apiConnection, ILogger<ViewsService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<ViewsService>.Instance;
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
        public async Task<GetViewsResponse> GetWorkspaceViewsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting workspace views for workspace ID: {WorkspaceId}", workspaceId);
            var endpoint = $"team/{workspaceId}/view"; // team_id is workspaceId
            var response = await _apiConnection.GetAsync<GetViewsResponse>(endpoint, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for GetWorkspaceViewsAsync (Workspace ID: {workspaceId}).");
        }

        /// <inheritdoc />
        public async Task<CreateTeamViewResponse> CreateWorkspaceViewAsync(
            string workspaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating workspace view in workspace ID: {WorkspaceId}, Name: {ViewName}", workspaceId, createViewRequest.Name);
            var endpoint = $"team/{workspaceId}/view";
            // Assuming CreateTeamViewResponse is the correct wrapper, or if it's just View, adjust PostAsync generic type
            var response = await _apiConnection.PostAsync<CreateViewRequest, CreateTeamViewResponse>(endpoint, createViewRequest, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for CreateWorkspaceViewAsync (Workspace ID: {workspaceId}).");
        }

        /// <inheritdoc />
        public async Task<GetViewsResponse> GetSpaceViewsAsync(
            string spaceId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting space views for space ID: {SpaceId}", spaceId);
            var endpoint = $"space/{spaceId}/view";
            var response = await _apiConnection.GetAsync<GetViewsResponse>(endpoint, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for GetSpaceViewsAsync (Space ID: {spaceId}).");
        }

        /// <inheritdoc />
        public async Task<CreateSpaceViewResponse> CreateSpaceViewAsync(
            string spaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating space view in space ID: {SpaceId}, Name: {ViewName}", spaceId, createViewRequest.Name);
            var endpoint = $"space/{spaceId}/view";
            var response = await _apiConnection.PostAsync<CreateViewRequest, CreateSpaceViewResponse>(endpoint, createViewRequest, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for CreateSpaceViewAsync (Space ID: {spaceId}).");
        }

        /// <inheritdoc />
        public async Task<GetViewsResponse> GetFolderViewsAsync(
            string folderId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting folder views for folder ID: {FolderId}", folderId);
            var endpoint = $"folder/{folderId}/view";
            var response = await _apiConnection.GetAsync<GetViewsResponse>(endpoint, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for GetFolderViewsAsync (Folder ID: {folderId}).");
        }

        /// <inheritdoc />
        public async Task<CreateFolderViewResponse> CreateFolderViewAsync(
            string folderId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating folder view in folder ID: {FolderId}, Name: {ViewName}", folderId, createViewRequest.Name);
            var endpoint = $"folder/{folderId}/view";
            var response = await _apiConnection.PostAsync<CreateViewRequest, CreateFolderViewResponse>(endpoint, createViewRequest, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for CreateFolderViewAsync (Folder ID: {folderId}).");
        }

        /// <inheritdoc />
        public async Task<GetViewsResponse> GetListViewsAsync(
            string listId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting list views for list ID: {ListId}", listId);
            var endpoint = $"list/{listId}/view";
            var response = await _apiConnection.GetAsync<GetViewsResponse>(endpoint, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for GetListViewsAsync (List ID: {listId}).");
        }

        /// <inheritdoc />
        public async Task<CreateListViewResponse> CreateListViewAsync(
            string listId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating list view in list ID: {ListId}, Name: {ViewName}", listId, createViewRequest.Name);
            var endpoint = $"list/{listId}/view";
            var response = await _apiConnection.PostAsync<CreateViewRequest, CreateListViewResponse>(endpoint, createViewRequest, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for CreateListViewAsync (List ID: {listId}).");
        }

        /// <inheritdoc />
        public async Task<GetViewResponse> GetViewAsync(
            string viewId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting view ID: {ViewId}", viewId);
            var endpoint = $"view/{viewId}";
            var response = await _apiConnection.GetAsync<GetViewResponse>(endpoint, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for GetViewAsync (View ID: {viewId}).");
        }

        /// <inheritdoc />
        public async Task<UpdateViewResponse> UpdateViewAsync(
            string viewId,
            UpdateViewRequest updateViewRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating view ID: {ViewId}, Name: {ViewName}", viewId, updateViewRequest.Name);
            var endpoint = $"view/{viewId}";
            var response = await _apiConnection.PutAsync<UpdateViewRequest, UpdateViewResponse>(endpoint, updateViewRequest, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for UpdateViewAsync (View ID: {viewId}).");
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteViewAsync(
            string viewId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting view ID: {ViewId}", viewId);
            var endpoint = $"view/{viewId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetViewTasksResponse> GetViewTasksAsync(
            string viewId,
            int page,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting view tasks for view ID: {ViewId}, Page: {Page}", viewId, page);
            var endpoint = $"view/{viewId}/task";
            var queryParams = new Dictionary<string, string?>
            {
                { "page", page.ToString() } // 'page' is a required query parameter
            };
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetViewTasksResponse>(endpoint, cancellationToken);
            return response ?? throw new InvalidOperationException($"API response was null for GetViewTasksAsync (View ID: {viewId}, Page: {page}).");
        }

        // Removed redundant explicit interface implementations that were throwing NotImplementedException.
        // The public methods above with matching signatures implicitly implement the interface.
    }
}
