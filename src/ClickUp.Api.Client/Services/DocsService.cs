using System;
using System.Collections.Generic;
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using ClickUp.Api.Client.Models.ResponseModels.Docs;
// Assuming a generic wrapper for single entity responses if API wraps them in "data"
// For example: public class ClickUpV3DataResponse<T> { public T Data { get; set; } }

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IDocsService"/> for ClickUp Docs (v3) operations.
    /// </summary>
    public class DocsService : IDocsService
    {
        private readonly IApiConnection _apiConnection;
        private const string BaseEndpoint = "/v3/workspaces"; // Base for v3 Docs API

        /// <summary>
        /// Initializes a new instance of the <see cref="DocsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public DocsService(IApiConnection apiConnection)
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
        public async Task<SearchDocsResponse> SearchDocsAsync(
            string workspaceId,
            SearchDocsRequest searchDocsRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(searchDocsRequest.Query)) queryParams["q"] = searchDocsRequest.Query;
            if (searchDocsRequest.Limit.HasValue) queryParams["limit"] = searchDocsRequest.Limit.Value.ToString();
            if (!string.IsNullOrEmpty(searchDocsRequest.Cursor)) queryParams["cursor"] = searchDocsRequest.Cursor;
            // Add other params from SearchDocsRequest like space_id, parent_id, etc.
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<SearchDocsResponse>(endpoint, cancellationToken);
            return response ?? throw new InvalidOperationException("API response was null for SearchDocsAsync.");
        }

        /// <inheritdoc />
        public async Task<Doc> CreateDocAsync(
            string workspaceId,
            CreateDocRequest createDocRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs";
            var responseWrapper = await _apiConnection.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(endpoint, createDocRequest, cancellationToken);
            return responseWrapper?.Data ?? throw new InvalidOperationException("API response or its data was null for CreateDocAsync.");
        }

        /// <inheritdoc />
        public async Task<Doc> GetDocAsync(
            string workspaceId,
            string docId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}";
            var responseWrapper = await _apiConnection.GetAsync<ClickUpV3DataResponse<Doc>>(endpoint, cancellationToken);
            return responseWrapper?.Data ?? throw new InvalidOperationException($"API response or its data was null for GetDocAsync (Doc ID: {docId}).");
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DocPageListingItem>> GetDocPageListingAsync(
            string workspaceId,
            string docId,
            int? maxPageDepth = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}/pageListing";
            var queryParams = new Dictionary<string, string?>();
            if (maxPageDepth.HasValue) queryParams["max_page_depth"] = maxPageDepth.Value.ToString();
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.GetAsync<ClickUpV3DataListResponse<DocPageListingItem>>(endpoint, cancellationToken);
            return responseWrapper?.Data ?? Enumerable.Empty<DocPageListingItem>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Page>> GetDocPagesAsync(
            string workspaceId,
            string docId,
            int? maxPageDepth = null,
            string? contentFormat = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}/pages";
            var queryParams = new Dictionary<string, string?>();
            if (maxPageDepth.HasValue) queryParams["max_page_depth"] = maxPageDepth.Value.ToString();
            if (!string.IsNullOrEmpty(contentFormat)) queryParams["content_format"] = contentFormat;
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.GetAsync<ClickUpV3DataListResponse<Page>>(endpoint, cancellationToken);
            return responseWrapper?.Data ?? Enumerable.Empty<Page>();
        }

        /// <inheritdoc />
        public async Task<Page> CreatePageAsync(
            string workspaceId,
            string docId,
            CreatePageRequest createPageRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}/pages";
            var responseWrapper = await _apiConnection.PostAsync<CreatePageRequest, ClickUpV3DataResponse<Page>>(endpoint, createPageRequest, cancellationToken);
            return responseWrapper?.Data ?? throw new InvalidOperationException("API response or its data was null for CreatePageAsync.");
        }

        /// <inheritdoc />
        public async Task<Page> GetPageAsync(
            string workspaceId,
            string docId,
            string pageId,
            string? contentFormat = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}/pages/{pageId}";
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(contentFormat)) queryParams["content_format"] = contentFormat;
            endpoint += BuildQueryString(queryParams);

            var responseWrapper = await _apiConnection.GetAsync<ClickUpV3DataResponse<Page>>(endpoint, cancellationToken);
            return responseWrapper?.Data ?? throw new InvalidOperationException($"API response or its data was null for GetPageAsync (Doc ID: {docId}, Page ID: {pageId}).");
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task EditPageAsync(
            string workspaceId,
            string docId,
            string pageId,
            EditPageRequest updatePageRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}/pages/{pageId}";
            // API returns 200 with an empty object for this PUT.
            await _apiConnection.PutAsync(endpoint, updatePageRequest, cancellationToken);
        }
    }

    // Helper DTOs assumed for v3 responses (these would ideally be in a Models/ResponseModels/Shared or similar)
    // These are conceptual and would need to be properly defined if not already.
    internal class ClickUpV3DataResponse<T> { public T? Data { get; set; } }
    internal class ClickUpV3DataListResponse<T> { public List<T>? Data { get; set; } }
}
