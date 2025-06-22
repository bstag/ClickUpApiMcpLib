using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Folder operations.
    /// </summary>
    /// <remarks>
    /// This service provides methods for managing Folders within Spaces,
    /// including creating, retrieving, updating, deleting Folders, and creating Folders from templates.
    /// Covered API Endpoints:
    /// - `GET /space/{space_id}/folder`: Retrieves Folders in a Space.
    /// - `POST /space/{space_id}/folder`: Creates a new Folder in a Space.
    /// - `GET /folder/{folder_id}`: Retrieves details of a specific Folder.
    /// - `PUT /folder/{folder_id}`: Updates an existing Folder.
    /// - `DELETE /folder/{folder_id}`: Deletes a Folder.
    /// - `POST /space/{space_id}/folder_template/{template_id}`: Creates a Folder from a template.
    /// </remarks>
    public interface IFoldersService
    {
        /// <summary>
        /// Retrieves all Folders within a specific Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space from which to retrieve Folders.</param>
        /// <param name="archived">Optional. If set to <c>true</c>, includes archived Folders in the results. Defaults to <c>false</c>.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Folder"/> objects found in the Space.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access Folders in this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures, such as rate limiting or request errors.</exception>
        Task<IEnumerable<Folder>> GetFoldersAsync(
            string spaceId,
            bool? archived = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Folder within a specified Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space where the new Folder will be created.</param>
        /// <param name="createFolderRequest">An object containing the details for the new Folder, such as its name.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Folder"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="createFolderRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create Folders in this Space.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Folder> CreateFolderAsync(
            string spaceId,
            CreateFolderRequest createFolderRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details of a specific Folder by its ID.
        /// </summary>
        /// <param name="folderId">The unique identifier of the Folder to retrieve.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the details of the requested <see cref="Folder"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Folder with the specified ID does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this Folder.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Folder> GetFolderAsync(
            string folderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the properties of an existing Folder, such as its name.
        /// </summary>
        /// <param name="folderId">The unique identifier of the Folder to update.</param>
        /// <param name="updateFolderRequest">An object containing the properties to update for the Folder.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Folder"/> object.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> or <paramref name="updateFolderRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Folder with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this Folder.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Folder> UpdateFolderAsync(
            string folderId,
            UpdateFolderRequest updateFolderRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specified Folder.
        /// </summary>
        /// <param name="folderId">The unique identifier of the Folder to delete.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Folder with the specified ID does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this Folder.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task DeleteFolderAsync(
            string folderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Folder from a specified Folder template within a Space.
        /// </summary>
        /// <param name="spaceId">The unique identifier of the Space where the new Folder will be created.</param>
        /// <param name="templateId">The unique identifier of the Folder template to use.</param>
        /// <param name="createFolderFromTemplateRequest">An object containing details for creating the Folder from the template, such as the new Folder's name.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Folder"/> object. Note: The API might return only an ID if specific 'return_immediately' options are used in the request, though this SDK aims to return the full object where possible.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/>, <paramref name="templateId"/>, or <paramref name="createFolderFromTemplateRequest"/> is null.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the Space or Folder template with the specified IDs does not exist or is not accessible.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action in the Space or use the template.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<Folder> CreateFolderFromTemplateAsync(
            string spaceId,
            string templateId,
            CreateFolderFromTemplateRequest createFolderFromTemplateRequest,
            CancellationToken cancellationToken = default);
    }
}
