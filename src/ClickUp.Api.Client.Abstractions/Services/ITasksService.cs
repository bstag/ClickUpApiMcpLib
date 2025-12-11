using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Models.Entities.Tasks;
using ClickUp.Api.Client.Models.RequestModels.Tasks;
using ClickUp.Api.Client.Models.ResponseModels.Tasks;
using ClickUp.Api.Client.Models.Common.Pagination;
using ClickUp.Api.Client.Models.Parameters; // Consolidated using

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Provides comprehensive operations for managing ClickUp Tasks, composed of specialized task operation interfaces.
    /// This interface follows the Interface Segregation Principle by composing smaller, focused interfaces.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing Tasks, including CRUD operations,
    /// filtering, merging, retrieving time in status, and creating Tasks from templates.
    /// It also supports pagination for retrieving lists of Tasks.
    /// Covered API Endpoints (non-exhaustive list):
    /// - List Tasks: `GET /list/{list_id}/task`
    /// - Create Task: `POST /list/{list_id}/task`
    /// - Get Task: `GET /task/{task_id}`
    /// - Update Task: `PUT /task/{task_id}`
    /// - Delete Task: `DELETE /task/{task_id}`
    /// - Get Filtered Workspace Tasks: `GET /team/{team_id}/task`
    /// - Merge Tasks: `POST /task/{task_id}/merge` (Note: API endpoint might be different, this is based on typical patterns)
    /// - Task Time in Status: `GET /task/{task_id}/time_in_status`
    /// - Bulk Task Time in Status: `GET /task/bulk_time_in_status/task_ids`
    /// - Create Task from Template: `POST /list/{list_id}/taskTemplate/{template_id}`
    /// </remarks>
    public interface ITasksService : ITaskReader, ITaskWriter, ITaskRelations, ITaskAnalytics
    {
        // This interface now composes the specialized task interfaces:
        // - ITaskReader: For reading and retrieving task information
        // - ITaskWriter: For creating, updating, and deleting tasks
        // - ITaskRelations: For managing task relationships and interactions
        // - ITaskAnalytics: For task analytics, reporting, and time tracking
        //
        // All method declarations are now inherited from the composed interfaces.
    }
}
