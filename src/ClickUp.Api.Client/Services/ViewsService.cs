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

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IViewsService"/> for ClickUp View operations.
    /// </summary>
    public class ViewsService : IViewsService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public ViewsService(IApiConnection apiConnection)
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
        public async Task<IEnumerable<View>?> GetWorkspaceViewsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/view"; // team_id is workspaceId
            var response = await _apiConnection.GetAsync<GetViewsResponse>(endpoint, cancellationToken); // API returns {"views": [...]}
            return response?.Views;
        }

        /// <inheritdoc />
        public async Task<View?> CreateWorkspaceViewAsync(
            string workspaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"team/{workspaceId}/view";
            var response = await _apiConnection.PostAsync<CreateViewRequest, GetViewResponse>(endpoint, createViewRequest, cancellationToken); // API returns {"view": {...}}
            return response?.View;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<View>?> GetSpaceViewsAsync(
            string spaceId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"space/{spaceId}/view";
            var response = await _apiConnection.GetAsync<GetViewsResponse>(endpoint, cancellationToken);
            return response?.Views;
        }

        /// <inheritdoc />
        public async Task<View?> CreateSpaceViewAsync(
            string spaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"space/{spaceId}/view";
            var response = await _apiConnection.PostAsync<CreateViewRequest, GetViewResponse>(endpoint, createViewRequest, cancellationToken);
            return response?.View;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<View>?> GetFolderViewsAsync(
            string folderId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"folder/{folderId}/view";
            var response = await _apiConnection.GetAsync<GetViewsResponse>(endpoint, cancellationToken);
            return response?.Views;
        }

        /// <inheritdoc />
        public async Task<View?> CreateFolderViewAsync(
            string folderId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"folder/{folderId}/view";
            var response = await _apiConnection.PostAsync<CreateViewRequest, GetViewResponse>(endpoint, createViewRequest, cancellationToken);
            return response?.View;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<View>?> GetListViewsAsync(
            string listId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"list/{listId}/view";
            var response = await _apiConnection.GetAsync<GetViewsResponse>(endpoint, cancellationToken);
            return response?.Views;
        }

        /// <inheritdoc />
        public async Task<View?> CreateListViewAsync(
            string listId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"list/{listId}/view";
            var response = await _apiConnection.PostAsync<CreateViewRequest, GetViewResponse>(endpoint, createViewRequest, cancellationToken);
            return response?.View;
        }

        /// <inheritdoc />
        public async Task<View?> GetViewAsync(
            string viewId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"view/{viewId}";
            var response = await _apiConnection.GetAsync<GetViewResponse>(endpoint, cancellationToken);
            return response?.View;
        }

        /// <inheritdoc />
        public async Task<View?> UpdateViewAsync(
            string viewId,
            UpdateViewRequest updateViewRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"view/{viewId}";
            var response = await _apiConnection.PutAsync<UpdateViewRequest, GetViewResponse>(endpoint, updateViewRequest, cancellationToken);
            return response?.View;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteViewAsync(
            string viewId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"view/{viewId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetViewTasksResponse?> GetViewTasksAsync(
            string viewId,
            int page,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"view/{viewId}/task";
            var queryParams = new Dictionary<string, string?>
            {
                { "page", page.ToString() } // 'page' is a required query parameter
            };
            endpoint += BuildQueryString(queryParams);

            return await _apiConnection.GetAsync<GetViewTasksResponse>(endpoint, cancellationToken);
        }
    }
}
