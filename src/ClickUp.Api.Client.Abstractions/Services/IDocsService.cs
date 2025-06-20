﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Docs; // Assuming Doc, Page, PageListingItem DTOs are here
using ClickUp.Api.Client.Models.RequestModels.Docs; // Assuming Request DTOs are here
using ClickUp.Api.Client.Models.ResponseModels.Docs; // Assuming Response DTOs like SearchDocsResponse are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Docs operations in the ClickUp API (v3).
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v3/workspaces/{workspaceId}/docs
    /// - POST /v3/workspaces/{workspaceId}/docs
    /// - GET /v3/workspaces/{workspaceId}/docs/{docId}
    /// - GET /v3/workspaces/{workspaceId}/docs/{docId}/pageListing
    /// - GET /v3/workspaces/{workspaceId}/docs/{docId}/pages
    /// - POST /v3/workspaces/{workspaceId}/docs/{docId}/pages
    /// - GET /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}
    /// - PUT /v3/workspaces/{workspaceId}/docs/{docId}/pages/{pageId}
    /// </remarks>
    public interface IDocsService
    {
        /// <summary>
        /// Searches for Docs within a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="searchDocsRequest">Parameters for searching/filtering docs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="SearchDocsResponse"/> object containing a list of Docs and pagination cursor.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="searchDocsRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to search docs in this workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<SearchDocsResponse> SearchDocsAsync(
            string workspaceId,
            SearchDocsRequest searchDocsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Doc in a Workspace.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="createDocRequest">Details for creating the Doc.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="Doc"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="createDocRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create docs in this workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Doc> CreateDocAsync(
            string workspaceId,
            CreateDocRequest createDocRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves details of a specific Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="Doc"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="docId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the doc with the specified ID is not found in the workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this doc.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Doc> GetDocAsync(
            string workspaceId,
            string docId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the PageListing for a Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="maxPageDepth">Optional. Maximum depth to retrieve pages and subpages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="DocPageListingItem"/> objects.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="docId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the doc with the specified ID is not found in the workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this doc's page listing.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<IEnumerable<DocPageListingItem>> GetDocPageListingAsync(
            string workspaceId,
            string docId,
            int? maxPageDepth = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all pages within a Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="maxPageDepth">Optional. Maximum depth to retrieve pages and subpages.</param>
        /// <param name="contentFormat">Optional. Format for page content (e.g., "text/md", "application/json").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Page"/> objects within the Doc.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="docId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the doc with the specified ID is not found in the workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access pages for this doc.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<IEnumerable<Page>> GetDocPagesAsync(
            string workspaceId,
            string docId,
            int? maxPageDepth = null,
            string? contentFormat = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new page in a Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="createPageRequest">Details for creating the page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="Page"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="docId"/>, or <paramref name="createPageRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the doc with the specified ID is not found in the workspace.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create pages in this doc.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Page> CreatePageAsync(
            string workspaceId,
            string docId,
            CreatePageRequest createPageRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves details of a specific page within a Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="pageId">The ID of the page.</param>
        /// <param name="contentFormat">Optional. Format for page content.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="Page"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="docId"/>, or <paramref name="pageId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the doc or page with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this page.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Page> GetPageAsync(
            string workspaceId,
            string docId,
            string pageId,
            string? contentFormat = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Edits a page within a Doc.
        /// </summary>
        /// <param name="workspaceId">The ID of the Workspace.</param>
        /// <param name="docId">The ID of the Doc.</param>
        /// <param name="pageId">The ID of the page to edit.</param>
        /// <param name="updatePageRequest">Details for editing the page using <see cref="EditPageRequest"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/>, <paramref name="docId"/>, <paramref name="pageId"/>, or <paramref name="updatePageRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the doc or page with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to edit this page.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task EditPageAsync(
            string workspaceId,
            string docId,
            string pageId,
            EditPageRequest updatePageRequest,
            CancellationToken cancellationToken = default);
    }
}
