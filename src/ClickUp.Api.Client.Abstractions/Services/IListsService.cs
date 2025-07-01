using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.RequestModels.Lists;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp List operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for managing Lists within Folders or directly within Spaces (Folderless Lists).
    /// It supports creating, retrieving, updating, and deleting Lists, as well as managing tasks within Lists
    /// (e.g., adding/removing tasks from additional Lists if the "Tasks in Multiple Lists" ClickApp is enabled)
    /// and creating Lists from templates.
    /// Covered API Endpoints:
    /// - Lists in Folders: `GET /folder/{folder_id}/list`, `POST /folder/{folder_id}/list`
    /// - Folderless Lists: `GET /space/{space_id}/list`, `POST /space/{space_id}/list`
    /// - General List Operations: `GET /list/{list_id}`, `PUT /list/{list_id}`, `DELETE /list/{list_id}`
    /// - Tasks in Multiple Lists: `POST /list/{list_id}/task/{task_id}`, `DELETE /list/{list_id}/task/{task_id}`
    /// - Lists from Templates: `POST /folder/{folder_id}/list_template/{template_id}`, `POST /space/{space_id}/list_template/{template_id}`
    /// </remarks>
    public interface IListsService
    {
        /// <summary>
        /// Retrieves all Lists within a specific Folder.
        /// </summary>
        /// <param name="folderId">The unique identifier of the Folder from which to retrieve Lists.</param>
        /// <param name="archived">Optional. If set to <c>true</c>, includes archived Lists in the results. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="ClickUpList"/> objects found in the Folder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Folder with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Lists in this Folder.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<IEnumerable<ClickUpList>> GetListsInFolderAsync(
            string folderId,
            bool? archived = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new List within a specified Folder.
        /// </summary>
        /// <param name="folderId">The unique identifier of the Folder where the new List will be created.</param>
        /// <param name="createListRequest">An object containing the details for the new List, such as its name and status settings.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="ClickUpList"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> or <paramref name="createListRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Folder with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create Lists in this Folder.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ClickUpList> CreateListInFolderAsync(
            string folderId,
            CreateListRequest createListRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all Folderless Lists within a specific Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space from which to retrieve Folderless Lists.</param>
        /// <param name="archived">Optional. If set to <c>true</c>, includes archived Lists in the results. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of folderless <see cref="ClickUpList"/> objects found in the Space.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Folderless Lists in this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<ClickUpList>> GetFolderlessListsAsync(
            string spaceId,
            bool? archived = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Folderless List within a specified Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space where the new Folderless List will be created.</param>
        /// <param name="createListRequest">An object containing the details for the new List.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="ClickUpList"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="createListRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create Folderless Lists in this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ClickUpList> CreateFolderlessListAsync(
            string spaceId,
            CreateListRequest createListRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details of a specific List by its ID.
        /// </summary>
        /// <param name="listId">The unique identifier of the List to retrieve.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the details of the requested <see cref="ClickUpList"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ClickUpList> GetListAsync(
            string listId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the properties of an existing List.
        /// </summary>
        /// <param name="listId">The unique identifier of the List to update.</param>
        /// <param name="updateListRequest">An object containing the properties to update for the List.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="ClickUpList"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="updateListRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ClickUpList> UpdateListAsync(
            string listId,
            UpdateListRequest updateListRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specified List.
        /// </summary>
        /// <param name="listId">The unique identifier of the List to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteListAsync(
            string listId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds an existing task to an additional List. This operation requires the "Tasks in Multiple Lists" ClickApp to be enabled for the Workspace.
        /// </summary>
        /// <param name="listId">The unique identifier of the List to which the task will be added.</param>
        /// <param name="taskId">The unique identifier of the task to add to the List.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List or task with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the "Tasks in Multiple Lists" ClickApp is not enabled, or if other validation rules fail.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task AddTaskToListAsync(
            string listId,
            string taskId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a task from one of its Lists. This operation requires the "Tasks in Multiple Lists" ClickApp to be enabled.
        /// A task must always belong to at least one List; it cannot be removed from its primary List if it's not in any other Lists.
        /// </summary>
        /// <param name="listId">The unique identifier of the List from which the task will be removed.</param>
        /// <param name="taskId">The unique identifier of the task to remove from the List.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="listId"/> or <paramref name="taskId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the List or task with the specified IDs does not exist, or if the task is not currently associated with the List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiValidationException">Thrown if the "Tasks in Multiple Lists" ClickApp is not enabled, or if attempting to remove the task from its only List.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task RemoveTaskFromListAsync(
            string listId,
            string taskId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new List from a specified List template within a Folder.
        /// </summary>
        /// <param name="folderId">The unique identifier of the Folder where the new List will be created.</param>
        /// <param name="templateId">The unique identifier of the List template to use.</param>
        /// <param name="createListFromTemplateRequest">An object containing details for creating the List from the template, such as the new List's name.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="ClickUpList"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/>, <paramref name="templateId"/>, or <paramref name="createListFromTemplateRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Folder or List template with the specified IDs does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action in the Folder or use the template.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ClickUpList> CreateListFromTemplateInFolderAsync(
            string folderId,
            string templateId,
            CreateListFromTemplateRequest createListFromTemplateRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Folderless List from a specified List template within a Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space where the new Folderless List will be created.</param>
        /// <param name="templateId">The unique identifier of the List template to use.</param>
        /// <param name="createListFromTemplateRequest">An object containing details for creating the List from the template.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="ClickUpList"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/>, <paramref name="templateId"/>, or <paramref name="createListFromTemplateRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space or List template with the specified IDs does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action in the Space or use the template.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<ClickUpList> CreateListFromTemplateInSpaceAsync(
            string spaceId,
            string templateId,
            CreateListFromTemplateRequest createListFromTemplateRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all Folderless Lists within a specific Space, automatically handling pagination using <see cref="IAsyncEnumerable{T}"/>.
         /// </summary>
        /// <param name="spaceId">The unique identifier of the Space.</param>
        /// <param name="archived">Optional. If set to <c>true</c>, includes archived Lists in the results. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>An asynchronous stream of <see cref="ClickUpList"/> objects representing the Folderless Lists in the Space.</returns>
         /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Folderless Lists in this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown if an API call fails during the pagination process.</exception>
         IAsyncEnumerable<ClickUpList> GetFolderlessListsAsyncEnumerableAsync(
             string spaceId,
             bool? archived = null,
             CancellationToken cancellationToken = default
         );
     }

}
