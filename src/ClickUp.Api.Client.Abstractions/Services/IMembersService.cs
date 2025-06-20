using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.ResponseModels.Members;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Represents the Members operations in the ClickUp API, focusing on retrieving members of tasks and lists.
    /// </summary>
    /// <remarks>
    /// Based on endpoints like:
    /// - GET /v2/task/{task_id}/member
    /// - GET /v2/list/{list_id}/member
    /// </remarks>
    public interface IMembersService
    {
        /// <summary>
        /// Retrieves members who have explicit access to a specific task.
        /// Does not include users with inherited Hierarchy permission unless they are also explicitly added to the task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="ClickUp.Api.Client.Models.ResponseModels.Members.Member"/> objects associated with the task.</returns>
        Task<IEnumerable<ClickUp.Api.Client.Models.ResponseModels.Members.Member>> GetTaskMembersAsync(
            string taskId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves Workspace members who have explicit access to a specific List.
        /// </summary>
        /// <param name="listId">The ID of the List.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of <see cref="ClickUp.Api.Client.Models.ResponseModels.Members.Member"/> objects associated with the List.</returns>
        Task<IEnumerable<ClickUp.Api.Client.Models.ResponseModels.Members.Member>> GetListMembersAsync(
            string listId,
            CancellationToken cancellationToken = default);
    }
}
