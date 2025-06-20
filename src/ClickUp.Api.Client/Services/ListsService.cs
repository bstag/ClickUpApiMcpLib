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
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.RequestModels.Lists;
using ClickUp.Api.Client.Models.ResponseModels.Lists; // Assuming GetListsResponse exists

namespace ClickUp.Api.Client.Services
{
    /// <summary>
    /// Implements <see cref="IListsService"/> for ClickUp List operations.
    /// </summary>
    public class ListsService : IListsService
    {
        private readonly IApiConnection _apiConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListsService"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection to use for making requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiConnection is null.</exception>
        public ListsService(IApiConnection apiConnection)
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
        public async Task<IEnumerable<ClickUpList>> GetListsInFolderAsync(
            string folderId,
            bool? archived = null,
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"folder/{folderId}/list";
            var queryParams = new Dictionary<string, string?>();
            if (archived.HasValue) queryParams["archived"] = archived.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetListsResponse>(endpoint, cancellationToken); // API returns {"lists": [...]}
            return response?.Lists?.Select(list => new ClickUpList
            {
                Id = list.Id,
                Name = list.Name,
                Folder = list.Folder,
                Priority = list.Priority
            }) ?? Enumerable.Empty<ClickUpList>();
        }

        /// <inheritdoc />
        public async Task<ClickUpList> CreateListInFolderAsync(
            string folderId,
            CreateListRequest createListRequest,
            CancellationToken cancellationToken = default)
        {
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
            var endpoint = $"space/{spaceId}/list"; // Folderless lists are directly under a space
            var queryParams = new Dictionary<string, string?>();
            if (archived.HasValue) queryParams["archived"] = archived.Value.ToString().ToLower();
            endpoint += BuildQueryString(queryParams);

            var response = await _apiConnection.GetAsync<GetListsResponse>(endpoint, cancellationToken); // API returns {"lists": [...]}
            return response?.Lists?.Select(list => new ClickUpList
            {
                Id = list.Id,
                Name = list.Name,
                Folder = list.Folder,
                Priority = list.Priority
            }) ?? Enumerable.Empty<ClickUpList>();
        }

        /// <inheritdoc />
        public async Task<ClickUpList> CreateFolderlessListAsync(
            string spaceId,
            CreateListRequest createListRequest,
            CancellationToken cancellationToken = default)
        {
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
            var endpoint = $"list/{listId}";
            await _apiConnection.DeleteAsync(endpoint, cancellationToken);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task AddTaskToListAsync(
            string listId,
            string taskId,
            CancellationToken cancellationToken = default)
        {
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
            int currentPage = 0;
            bool lastPageReached;

            do
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                // Note: The original GetFolderlessListsAsync in ListsService already calls
                // _apiConnection.GetAsync<GetListsResponse>(endpoint, cancellationToken);
                // and returns response?.Lists. We need to call that underlying mechanism or replicate it.
                // For IAsyncEnumerable, we directly call the paged version of GetFolderlessListsAsync
                // which takes 'page' as a parameter.
                // The public IListsService.GetFolderlessListsAsync takes 'page' and 'archived'.
                // However, the service implementation for GetFolderlessListsAsync itself will use _apiConnection.
                // This async enumerable will call the *existing* GetFolderlessListsAsync that takes a page param.
                // Let's refine this: the existing GetFolderlessListsAsync method should be the one that takes a page number.
                // If it doesn't (i.e., if the interface method doesn't take page), we need to adjust.
                // Looking at IListsService, GetFolderlessListsAsync(string spaceId, bool? archived, CancellationToken) does NOT take page.
                // This means the *implementation* of GetFolderlessListsAsync in ListsService.cs must be what we call,
                // and that implementation itself must be calling the API with a page.
                // This is a bit circular. The `GetFolderlessListsAsync` on the service already returns `IEnumerable<ClickUpList>`.
                // The paged version that `IAsyncEnumerable` should call is essentially the core logic.
                // The current `GetFolderlessListsAsync` in `ListsService.cs` already fetches what seems like ALL lists.
                // This implies the `page` parameter was not actually implemented in the service method for `GetFolderlessListsAsync`.
                // Let's assume the ClickUp API for /space/{space_id}/list DOES support a page query param.
                // We will construct the call to _apiConnection here directly.

                var endpoint = $"space/{spaceId}/list";
                var queryParams = new Dictionary<string, string?>();
                if (archived.HasValue) queryParams["archived"] = archived.Value.ToString().ToLower();
                queryParams["page"] = currentPage.ToString();
                endpoint += BuildQueryString(queryParams);

                var response = await _apiConnection.GetAsync<GetListsResponse>(endpoint, cancellationToken).ConfigureAwait(false);

                if (response?.Lists != null && response.Lists.Any())
                {
                    foreach (var list in response.Lists)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            yield break;
                        }
                        yield return list;
                    }
                    // We infer lastPage if the returned list is empty.
                    // ClickUp's /space/{id}/list endpoint with 'page' might not return a 'last_page' field.
                    lastPageReached = !response.Lists.Any();
                    // A more robust way if the API guarantees to return less than requested items only on the last page:
                    // if (response.Lists.Count < some_page_size_if_known_and_fixed) lastPageReached = true;
                }
                else
                {
                    lastPageReached = true;
                }

                if (!lastPageReached)
                {
                    currentPage++;
                }
            } while (!lastPageReached);
        }
    }
}
