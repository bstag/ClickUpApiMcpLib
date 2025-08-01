using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models;
using ClickUp.Api.Client.Models.Entities.Lists;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Defines operations for reading and retrieving List information from ClickUp.
    /// </summary>
    public interface IListReader
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
            CancellationToken cancellationToken = default);
    }
}