using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.Entities.Lists;
using ClickUp.Api.Client.Models.RequestModels.Lists;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines operations for creating, updating, and deleting Lists in ClickUp.
    /// </summary>
    public interface IListWriter
    {
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
    }
}