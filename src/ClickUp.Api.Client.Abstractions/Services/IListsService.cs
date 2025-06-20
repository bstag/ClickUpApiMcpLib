using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.Entities; // Assuming ClickUpList DTO is here
using ClickUp.Api.Client.Models.RequestModels.Lists; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Lists operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/folder/{folder_id}/list
    /// - POST /v2/folder/{folder_id}/list
    /// - GET /v2/space/{space_id}/list (Folderless)
    /// - POST /v2/space/{space_id}/list (Folderless)
    /// - GET /v2/list/{list_id}
    /// - PUT /v2/list/{list_id}
    /// - DELETE /v2/list/{list_id}
    /// - POST /v2/list/{list_id}/task/{task_id} (Add task to additional List)
    /// - DELETE /v2/list/{list_id}/task/{task_id} (Remove task from additional List)
    /// - POST /v2/folder/{folder_id}/list_template/{template_id}
    /// - POST /v2/space/{space_id}/list_template/{template_id}
    /// </remarks>
    public interface IListsService
    {
        /// <summary>
        /// Retrieves Lists within a specific Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="archived">Optional. Whether to include archived Lists.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="ClickUpList"/> objects in the Folder.</returns>
        Task<IEnumerable<ClickUpList>> GetListsInFolderAsync(
            string folderId,
            bool? archived = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new List in a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="createListRequest">Details of the List to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="ClickUpList"/>.</returns>
        Task<ClickUpList> CreateListInFolderAsync(
            string folderId,
            CreateListRequest createListRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves Folderless Lists in a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="archived">Optional. Whether to include archived Lists.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of folderless <see cref="ClickUpList"/> objects in the Space.</returns>
        Task<IEnumerable<ClickUpList>> GetFolderlessListsAsync(
            string spaceId,
            bool? archived = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Folderless List in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="createListRequest">Details of the List to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="ClickUpList"/>.</returns>
        Task<ClickUpList> CreateFolderlessListAsync(
            string spaceId,
            CreateListRequest createListRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves details of a specific List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="ClickUpList"/>.</returns>
        Task<ClickUpList> GetListAsync(
            string listId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="updateListRequest">Details for updating the List.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="ClickUpList"/>.</returns>
        Task<ClickUpList> UpdateListAsync(
            string listId,
            UpdateListRequest updateListRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a List.
        /// </summary>
        /// <param name="listId">The ID of the List to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task DeleteListAsync(
            string listId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a task to an additional List. Requires the Tasks in Multiple Lists ClickApp.
        /// </summary>
        /// <param name="listId">The ID of the List to add the task to.</param>
        /// <param name="taskId">The ID of the task to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task AddTaskToListAsync(
            string listId,
            string taskId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a task from an additional List. Requires the Tasks in Multiple Lists ClickApp.
        /// </summary>
        /// <param name="listId">The ID of the List to remove the task from.</param>
        /// <param name="taskId">The ID of the task to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task RemoveTaskFromListAsync(
            string listId,
            string taskId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new List from a template in a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="templateId">The ID of the List template.</param>
        /// <param name="createListFromTemplateRequest">Details for creating the List from a template (e.g., name).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="ClickUpList"/>. Note: API might return only an ID if 'return_immediately' option is used.</returns>
        Task<ClickUpList> CreateListFromTemplateInFolderAsync(
            string folderId,
            string templateId,
            CreateListFromTemplateRequest createListFromTemplateRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new List from a template in a Space (folderless).
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="templateId">The ID of the List template.</param>
        /// <param name="createListFromTemplateRequest">Details for creating the List from a template (e.g., name).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="ClickUpList"/>. Note: API might return only an ID if 'return_immediately' option is used.</returns>
        Task<ClickUpList> CreateListFromTemplateInSpaceAsync(
            string spaceId,
            string templateId,
            CreateListFromTemplateRequest createListFromTemplateRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all folderless lists in a specific Space, automatically handling pagination.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="archived">Optional. Whether to include archived Lists.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An asynchronous stream of folderless lists.</returns>
        IAsyncEnumerable<ClickUpList> GetFolderlessListsAsyncEnumerableAsync(
            string spaceId,
            bool? archived = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default
        );
    }
}
