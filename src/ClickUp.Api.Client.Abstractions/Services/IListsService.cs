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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the folder with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access lists in this folder.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> or <paramref name="createListRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the folder with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create lists in this folder.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access folderless lists in this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="createListRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create folderless lists in this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the list with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this list.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="updateListRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the list with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this list.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the list with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this list.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the list or task with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiValidationException">Thrown if the 'Tasks in Multiple Lists' ClickApp is not enabled or other validation fails.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the list or task with the specified IDs are not found, or the task is not in the list.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiValidationException">Thrown if the 'Tasks in Multiple Lists' ClickApp is not enabled or other validation fails.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/>, <paramref name="templateId"/>, or <paramref name="createListFromTemplateRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the folder or template with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/>, <paramref name="templateId"/>, or <paramref name="createListFromTemplateRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space or template with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
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
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access folderless lists in this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons during pagination.</exception>
        IAsyncEnumerable<ClickUpList> GetFolderlessListsAsyncEnumerableAsync(
            string spaceId,
            bool? archived = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default
        );
    }
}
