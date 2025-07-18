using System;
using System.Collections.Generic;
using System.Linq; // For Linq Any
using System.Net.Http;
using System.Text; // For StringBuilder
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Http; // IApiConnection
using ClickUp.Api.Client.Abstractions.Services;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.RequestModels.Lists;
using ClickUp.Api.Client.Models.ResponseModels.Lists;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUp.Api.Client.Helpers;

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IListsService"/> for ClickUp List operations.
    /// </summary>
    public class ListsService : IListsService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<ListsService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <param name="logger">The logger for this service.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection or logger is null.</exception>
        public ListsService(IApiConnection apiConnection, ILogger<ListsService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? NullLogger<ListsService>.Instance;
        }


        /// <inheritdoc />
        public async Task<IEnumerable<ClickUpList>> GetListsInFolderAsync(
            string folderId,
            bool? archived = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting lists in folder ID: {FolderId}, Archived: {Archived}", folderId, archived);
            var endpoint = $"folder/{folderId}/list";
            var queryParams = new Dictionary<string, string?>();
            if (archived.HasValue) queryParams["archived"] = archived.Value.ToString().ToLower();
            endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetListsResponse>(endpoint, cancellationToken); // API returns {"lists": [...]}
            return response?.Lists ?? Enumerable.Empty<ClickUpList>();
        }

        /// <inheritdoc />
        public async Task<ClickUpList> CreateListInFolderAsync(
            string folderId,
            CreateListRequest createListRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating list in folder ID: {FolderId} with name: {ListName}", folderId, createListRequest.Name);
            var endpoint = $"folder/{folderId}/list";
            var list = await _apiConnection.PostAsync<CreateListRequest, ClickUpList>(endpoint, createListRequest, cancellationToken);
            if (list == null)
            {
                throw new InvalidOperationException($"API connection returned null response when creating list in folder {folderId}.");
            }
            return list;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ClickUpList>> GetFolderlessListsAsync(
            string spaceId,
            bool? archived = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting folderless lists in space ID: {SpaceId}, Archived: {Archived}", spaceId, archived);
            var endpoint = $"space/{spaceId}/list"; // Folderless lists are directly under a space
            var queryParams = new Dictionary<string, string?>();
            if (archived.HasValue) queryParams["archived"] = archived.Value.ToString().ToLower();
            endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetListsResponse>(endpoint, cancellationToken); // API returns {"lists": [...]}
            return response?.Lists ?? Enumerable.Empty<ClickUpList>();
        }

        /// <inheritdoc />
        public async Task<ClickUpList> CreateFolderlessListAsync(
            string spaceId,
            CreateListRequest createListRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating folderless list in space ID: {SpaceId} with name: {ListName}", spaceId, createListRequest.Name);
            var endpoint = $"space/{spaceId}/list";
            var list = await _apiConnection.PostAsync<CreateListRequest, ClickUpList>(endpoint, createListRequest, cancellationToken);
            if (list == null)
            {
                throw new InvalidOperationException($"API connection returned null response when creating folderless list in space {spaceId}.");
            }
            return list;
        }

        /// <inheritdoc />
        public async Task<ClickUpList> GetListAsync(
            string listId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting list with ID: {ListId}", listId);
            var endpoint = $"list/{listId}";
            var list = await _apiConnection.GetAsync<ClickUpList>(endpoint, cancellationToken);
            if (list == null)
            {
                throw new InvalidOperationException($"API connection returned null response when getting list {listId}.");
            }
            return list;
        }

        /// <inheritdoc />
        public async Task<ClickUpList> UpdateListAsync(
            string listId,
            UpdateListRequest updateListRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating list with ID: {ListId}", listId);
            var endpoint = $"list/{listId}";
            var list = await _apiConnection.PutAsync<UpdateListRequest, ClickUpList>(endpoint, updateListRequest, cancellationToken);
            if (list == null)
            {
                throw new InvalidOperationException($"API connection returned null response when updating list {listId}.");
            }
            return list;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteListAsync(
            string listId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting list with ID: {ListId}", listId);
            var endpoint = $"list/{listId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task AddTaskToListAsync(
            string listId,
            string taskId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding task ID: {TaskId} to list ID: {ListId}", taskId, listId);
            var endpoint = $"list/{listId}/task/{taskId}";
            // This POST request does not have a body.
            await _apiConnection.PostAsync<object>(endpoint, new object(), cancellationToken); // Sending an empty object as payload
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task RemoveTaskFromListAsync(
            string listId,
            string taskId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing task ID: {TaskId} from list ID: {ListId}", taskId, listId);
            var endpoint = $"list/{listId}/task/{taskId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ClickUpList> CreateListFromTemplateInFolderAsync(
            string folderId,
            string templateId,
            CreateListFromTemplateRequest createListFromTemplateRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating list in folder ID: {FolderId} from template ID: {TemplateId}", folderId, templateId);
            var endpoint = $"folder/{folderId}/listTemplate/{templateId}"; // Corrected from list_template to listTemplate
            var list = await _apiConnection.PostAsync<CreateListFromTemplateRequest, ClickUpList>(endpoint, createListFromTemplateRequest, cancellationToken);
            if (list == null)
            {
                throw new InvalidOperationException($"API connection returned null response when creating list from template {templateId} in folder {folderId}.");
            }
            return list;
        }

        /// <inheritdoc />
        public async Task<ClickUpList> CreateListFromTemplateInSpaceAsync(
            string spaceId,
            string templateId,
            CreateListFromTemplateRequest createListFromTemplateRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating folderless list in space ID: {SpaceId} from template ID: {TemplateId}", spaceId, templateId);
            var endpoint = $"space/{spaceId}/listTemplate/{templateId}"; // Corrected from list_template to listTemplate
            var list = await _apiConnection.PostAsync<CreateListFromTemplateRequest, ClickUpList>(endpoint, createListFromTemplateRequest, cancellationToken);
            if (list == null)
            {
                throw new InvalidOperationException($"API connection returned null response when creating list from template {templateId} in space {spaceId}.");
            }
            return list;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ClickUpList> GetFolderlessListsAsyncEnumerableAsync(
            string spaceId,
            bool? archived = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting folderless lists as async enumerable for space ID: {SpaceId}, Archived: {Archived}", spaceId, archived);
            int currentPage = 0;
            bool lastPageReached;

            do
            {
                cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation at the start of each iteration

                _logger.LogDebug("Fetching page {PageNumber} for folderless lists in space ID {SpaceId} via async enumerable.", currentPage, spaceId);

                var endpoint = $"space/{spaceId}/list";
                var queryParams = new Dictionary<string, string?>();
                if (archived.HasValue) queryParams["archived"] = archived.Value.ToString().ToLower();
                queryParams["page"] = currentPage.ToString();
                endpoint += UrlBuilderHelper.BuildQueryString(queryParams);

                var response = await _apiConnection.GetAsync<GetListsResponse>(endpoint, cancellationToken).ConfigureAwait(false);

                if (response?.Lists != null && response.Lists.Any())
                {
                    foreach (var list in response.Lists)
                    {
                        cancellationToken.ThrowIfCancellationRequested(); // Check before yielding each item
                        yield return list;
                    }
                    lastPageReached = !response.Lists.Any(); // This logic remains: if we got items, there might be more
                }
                else
                {
                    lastPageReached = true; // No items or null response means we are done
                }

                if (!lastPageReached)
                {
                    currentPage++;
                }
            } while (!lastPageReached);
        }
    }
}
