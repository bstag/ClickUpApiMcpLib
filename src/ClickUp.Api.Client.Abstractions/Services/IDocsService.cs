using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs;
using ClickUp.Api.Client.Models.ResponseModels.Docs;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Docs operations (v3 API).
    /// </summary>
    /// <remarks>
    /// This service provides methods for managing Docs and their Pages within a Workspace.
    /// It allows for searching, creating, retrieving, and editing Docs and Pages.
    /// Note: The Docs API is part of ClickUp's v3 API and may be subject to changes.
    /// Covered API Endpoints (non-exhaustive list):
    /// - Docs: `GET /v3/workspaces/{workspaceId}/docs`, `POST /v3/workspaces/{workspaceId}/docs`, `GET /v3/workspaces/{workspaceId}/docs/{docId}`
    /// - Pages: `GET /v3/workspaces/{workspaceId}/docs/{docId}/pageListing`, `GET /v3/workspaces/{workspaceId}/docs/{docId}/pages`, `POST /v3/workspaces/{workspaceId}/docs/{docId}/pages`, `GET /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}`, `PUT /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}`
    /// </remarks>
    public interface IDocsService
    {
        /// <summary>
        /// Searches for Docs within a specified Workspace based on provided criteria.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace in which to search for Docs.</param>
        /// <param name="searchDocsRequest">An object containing parameters for searching and filtering Docs, such as search query, parent ID, location type, etc.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="SearchDocsResponse"/> object, which includes a list of matching <see cref="Doc"/> objects and pagination information (cursor).</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="searchDocsRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to search for Docs in this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<SearchDocsResponse> SearchDocsAsync(
            string workspaceId,
            SearchDocsRequest searchDocsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously searches for all Docs within a specified Workspace that match the provided criteria,
        /// automatically handling pagination to retrieve all results.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace in which to search for Docs.</param>
        /// <param name="baseSearchDocsRequest">
        /// An object containing parameters for searching and filtering Docs, such as search query, parent ID, location type, etc.
        /// The 'Cursor' and 'Limit' properties of this request object will be ignored and managed by the helper.
        /// </param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Doc"/> objects, allowing asynchronous iteration over all matching Docs from all pages.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="baseSearchDocsRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to search for Docs in this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        IAsyncEnumerable<Doc> SearchAllDocsAsync(
            string workspaceId,
            SearchDocsRequest baseSearchDocsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Doc within a specified Workspace.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace where the Doc will be created.</param>
        /// <param name="createDocRequest">An object containing the details for the new Doc, such as its name, parent ID, and location type.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Doc"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createDocRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create Docs in this Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Doc> CreateDocAsync(
            string workspaceId,
            CreateDocRequest createDocRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details of a specific Doc by its ID within a Workspace.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace where the Doc resides.</param>
        /// <param name="docId">The unique identifier of the Doc to retrieve.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the details of the requested <see cref="Doc"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="docId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Doc with the specified ID does not exist in the Workspace or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this Doc.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Doc> GetDocAsync(
            string workspaceId,
            string docId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the hierarchical listing of pages (PageListing) for a specific Doc.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="docId">The unique identifier of the Doc for which to retrieve the page listing.</param>
        /// <param name="maxPageDepth">Optional. The maximum depth of subpages to retrieve in the listing. If not specified, a default depth may be used by the API.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="DocPageListingItem"/> objects representing the page structure.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="docId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Doc with the specified ID does not exist in the Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access the page listing for this Doc.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<DocPageListingItem>> GetDocPageListingAsync(
            string workspaceId,
            string docId,
            int? maxPageDepth = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all pages contained within a specific Doc, optionally up to a maximum depth and in a specified content format.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="docId">The unique identifier of the Doc from which to retrieve pages.</param>
        /// <param name="maxPageDepth">Optional. The maximum depth of subpages to retrieve. If not specified, a default depth may be used by the API.</param>
        /// <param name="contentFormat">Optional. The desired format for the content of the pages (e.g., "text/md", "application/json").</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Page"/> objects from the Doc.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="docId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Doc with the specified ID does not exist in the Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access pages for this Doc.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<Page>> GetDocPagesAsync(
            string workspaceId,
            string docId,
            int? maxPageDepth = null,
            string? contentFormat = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new page within an existing Doc.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="docId">The unique identifier of the Doc in which the new page will be created.</param>
        /// <param name="createPageRequest">An object containing the details for the new page, such as its title, content, and parent page ID (if it's a subpage).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Page"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="docId"/>, or <paramref name="createPageRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Doc with the specified ID does not exist in the Workspace.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create pages in this Doc.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Page> CreatePageAsync(
            string workspaceId,
            string docId,
            CreatePageRequest createPageRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details of a specific page within a Doc.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="docId">The unique identifier of the Doc containing the page.</param>
        /// <param name="pageId">The unique identifier of the page to retrieve.</param>
        /// <param name="contentFormat">Optional. The desired format for the page content.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the details of the requested <see cref="Page"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="docId"/>, or <paramref name="pageId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Doc or Page with the specified IDs does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this page.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Page> GetPageAsync(
            string workspaceId,
            string docId,
            string pageId,
            string? contentFormat = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Edits the content or properties of an existing page within a Doc.
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace.</param>
        /// <param name="docId">The unique identifier of the Doc containing the page to be edited.</param>
        /// <param name="pageId">The unique identifier of the page to edit.</param>
        /// <param name="updatePageRequest">An object containing the properties to update for the page, such as its title, content, or order. Uses <see cref="EditPageRequest"/>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="docId"/>, <paramref name="pageId"/>, or <paramref name="updatePageRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Doc or Page with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to edit this page.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task EditPageAsync(
            string workspaceId,
            string docId,
            string pageId,
            EditPageRequest updatePageRequest,
            CancellationToken cancellationToken = default);
    }
}
