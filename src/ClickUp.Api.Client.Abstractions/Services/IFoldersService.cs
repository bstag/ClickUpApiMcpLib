using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities; // Assuming Folder DTO is here
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
        Task<Folder> CreateFolderFromTemplateAsync(
            string spaceId,
            string templateId,
            CreateFolderFromTemplateRequest createFolderFromTemplateRequest,
            CancellationToken cancellationToken = default);
    }
}
