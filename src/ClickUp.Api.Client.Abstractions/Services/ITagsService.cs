using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Tags operations in the ClickUp API
    // Based on endpoints like:
    // - GET /v2/space/{space_id}/tag
    // - POST /v2/space/{space_id}/tag
    // - PUT /v2/space/{space_id}/tag/{tag_name}
    // - DELETE /v2/space/{space_id}/tag/{tag_name}
    // - POST /v2/task/{task_id}/tag/{tag_name}
    // - DELETE /v2/task/{task_id}/tag/{tag_name}

    public interface ITagsService
    {
        /// <summary>
        /// Retrieves task Tags available in a specific Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <returns>A list of Tags in the Space.</returns>
        Task<IEnumerable<object>> GetSpaceTagsAsync(double spaceId);
        // Note: Return type should be IEnumerable<TagDto>. Response is { "tags": [...] }.

        /// <summary>
        /// Creates a new task Tag in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="createTagRequest">Details of the Tag to create.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task CreateSpaceTagAsync(double spaceId, object createTagRequest);
        // Note: createTagRequest should be CreateTagRequest. API returns 200 with an empty object.

        /// <summary>
        /// Updates a task Tag in a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="tagName">The original name of the Tag to edit.</param>
        /// <param name="editTagRequest">Details for updating the Tag.</param>
        /// <returns>The updated Tag details.</returns>
        Task<object> EditSpaceTagAsync(double spaceId, string tagName, object editTagRequest);
        // Note: editTagRequest should be EditTagRequest. Response is { "tag": {...} }. Return type should be TagDto.

        /// <summary>
        /// Deletes a task Tag from a Space.
        /// </summary>
        /// <param name="spaceId">The ID of the Space.</param>
        /// <param name="tagName">The name of the Tag to delete.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task DeleteSpaceTagAsync(double spaceId, string tagName);
        // Note: API returns 200 with an empty object. The request body in spec seems incorrect for a DELETE; it should likely be parameter-based or empty. Assuming empty for now.

        /// <summary>
        /// Adds a Tag to a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="tagName">The name of the Tag to add.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task AddTagToTaskAsync(string taskId, string tagName, bool? customTaskIds = null, double? teamId = null);
        // Note: API returns 200 with an empty object.

        /// <summary>
        /// Removes a Tag from a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="tagName">The name of the Tag to remove.</param>
        /// <param name="customTaskIds">Optional. If true, references task by its custom task id.</param>
        /// <param name="teamId">Optional. Workspace ID, required if customTaskIds is true.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task RemoveTagFromTaskAsync(string taskId, string tagName, bool? customTaskIds = null, double? teamId = null);
        // Note: API returns 200 with an empty object.
    }
}
