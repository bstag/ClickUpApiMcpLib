using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Folders;
using ClickUp.Api.Client.Models.RequestModels.Folders;

namespace ClickUp.Api.Client.Abstractions.Services.Folders
{
    /// <summary>
    /// Interface for writing ClickUp Folder operations (Create, Update, Delete).
    /// This interface follows the Interface Segregation Principle by focusing solely on basic write operations.
    /// </summary>
    public interface IFolderWriter
    {
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
    }
}