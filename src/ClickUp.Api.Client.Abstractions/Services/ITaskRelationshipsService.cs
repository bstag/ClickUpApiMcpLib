using ClickUp.Api.Client.Abstractions.Services.TaskRelationships;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Comprehensive service interface for ClickUp Task Relationships operations.
    /// This interface follows the Interface Segregation Principle by composing smaller, focused interfaces.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing task relationships, including dependencies and links.
    /// The interface is composed of:
    /// - ITaskDependencyManager: For managing task dependencies (blocking/blocked by relationships)
    /// - ITaskLinkManager: For managing non-dependent task links (related but not blocking relationships)
    /// 
    /// Covered API Endpoints:
    /// - Add Dependency: `POST /task/{task_id}/dependency`
    /// - Remove Dependency: `DELETE /task/{task_id}/dependency`
    /// - Add Task Link: `POST /task/{task_id}/link/{links_to_task_id}`
    /// - Remove Task Link: `DELETE /task/{task_id}/link/{links_to_task_id}`
    /// </remarks>
    public interface ITaskRelationshipsService : ITaskDependencyManager, ITaskLinkManager
    {
        // All methods are now inherited from the composed interfaces:
        // - ITaskDependencyManager: Task dependency operations (add/remove dependencies)
        // - ITaskLinkManager: Task link operations (add/remove non-dependent links)
    }
}
