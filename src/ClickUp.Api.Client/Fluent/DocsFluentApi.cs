using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models.Entities.Docs;
using ClickUp.Api.Client.Models.RequestModels.Docs; // Contains SearchDocsRequest and LocationType
using ClickUp.Api.Client.Models.ResponseModels.Docs;
using System.Collections.Generic;
using System.Linq; // For .ToList()
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent;

public class DocsFluentApi
{
    private readonly IDocsService _docsService;

    public DocsFluentApi(IDocsService docsService)
    {
        _docsService = docsService;
    }

    public SearchDocsFluentRequest SearchDocs(string workspaceId,
        string? query = null,
        List<string>? spaceIds = null,
        List<string>? folderIds = null,
        List<string>? listIds = null,
        List<string>? taskIds = null,
        bool? includeArchived = null,
        int? limit = null,
        string? cursor = null,
        string? parentId = null,
        int? parentType = null,
        bool? includeDeleted = null,
        int? creatorId = null)
    {
        return new SearchDocsFluentRequest(workspaceId, _docsService);
    }

    public DocFluentCreateRequest CreateDoc(string workspaceId, string name)
    {
        return new DocFluentCreateRequest(workspaceId, _docsService, name);
    }

    public async Task<Doc> GetDocAsync(string workspaceId, string docId, CancellationToken cancellationToken = default)
    {
        return await _docsService.GetDocAsync(workspaceId, docId, cancellationToken);
    }

    public async Task<IEnumerable<DocPageListingItem>> GetDocPageListingAsync(string workspaceId, string docId, int? maxPageDepth = null, CancellationToken cancellationToken = default)
    {
        return await _docsService.GetDocPageListingAsync(workspaceId, docId, maxPageDepth, cancellationToken);
    }

    public async Task<IEnumerable<Page>> GetDocPagesAsync(string workspaceId, string docId, int? maxPageDepth = null, string? contentFormat = null, CancellationToken cancellationToken = default)
    {
        return await _docsService.GetDocPagesAsync(workspaceId, docId, maxPageDepth, contentFormat, cancellationToken);
    }

    public PageFluentCreateRequest CreatePage(string workspaceId, string docId, string name, string content, string contentFormat)
    {
        return new PageFluentCreateRequest(workspaceId, docId, _docsService, name, content, contentFormat);
    }

    public async Task<Page> GetPageAsync(string workspaceId, string docId, string pageId, string? contentFormat = null, CancellationToken cancellationToken = default)
    {
        return await _docsService.GetPageAsync(workspaceId, docId, pageId, contentFormat, cancellationToken);
    }

    public PageFluentEditRequest EditPage(string workspaceId, string docId, string pageId)
    {
        return new PageFluentEditRequest(workspaceId, docId, pageId, _docsService);
    }

    /// <summary>
    /// Retrieves all docs within a workspace, handling pagination.
    /// This is a convenience method that uses the SearchAllDocsAsync capability of the service
    /// with minimal filter criteria.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace.</param>
    /// <param name="includeArchived">Optional. Whether to include archived docs. Defaults to false.</param>
    /// <param name="includeDeleted">Optional. Whether to include deleted docs. Defaults to false.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Doc"/>.</returns>
    public async IAsyncEnumerable<Doc> GetDocsAsyncEnumerableAsync(
        string workspaceId,
        bool? includeArchived = null,
        bool? includeDeleted = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var searchRequest = new SearchDocsRequest // Using the specific request model for clarity
        {
            // Query = null by default, meaning no specific text search
            IncludeArchived = includeArchived,
            IncludeDeleted = includeDeleted
            // Other filters like SpaceIds, FolderIds, etc., are null by default,
            // effectively listing all accessible docs within the workspace subject to permissions.
        };

        await foreach (var doc in _docsService.SearchAllDocsAsync(workspaceId, searchRequest, cancellationToken).WithCancellation(cancellationToken))
        {
            yield return doc;
        }
    }

    /// <summary>
    /// Searches for docs within a workspace based on specified criteria, handling pagination.
    /// </summary>
    /// <param name="workspaceId">The ID of the workspace.</param>
    /// <param name="query">The search query string.</param>
    /// <param name="spaceIds">Optional list of Space IDs to filter by.</param>
    /// <param name="folderIds">Optional list of Folder IDs to filter by.</param>
    /// <param name="listIds">Optional list of List IDs to filter by.</param>
    /// <param name="taskIds">Optional list of Task IDs to filter by.</param>
    /// <param name="includeArchived">Optional. Whether to include archived docs.</param>
    /// <param name="parentId">Optional. Filter by parent ID (e.g., Space, Folder, List, Task ID).</param>
    /// <param name="parentType">Optional. Type of the parent ID (1: Space, 2: Folder, 3: List, 4: Task).</param>
    /// <param name="includeDeleted">Optional. Whether to include deleted docs.</param>
    /// <param name="creatorId">Optional. Filter by creator user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Doc"/>.</returns>
    public async IAsyncEnumerable<Doc> SearchDocsAsyncEnumerableAsync(
        string workspaceId,
        string? query = null,
        IEnumerable<string>? spaceIds = null,
        IEnumerable<string>? folderIds = null,
        IEnumerable<string>? listIds = null,
        IEnumerable<string>? taskIds = null,
        bool? includeArchived = null,
        string? parentId = null,
        int? parentType = null, // Consider using an enum here in the future if one is defined
        bool? includeDeleted = null,
        int? creatorId = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var searchRequest = new SearchDocsRequest
        {
            Query = query,
            SpaceIds = spaceIds?.ToList(),
            FolderIds = folderIds?.ToList(),
            ListIds = listIds?.ToList(),
            TaskIds = taskIds?.ToList(),
            IncludeArchived = includeArchived,
            ParentId = parentId,
            ParentType = parentType.HasValue ? (LocationType?)parentType.Value : null,
            IncludeDeleted = includeDeleted,
            CreatorId = creatorId
            // Cursor and Limit are handled by SearchAllDocsAsync
        };

        await foreach (var doc in _docsService.SearchAllDocsAsync(workspaceId, searchRequest, cancellationToken).WithCancellation(cancellationToken))
        {
            yield return doc;
        }
    }
}
