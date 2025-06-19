using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Docs operations in the ClickUp API (v3)
    // Based on endpoints like:
    // - GET /v3/workspaces/{workspaceId}/docs
    // - POST /v3/workspaces/{workspaceId}/docs
    // - GET /v3/workspaces/{workspaceId}/docs/{docId}
    // - GET /v3/workspaces/{workspaceId}/docs/{docId}/pageListing
    // - GET /v3/workspaces/{workspaceId}/docs/{docId}/pages
    // - POST /v3/workspaces/{workspaceId}/docs/{docId}/pages
    // - GET /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}
    // - PUT /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId} (Edit Page - was PATCH in my thought, but spec says PUT for editPage operationId)

    public interface IDocsService
    {
        /// <summary>
        /// Searches for Docs within a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="searchParams">Parameters for searching/filtering docs.</param>
        /// <returns>A list of Docs matching the search criteria.</returns>
        Task<object> SearchDocsAsync(double workspaceId, object searchParams);
        // Note: searchParams should be SearchDocsParams. Return should be a DTO with 'docs' and 'next_cursor'.

        /// <summary>
        /// Creates a new Doc in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="createDocRequest">Details for creating the Doc.</param>
        /// <returns>The created Doc.</returns>
        Task<object> CreateDocAsync(double workspaceId, object createDocRequest);
        // Note: createDocRequest should be CreateDocRequest, return type should be DocDto.

        /// <summary>
        /// Retrieves details of a specific Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <returns>Details of the Doc.</returns>
        Task<object> GetDocAsync(double workspaceId, string docId);
        // Note: Return type should be DocDto.

        /// <summary>
        /// Retrieves the PageListing for a Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="maxPageDepth">Optional. Maximum depth to retrieve pages and subpages.</param>
        /// <returns>A list of page listing items.</returns>
        Task<IEnumerable<object>> GetDocPageListingAsync(double workspaceId, string docId, double? maxPageDepth = null);
        // Note: Return type should be IEnumerable<PageListingItemDto>.

        /// <summary>
        /// Retrieves all pages within a Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="maxPageDepth">Optional. Maximum depth to retrieve pages and subpages.</param>
        /// <param name="contentFormat">Optional. Format for page content (e.g., "text/md").</param>
        /// <returns>A list of pages within the Doc.</returns>
        Task<IEnumerable<object>> GetDocPagesAsync(double workspaceId, string docId, double? maxPageDepth = null, string? contentFormat = null);
        // Note: Return type should be IEnumerable<PageDto>.

        /// <summary>
        /// Creates a new page in a Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="createPageRequest">Details for creating the page.</param>
        /// <returns>The created page.</returns>
        Task<object> CreatePageAsync(double workspaceId, string docId, object createPageRequest);
        // Note: createPageRequest should be CreatePageRequest, return type should be PageDto.

        /// <summary>
        /// Retrieves details of a specific page within a Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="pageId">The ID of the page.</param>
        /// <param name="contentFormat">Optional. Format for page content.</param>
        /// <returns>Details of the page.</returns>
        Task<object> GetPageAsync(double workspaceId, string docId, string pageId, string? contentFormat = null);
        // Note: Return type should be PageDto.

        /// <summary>
        /// Edits a page within a Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="pageId">The ID of the page to edit.</param>
        /// <param name="editPageRequest">Details for editing the page.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task EditPageAsync(double workspaceId, string docId, string pageId, object editPageRequest);
        // Note: editPageRequest should be EditPageRequest. API returns 200 with an empty object.
    }
}
