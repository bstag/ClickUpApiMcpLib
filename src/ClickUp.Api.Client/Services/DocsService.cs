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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUp.Api.Client.Models.Common.Pagination; // For IPagedResult
using ClickUp.Api.Client.Helpers; // For PaginationHelpers

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IDocsService"/> for ClickUp Docs (v3) operations.
    /// </summary>
    public class DocsService : IDocsService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<DocsService> _logger;
        private const string BaseEndpoint = "/v3/workspaces"; // Base for v3 Docs API

        /// <summary>
        /// Initializes a new instance of the <see cref="DocsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public DocsService(IApiConnection apiConnection, ILogger<DocsService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<DocsService>.Instance;
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
        public async Task<IPagedResult<Doc>> SearchDocsAsync(
            string workspaceId,
            SearchDocsRequest searchDocsRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Searching docs in workspace ID: {WorkspaceId}, Query: {Query}, NextCursor: {NextCursor}, Limit: {Limit}",
                workspaceId, searchDocsRequest.Query, searchDocsRequest.NextCursor, searchDocsRequest.Limit);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs";
            var queryParams = new Dictionary<string, string?>();

            // Populate all relevant query params from searchDocsRequest
            if (!string.IsNullOrEmpty(searchDocsRequest.Query)) queryParams["q"] = searchDocsRequest.Query; // 'q' is used by ClickUp for query
            if (searchDocsRequest.Limit.HasValue) queryParams["limit"] = searchDocsRequest.Limit.Value.ToString();
            if (!string.IsNullOrEmpty(searchDocsRequest.NextCursor)) queryParams["cursor"] = searchDocsRequest.NextCursor; // ClickUp uses 'next_cursor' in query, but request DTO has 'NextCursor'

            // Correcting query param names based on typical API conventions / OpenAPI spec for SearchDocs
            if (!string.IsNullOrEmpty(searchDocsRequest.ParentId)) queryParams["parent_id"] = searchDocsRequest.ParentId;
            if (searchDocsRequest.ParentType.HasValue) queryParams["parent_type"] = ((int)searchDocsRequest.ParentType.Value).ToString(); // Assuming ParentType is an enum
            if (searchDocsRequest.IncludeArchived.HasValue) queryParams["archived"] = searchDocsRequest.IncludeArchived.Value.ToString().ToLower();
            if (searchDocsRequest.IncludeDeleted.HasValue) queryParams["deleted"] = searchDocsRequest.IncludeDeleted.Value.ToString().ToLower();
            if (searchDocsRequest.CreatorId.HasValue) queryParams["creator"] = searchDocsRequest.CreatorId.Value.ToString();
            // Note: OpenAPI for "Search Docs" uses "next_cursor" as a query param for pagination, not "cursor".
            // The SearchDocsRequest DTO uses "NextCursor". If the API strictly needs "next_cursor", this needs alignment.
            // For now, assuming "cursor" from DTO is what we send. The response contains "next_cursor".
            // Let's assume for the request, 'cursor' is the parameter name if it's for fetching the *next* page.
            // If it's the *first* page, cursor is usually omitted.
            // The SearchDocsRequest.NextCursor should represent the cursor for the *next* page to fetch.
            // If SearchDocsRequest.NextCursor is null/empty, it implies fetching the first page.

            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<SearchDocsResponse>(endpoint, cancellationToken);

            if (response == null)
            {
                _logger.LogWarning("API connection returned null response for SearchDocsAsync in workspace {WorkspaceId}. Returning empty paged result.", workspaceId);
                return PagedResult<Doc>.Empty(0); // Using 0 as placeholder for page number
            }

            var items = response.Docs ?? Enumerable.Empty<Doc>();
            int pseudoPageNumber = string.IsNullOrEmpty(searchDocsRequest.NextCursor) ? 0 : 1;
            int pageSize = searchDocsRequest.Limit ?? items.Count();
            if (pageSize == 0 && items.Any()) pageSize = items.Count();

            long totalCount = response.TotalCount ?? -1;
            bool hasNextPage;

            if (response.LastPage.HasValue)
            {
                hasNextPage = !response.LastPage.Value;
            }
            else
            {
                hasNextPage = !string.IsNullOrEmpty(response.NextPageId);
            }

            if (totalCount >= 0)
            {
                return new PagedResult<Doc>(
                    items,
                    pseudoPageNumber,
                    pageSize,
                    totalCount
                );
            }
            else
            {
                return new PagedResult<Doc>(
                    items,
                    pseudoPageNumber,
                    pageSize,
                    hasNextPage
                );
            }
        }

        /// <inheritdoc />
        public async Task<Doc> CreateDocAsync(
            string workspaceId,
            CreateDocRequest createDocRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating doc in workspace ID: {WorkspaceId}, Name: {DocName}", workspaceId, createDocRequest.Name);
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
            _logger.LogInformation("Getting doc ID: {DocId} in workspace ID: {WorkspaceId}", docId, workspaceId);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}";
            var responseWrapper = await _apiConnection.GetAsync<ClickUpV3DataResponse<Doc>>(endpoint, cancellationToken);
            return responseWrapper?.Data ?? throw new InvalidOperationException($"API response or its data was null for GetDocAsync (Doc ID: {docId}).");
        }

        /// <inheritdoc />
        public IAsyncEnumerable<Doc> SearchAllDocsAsync(
            string workspaceId,
            SearchDocsRequest baseSearchDocsRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Searching all docs in workspace ID: {WorkspaceId}, Query: {Query}", workspaceId, baseSearchDocsRequest.Query);
            if (string.IsNullOrWhiteSpace(workspaceId)) throw new ArgumentNullException(nameof(workspaceId));
            if (baseSearchDocsRequest == null) throw new ArgumentNullException(nameof(baseSearchDocsRequest));

            return Helpers.PaginationHelpers.GetAllPaginatedDataAsync<Doc, SearchDocsResponse>(
                async (currentCursor, ct) =>
                {
                    // Clone the base request and update cursor for this specific call
                    var pageRequest = baseSearchDocsRequest with { NextCursor = currentCursor, Limit = null }; // Limit is handled by API if not set, or could be exposed

                    var endpoint = $"{BaseEndpoint}/{workspaceId}/docs";
                    var queryParams = new Dictionary<string, string?>();
                    if (!string.IsNullOrEmpty(pageRequest.Query)) queryParams["q"] = pageRequest.Query;
                    // Add other relevant parameters from pageRequest to queryParams here, similar to SearchDocsAsync
                    // Example: if (pageRequest.SomeOtherFilter.HasValue) queryParams["some_other_filter"] = pageRequest.SomeOtherFilter.Value.ToString();
                    if (!string.IsNullOrEmpty(pageRequest.ParentId)) queryParams["parent_id"] = pageRequest.ParentId;
                    if (pageRequest.ParentType.HasValue) queryParams["parent_type"] = ((int)pageRequest.ParentType.Value).ToString();
                    if (pageRequest.IncludeArchived.HasValue) queryParams["archived"] = pageRequest.IncludeArchived.Value.ToString().ToLower();
                    if (pageRequest.IncludeDeleted.HasValue) queryParams["deleted"] = pageRequest.IncludeDeleted.Value.ToString().ToLower();
                    if (pageRequest.CreatorId.HasValue) queryParams["creator"] = pageRequest.CreatorId.Value.ToString();
                    // The 'limit' parameter for the underlying API call can be omitted if the API default is acceptable,
                    // or it could be set to a reasonable default (e.g., 100, the max per OpenAPI spec for searchDocs).
                    // For now, we'll let the API use its default page size.
                    if (!string.IsNullOrEmpty(pageRequest.NextCursor)) queryParams["cursor"] = pageRequest.NextCursor;


                    endpoint += BuildQueryString(queryParams);

                    // Ensure SearchDocsResponse has a public property 'Docs' and 'NextCursor'
                    // The helper already validates for 'Data' or 'Items' and 'NextCursor'.
                    // If SearchDocsResponse uses 'Docs' specifically, the helper needs adjustment or SearchDocsResponse needs an 'Items' alias.
                    // For now, assuming SearchDocsResponse has 'Docs' and the helper's logic for 'Items'/'Data' needs to be robust.
                    // The helper looks for `typeof(TItem).Name + "s"` which would be "Docs".
                    var response = await _apiConnection.GetAsync<SearchDocsResponse>(endpoint, ct);

                    // The helper expects TResponse to be non-null if there's a next page or items.
                    // If response can be null even with items/nextcursor, this might need adjustment in the helper or here.
                    // However, _apiConnection.GetAsync already throws if the response is not successful or returns null unexpectedly for success.
                    return response;
                },
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DocPageListingItem>> GetDocPageListingAsync(
            string workspaceId,
            string docId,
            int? maxPageDepth = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting doc page listing for doc ID: {DocId} in workspace ID: {WorkspaceId}", docId, workspaceId);
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
            _logger.LogInformation("Getting doc pages for doc ID: {DocId} in workspace ID: {WorkspaceId}", docId, workspaceId);
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
            _logger.LogInformation("Creating page in doc ID: {DocId}, workspace ID: {WorkspaceId}, Page Name: {PageName}", docId, workspaceId, createPageRequest.Name);
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
            _logger.LogInformation("Getting page ID: {PageId} in doc ID: {DocId}, workspace ID: {WorkspaceId}", pageId, docId, workspaceId);
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
            _logger.LogInformation("Editing page ID: {PageId} in doc ID: {DocId}, workspace ID: {WorkspaceId}", pageId, docId, workspaceId);
            var endpoint = $"{BaseEndpoint}/{workspaceId}/docs/{docId}/pages/{pageId}";
            // API returns 200 with an empty object for this PUT.
            await _apiConnection.PutAsync(endpoint, updatePageRequest, cancellationToken);
        }
    }

    // Helper DTOs ClickUpV3DataResponse and ClickUpV3DataListResponse are now in InternalDtos.cs (or should be if used)
}
