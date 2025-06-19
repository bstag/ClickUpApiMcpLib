using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{

    // Represents the Folders operations in the ClickUp API
    // Based on endpoints like:
    // - GET /v2/space/{space_id}/folder
    // - POST /v2/space/{space_id}/folder
    // - GET /v2/folder/{folder_id}
    // - PUT /v2/folder/{folder_id}
    // - DELETE /v2/folder/{folder_id}
    // - POST /v2/space/{space_id}/folder_template/{template_id}

    public interface IFoldersService
    {
        /// <summary>
        /// Retrieves Folders in a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="archived">Optional. Whether to include archived Folders.</param>
        /// <returns>A list of Folders in the Space.</returns>
        Task<IEnumerable<object>> GetFoldersAsync(double spaceId, bool? archived = null);
        // Note: Return type should be IEnumerable<FolderDto>.

        /// <summary>
        /// Creates a new Folder in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="createFolderRequest">Details of the Folder to create.</param>
        /// <returns>The created Folder.</returns>
        Task<object> CreateFolderAsync(double spaceId, object createFolderRequest);
        // Note: createFolderRequest should be CreateFolderRequest, return type should be FolderDto.

        /// <summary>
        /// Retrieves details of a specific Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <returns>Details of the Folder.</returns>
        Task<object> GetFolderAsync(double folderId);
        // Note: Return type should be FolderDto.

        /// <summary>
        /// Renames a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder.</param>
        /// <param name="updateFolderRequest">Details for updating the Folder.</param>
        /// <returns>The updated Folder.</returns>
        Task<object> UpdateFolderAsync(double folderId, object updateFolderRequest);
        // Note: updateFolderRequest should be UpdateFolderRequest, return type should be FolderDto.

        /// <summary>
        /// Deletes a Folder.
        /// </summary>
        /// <param name="folderId">The ID of the Folder to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteFolderAsync(double folderId);
        // Note: API returns 200 with an empty object.

        /// <summary>
        /// Creates a new Folder from a template within a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="templateId">The ID of the Folder template.</param>
        /// <param name="createFolderFromTemplateRequest">Details for creating the Folder from a template.</param>
        /// <returns>The created Folder, or an object containing an ID if return_immediately is true in options.</returns>
        Task<object> CreateFolderFromTemplateAsync(string spaceId, string templateId, object createFolderFromTemplateRequest);
        // Note: createFolderFromTemplateRequest should be CreateFolderFromTemplateRequest.
        // Return type depends on options, could be FolderDto or an object with just an ID.
    }
}
