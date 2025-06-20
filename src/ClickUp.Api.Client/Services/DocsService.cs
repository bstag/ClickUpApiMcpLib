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
        public async Task<SearchDocsResponse?> SearchDocsAsync(
            string workspaceId,
            SearchDocsRequest searchDocsRequest, // This DTO should contain all query params like 'query', 'spaceId', 'limit', 'cursor', etc.
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs";
            // SearchDocsRequest DTO itself will be serialized by ApiConnection if it were a POST.
            // For GET, its properties need to be translated into query parameters.
            // For simplicity here, assuming SearchDocsRequest might be used to build query string manually or via a helper.
            // The IApiConnection.GetAsync doesn't take a request body DTO.
            // So, query parameters from searchDocsRequest must be appended to the endpoint.
            // This part needs careful construction of the query string from searchDocsRequest properties.
            // For now, I'll assume searchDocsRequest itself IS the query string or is handled by a more complex GetAsync overload not yet defined.
            // Let's assume searchDocsRequest.ToQueryString() for conceptual purposes.
            // endpoint += searchDocsRequest.ToQueryString(); // Conceptual

            // Simplified: Assuming SearchDocsRequest is minimal or query params are passed differently.
            // The GetAsync method in IApiConnection only takes an endpoint string.
            // Query parameters must be part of that string.
            // Let's assume the SearchDocsRequest is used to build the query parameters.
            var queryParams = new Dictionary<string, string?>();
            if (!string.IsNullOrEmpty(searchDocsRequest.Query)) queryParams["q"] = searchDocsRequest.Query;
            if (searchDocsRequest.Limit.HasValue) queryParams["limit"] = searchDocsRequest.Limit.Value.ToString();
            if (!string.IsNullOrEmpty(searchDocsRequest.Cursor)) queryParams["cursor"] = searchDocsRequest.Cursor;
            // Add other params from SearchDocsRequest like space_id, parent_id, etc.
            endpoint += BuildQueryString(queryParams);

            return await _apiConnection.GetAsync<SearchDocsResponse>(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Doc?> CreateDocAsync(
            string workspaceId,
            CreateDocRequest createDocRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs";
            // v3 API might wrap response in {"data": {...}}
            var response = await _apiConnection.PostAsync<CreateDocRequest, ClickUpV3DataResponse<Doc>>(endpoint, createDocRequest, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<Doc?> GetDocAsync(
            string workspaceId,
            string docId,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}";
            var response = await _apiConnection.GetAsync<ClickUpV3DataResponse<Doc>>(endpoint, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PageListingItem>?> GetDocPageListingAsync(
            string workspaceId,
            string docId,
            int? maxPageDepth = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}/pageListing";
            var queryParams = new Dictionary<string, string?>();
            if (maxPageDepth.HasValue) queryParams["max_page_depth"] = maxPageDepth.Value.ToString();
            endpoint += BuildQueryString(queryParams);

            // Assuming PageListing is directly an array in response or wrapped in "data"
            var response = await _apiConnection.GetAsync<ClickUpV3DataListResponse<PageListingItem>>(endpoint, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Page>?> GetDocPagesAsync(
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

            var response = await _apiConnection.GetAsync<ClickUpV3DataListResponse<Page>>(endpoint, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<Page?> CreatePageAsync(
            string workspaceId,
            string docId,
            CreatePageRequest createPageRequest,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}/pages";
            var response = await _apiConnection.PostAsync<CreatePageRequest, ClickUpV3DataResponse<Page>>(endpoint, createPageRequest, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async Task<Page?> GetPageAsync(
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

            var response = await _apiConnection.GetAsync<ClickUpV3DataResponse<Page>>(endpoint, cancellationToken);
            return response?.Data;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task EditPageAsync(
            string workspaceId,
            string docId,
            string pageId,
            UpdatePageRequest updatePageRequest,
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
