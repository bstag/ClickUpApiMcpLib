using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.TimeTracking;
using ClickUp.Api.Client.Models.RequestModels.TimeTracking;

namespace ClickUp.Api.Client.Abstractions.Services.TimeTracking
{
    /// <summary>
    /// Interface for managing ClickUp Time Entry Tag operations.
    /// Handles tag management operations for time entries within a Workspace.
    /// </summary>
    /// <remarks>
    /// This interface focuses on time entry tag management operations including:
    /// - Retrieving all time entry tags
    /// - Adding tags to time entries
    /// - Removing tags from time entries
    /// - Changing tag names and properties
    /// </remarks>
    public interface ITimeEntryTagManager
    {
        /// <summary>
        /// Retrieves all unique tags that have been used in time entries for a specific Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="TaskTag"/> objects representing the unique time entry tags.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> is null or empty.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to access time entry tags.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        Task<IEnumerable<TaskTag>> GetAllTimeEntryTagsAsync(
            string workspaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds one or more specified tags to one or more time entries within a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="addTagsRequest">An <see cref="AddTagsFromTimeEntriesRequest"/> object containing a list of time entry IDs and the tags to be added to them.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="addTagsRequest"/> is null, or if the request contains no time entry IDs or tags.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify tags on time entries.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task AddTagsToTimeEntriesAsync(
            string workspaceId,
            AddTagsFromTimeEntriesRequest addTagsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes one or more specified tags from one or more time entries within a Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="removeTagsRequest">An <see cref="RemoveTagsFromTimeEntriesRequest"/> object containing a list of time entry IDs and the tags to be removed from them.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="removeTagsRequest"/> is null, or if the request contains no time entry IDs or tags.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify tags on time entries.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task RemoveTagsFromTimeEntriesAsync(
            string workspaceId,
            RemoveTagsFromTimeEntriesRequest removeTagsRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the name and color attributes of an existing time entry tag across an entire Workspace (Team).
        /// </summary>
        /// <param name="workspaceId">The unique identifier of the Workspace (Team).</param>
        /// <param name="changeTagNamesRequest">A <see cref="ChangeTagNamesFromTimeEntriesRequest"/> object specifying the current tag name and the new name and color attributes to apply.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete, allowing cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous completion of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="workspaceId"/> or <paramref name="changeTagNamesRequest"/> is null, or if the tag name details are invalid.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiNotFoundException">Thrown if the tag to be changed does not exist.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiAuthenticationException">Thrown if the user is not authorized to modify time entry tags.</exception>
        /// <exception cref="Models.Exceptions.ClickUpApiException">Thrown for other API call failures.</exception>
        System.Threading.Tasks.Task ChangeTimeEntryTagNameAsync(
            string workspaceId,
            ChangeTagNamesFromTimeEntriesRequest changeTagNamesRequest,
            CancellationToken cancellationToken = default);
    }
}