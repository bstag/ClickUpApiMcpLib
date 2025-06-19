using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Services
{
    // Represents the Members operations in the ClickUp API, focusing on retrieving members of tasks and lists.
    // Based on endpoints like:
    // - GET /v2/task/{task_id}/member
    // - GET /v2/list/{list_id}/member

    public interface IMembersService
    {
        /// <summary>
        /// Retrieves members who have access to a specific task.
        /// Does not include users with inherited Hierarchy permission.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>A list of members associated with the task.</returns>
        Task<IEnumerable<object>> GetTaskMembersAsync(string taskId);
        // Note: Return type should be IEnumerable<TaskMemberDto>.

        /// <summary>
        /// Retrieves Workspace members who have access to a specific List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <returns>A list of members associated with the List.</returns>
        Task<IEnumerable<object>> GetListMembersAsync(double listId);
        // Note: Return type should be IEnumerable<TaskMemberDto> (or a similar Member DTO if structure varies).
    }
}
