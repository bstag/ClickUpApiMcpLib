using ClickUp.Api.Client.Abstractions.Services.Goals;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Comprehensive service interface for ClickUp Goals and Key Results (Targets) operations.
    /// This interface follows the Interface Segregation Principle by composing smaller, focused interfaces.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing Goals and their associated Key Results (Targets) within a Workspace.
    /// The interface is composed of:
    /// - IGoalManager: For Goal CRUD operations
    /// - IKeyResultManager: For Key Result (Target) management operations
    /// 
    /// Covered API Endpoints:
    /// - Goals: `GET /team/{team_id}/goal`, `POST /team/{team_id}/goal`, `GET /goal/{goal_id}`, `PUT /goal/{goal_id}`, `DELETE /goal/{goal_id}`
    /// - Key Results: `POST /goal/{goal_id}/key_result`, `PUT /key_result/{key_result_id}`, `DELETE /key_result/{key_result_id}`
    /// </remarks>
    public interface IGoalsService : IGoalManager, IKeyResultManager
    {
        // All methods are now inherited from the composed interfaces:
        // - IGoalManager: Provides Goal CRUD operations (Get, Create, Update, Delete Goals)
        // - IKeyResultManager: Provides Key Result management operations (Create, Edit, Delete Key Results)
    }
}
