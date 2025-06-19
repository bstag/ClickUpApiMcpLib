using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Lists operations in the ClickUp API
    // Based on endpoints like:
    // - GET /v2/folder/{folder_id}/list
    // - POST /v2/folder/{folder_id}/list
    // - GET /v2/space/{space_id}/list (Folderless)
    // - POST /v2/space/{space_id}/list (Folderless)
    // - GET /v2/list/{list_id}
    // - PUT /v2/list/{list_id}
    // - DELETE /v2/list/{list_id}
    // - POST /v2/list/{list_id}/task/{task_id}
    // - DELETE /v2/list/{list_id}/task/{task_id}
    // - POST /v2/folder/{folder_id}/list_template/{template_id}
    // - POST /v2/space/{space_id}/list_template/{template_id}

    public interface IListsService
    {
        /// <summary>
        /// Retrieves Lists within a specific Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="archived">Optional. Whether to include archived Lists.</param>
        /// <returns>A list of Lists in the Folder.</returns>
        Task<IEnumerable<object>> GetListsInFolderAsync(double folderId, bool? archived = null);
        // Note: Return type should be IEnumerable<ListDto>.

        /// <summary>
        /// Creates a new List in a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="createListRequest">Details of the List to create.</param>
        /// <returns>The created List.</returns>
        Task<object> CreateListInFolderAsync(double folderId, object createListRequest);
        // Note: createListRequest should be CreateListInFolderRequest, return type should be ListDto.

        /// <summary>
        /// Retrieves Folderless Lists in a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="archived">Optional. Whether to include archived Lists.</param>
        /// <returns>A list of Folderless Lists in the Space.</returns>
        Task<IEnumerable<object>> GetFolderlessListsAsync(double spaceId, bool? archived = null);
        // Note: Return type should be IEnumerable<ListDto>.

        /// <summary>
        /// Creates a new Folderless List in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="createListRequest">Details of the List to create.</param>
        /// <returns>The created List.</returns>
        Task<object> CreateFolderlessListAsync(double spaceId, object createListRequest);
        // Note: createListRequest should be CreateFolderlessListRequest, return type should be ListDto.

        /// <summary>
        /// Retrieves details of a specific List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <returns>Details of the List.</returns>
        Task<object> GetListAsync(double listId);
        // Note: Return type should be ListDto.

        /// <summary>
        /// Updates a List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="updateListRequest">Details for updating the List.</param>
        /// <returns>The updated List.</returns>
        Task<object> UpdateListAsync(string listId, object updateListRequest);
        // Note: updateListRequest should be UpdateListRequest, return type should be ListDto.

        /// <summary>
        /// Deletes a List.
        /// </summary>
        /// <param name="listId">The ID of the List to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteListAsync(double listId);
        // Note: API returns 200 with an empty object.

        /// <summary>
        /// Adds a task to an additional List. Requires the Tasks in Multiple Lists ClickApp.
        /// </summary>
        /// <param name="listId">The ID of the List to add the task to.</param>
        /// <param name="taskId">The ID of the task to add.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task AddTaskToListAsync(double listId, string taskId);
        // Note: API returns 200 with an empty object.

        /// <summary>
        /// Removes a task from an additional List. Requires the Tasks in Multiple Lists ClickApp.
        /// </summary>
        /// <param name="listId">The ID of the List to remove the task from.</param>
        /// <param name="taskId">The ID of the task to remove.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task RemoveTaskFromListAsync(double listId, string taskId);
        // Note: API returns 200 with an empty object.

        /// <summary>
        /// Creates a new List from a template in a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="templateId">The ID of the List template.</param>
        /// <param name="createListFromTemplateRequest">Details for creating the List from a template.</param>
        /// <returns>The created List, or an object with an ID if return_immediately is true.</returns>
        Task<object> CreateListFromTemplateInFolderAsync(string folderId, string templateId, object createListFromTemplateRequest);
        // Note: createListFromTemplateRequest should be CreateListFromTemplateRequest.

        /// <summary>
        /// Creates a new List from a template in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="templateId">The ID of the List template.</param>
        /// <param name="createListFromTemplateRequest">Details for creating the List from a template.</param>
        /// <returns>The created List, or an object with an ID if return_immediately is true.</returns>
        Task<object> CreateListFromTemplateInSpaceAsync(string spaceId, string templateId, object createListFromTemplateRequest);
        // Note: createListFromTemplateRequest should be CreateListFromTemplateRequest.
    }
}
