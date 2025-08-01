using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Folders;

namespace ClickUp.Api.Client.Abstractions.Services.Folders
{
    /// <summary>
    /// Interface for reading ClickUp Folder operations.
    /// This interface follows the Interface Segregation Principle by focusing solely on read operations.
    /// </summary>
    public interface IFolderReader
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
    }
}