using System;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.RequestModels.Views;
using ClickUp.Api.Client.Models.ResponseModels.Views;
using ClickUp.Api.Client.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClickUp.Api.Client.Services.Views
{
    /// <summary>
    /// Provides querying and creation operations for ClickUp Views across different contexts.
    /// Implements the Single Responsibility Principle by focusing only on view querying and context-specific creation operations.
    /// </summary>
    public class ViewQueryService : IViewQueryService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<ViewQueryService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewQueryService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        public ViewQueryService(IApiConnection apiConnection, ILogger<ViewQueryService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<ViewQueryService>.Instance;
        }

        /// <inheritdoc />
        public async Task<GetViewsResponse> GetWorkspaceViewsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default)
        {
            ValidateStringParameter(workspaceId, nameof(workspaceId));

            var url = BuildUrl($"team/{workspaceId}/view");
            return await GetAsync<GetViewsResponse>(url, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CreateTeamViewResponse> CreateWorkspaceViewAsync(
            string workspaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            ValidateStringParameter(workspaceId, nameof(workspaceId));
            ValidateObjectParameter(createViewRequest, nameof(createViewRequest));

            var url = BuildUrl($"team/{workspaceId}/view");
            return await PostAsync<CreateViewRequest, CreateTeamViewResponse>(url, createViewRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetViewsResponse> GetSpaceViewsAsync(
            string spaceId,
            CancellationToken cancellationToken = default)
        {
            ValidateStringParameter(spaceId, nameof(spaceId));

            var url = BuildUrl($"space/{spaceId}/view");
            return await GetAsync<GetViewsResponse>(url, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CreateSpaceViewResponse> CreateSpaceViewAsync(
            string spaceId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            ValidateStringParameter(spaceId, nameof(spaceId));
            ValidateObjectParameter(createViewRequest, nameof(createViewRequest));

            var url = BuildUrl($"space/{spaceId}/view");
            return await PostAsync<CreateViewRequest, CreateSpaceViewResponse>(url, createViewRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetViewsResponse> GetFolderViewsAsync(
            string folderId,
            CancellationToken cancellationToken = default)
        {
            ValidateStringParameter(folderId, nameof(folderId));

            var url = BuildUrl($"folder/{folderId}/view");
            return await GetAsync<GetViewsResponse>(url, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CreateFolderViewResponse> CreateFolderViewAsync(
            string folderId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            ValidateStringParameter(folderId, nameof(folderId));
            ValidateObjectParameter(createViewRequest, nameof(createViewRequest));

            var url = BuildUrl($"folder/{folderId}/view");
            return await PostAsync<CreateViewRequest, CreateFolderViewResponse>(url, createViewRequest, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<GetViewsResponse> GetListViewsAsync(
            string listId,
            CancellationToken cancellationToken = default)
        {
            ValidateStringParameter(listId, nameof(listId));

            var url = BuildUrl($"list/{listId}/view");
            return await GetAsync<GetViewsResponse>(url, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CreateListViewResponse> CreateListViewAsync(
            string listId,
            CreateViewRequest createViewRequest,
            CancellationToken cancellationToken = default)
        {
            ValidateStringParameter(listId, nameof(listId));
            ValidateObjectParameter(createViewRequest, nameof(createViewRequest));

            var url = BuildUrl($"list/{listId}/view");
            return await PostAsync<CreateViewRequest, CreateListViewResponse>(url, createViewRequest, cancellationToken);
        }

        private static void ValidateStringParameter(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", parameterName);
        }

        private static void ValidateObjectParameter(object value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        private static string BuildUrl(string path)
        {
            return path;
        }

        private async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting data from URL: {Url}", url);
            
            var endpoint = UrlBuilderHelper.CreateBuilder()
                .WithPathSegments(url.Split('/'))
                .Build();

            var result = await _apiConnection.GetAsync<T>(endpoint, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting data from {url}.");
            }
            
            _logger.LogDebug("Successfully retrieved data from {Url}", url);
            return result;
        }

        private async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Posting data to URL: {Url}", url);
            
            var endpoint = UrlBuilderHelper.CreateBuilder()
                .WithPathSegments(url.Split('/'))
                .Build();

            var result = await _apiConnection.PostAsync<TRequest, TResponse>(endpoint, request, cancellationToken);
            if (result == null)
            {
                throw new InvalidOperationException($"API connection returned null response when posting data to {url}.");
            }
            
            _logger.LogInformation("Successfully posted data to {Url}", url);
            return result;
        }
    }
}