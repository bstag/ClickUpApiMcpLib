using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities;
using ClickUp.Api.Client.Models.Entities.Tags; // Assuming Tag DTO is here
using ClickUp.Api.Client.Models.RequestModels; // Assuming Request DTOs are here

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Tags operations in the ClickUp API.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/space/{space_id}/tag
    /// - POST /v2/space/{space_id}/tag
    /// - PUT /v2/space/{space_id}/tag/{tag_name}
    /// - DELETE /v2/space/{space_id}/tag/{tag_name}
    /// - POST /v2/task/{task_id}/tag/{tag_name}
    /// - DELETE /v2/task/{task_id}/tag/{tag_name}
    /// </remarks>
    public interface ITagsService
    {
        /// <summary>
        /// Retrieves task Tags available in a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="Tag"/> objects in the Space.</returns>
        Task<IEnumerable<Tag>> GetSpaceTagsAsync(
            string spaceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new task Tag in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="createTagRequest">Details of the Tag to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task CreateSpaceTagAsync(
            string spaceId,
            CreateTagRequest createTagRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a task Tag in a Space. The tag is identified by its original name.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="tagName">The original name of the Tag to edit.</param>
        /// <param name="updateTagRequest">Details for updating the Tag (e.g., new name, colors).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated <see cref="Tag"/> details.</returns>
        Task<Tag> EditSpaceTagAsync(
            string spaceId,
            string tagName,
            UpdateTagRequest updateTagRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a task Tag from a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="tagName">The name of the Tag to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task DeleteSpaceTagAsync(
            string spaceId,
            string tagName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a Tag to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="tagName">The name of the Tag to add. The tag must already exist in the Space.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task AddTagToTaskAsync(
            string taskId,
            string tagName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a Tag from a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="tagName">The name of the Tag to remove.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID (formerly team_id), required if customTaskIds is true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An awaitable task representing the asynchronous operation (void).</returns>
        System.Threading.Tasks.Task RemoveTagFromTaskAsync(
            string taskId,
            string tagName,
            bool? customTaskIds = null,
            string? teamId = null,
            CancellationToken cancellationToken = default);
    }
}
