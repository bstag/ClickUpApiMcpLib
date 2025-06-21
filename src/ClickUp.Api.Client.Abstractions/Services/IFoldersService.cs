using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Folders; // Assuming Folder DTO is here
using ClickUp.Api.Client.Models.RequestModels.Folders; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Folders operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/space/{space_id}/folder
    /// - POST /v2/space/{space_id}/folder
    /// - GET /v2/folder/{folder_id}
    /// - PUT /v2/folder/{folder_id}
    /// - DELETE /v2/folder/{folder_id}
    /// - POST /v2/space/{space_id}/folder_template/{template_id}
    /// </remarks>
    public interface IFoldersService
    {
        /// <summary>
        /// Retrieves Folders in a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="archived">Optional. Whether to include archived Folders.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Folder"/> objects in the Space.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access folders in this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<IEnumerable<Folder>> GetFoldersAsync(
            string spaceId,
            bool? archived = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Folder in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="createFolderRequest">Details of the Folder to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="Folder"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/> or <paramref name="createFolderRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to create folders in this space.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Folder> CreateFolderAsync(
            string spaceId,
            CreateFolderRequest createFolderRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves details of a specific Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of the <see cref="Folder"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the folder with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access this folder.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Folder> GetFolderAsync(
            string folderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Renames or updates a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="updateFolderRequest">Details for updating the Folder.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="Folder"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> or <paramref name="updateFolderRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the folder with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to update this folder.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Folder> UpdateFolderAsync(
            string folderId,
            UpdateFolderRequest updateFolderRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="folderId"/> is null or empty.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the folder with the specified ID is not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to delete this folder.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        System.Threading.Tasks.Task DeleteFolderAsync(
            string folderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Folder from a template within a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="templateId">The ID of the Folder template.</param>
        /// <param name="createFolderFromTemplateRequest">Details for creating the Folder from a template.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created <see cref="Folder"/>. Note: API might return only an ID if 'return_immediately' option is used.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="spaceId"/>, <paramref name="templateId"/>, or <paramref name="createFolderFromTemplateRequest"/> is null.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiNotFoundException">Thrown if the space or template with the specified IDs are not found.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to perform this action.</exception>
        /// <exception cref="ClickUp.Api.Client.Models.Exceptions.ClickUpApiException">Thrown if the API call fails for other reasons.</exception>
        Task<Folder> CreateFolderFromTemplateAsync(
            string spaceId,
            string templateId,
            CreateFolderFromTemplateRequest createFolderFromTemplateRequest,
            CancellationToken cancellationToken = default);
    }
}
