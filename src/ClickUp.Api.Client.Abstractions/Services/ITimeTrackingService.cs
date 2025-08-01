using ClickUp.Api.Client.Abstractions.Services.TimeTracking;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Service interface for ClickUp Time Tracking operations.
    /// Composed of multiple focused interfaces following the Interface Segregation Principle.
    /// </summary>
    /// <remarks>
    /// This service provides methods for creating, retrieving, updating, deleting, and managing time entries
    /// and their associated tags within a Workspace. It also includes functionality for starting and stopping timers.
    /// Covered API Endpoints (non-exhaustive list):
    /// - Get Time Entries: `GET /team/{team_id}/time_entries`
    /// - Create Time Entry: `POST /team/{team_id}/time_entries`
    /// - Get Single Time Entry: `GET /team/{team_id}/time_entries/{timer_id}`
    /// - Update Time Entry: `PUT /team/{team_id}/time_entries/{timer_id}`
    /// - Delete Time Entry: `DELETE /team/{team_id}/time_entries/{timer_id}`
    /// - Get Time Entry History: `GET /team/{team_id}/time_entries/{timer_id}/history`
    /// - Get Running Time Entry: `GET /team/{team_id}/time_entries/current`
    /// - Start Timer: `POST /team/{team_id}/time_entries/start`
    /// - Stop Timer: `POST /team/{team_id}/time_entries/stop`
    /// - Time Entry Tags: `GET /team/{team_id}/time_entries/tags`, `POST /team/{team_id}/time_entries/tags`, `DELETE /team/{team_id}/time_entries/tags`, `PUT /team/{team_id}/time_entries/tags`
    /// </remarks>
    public interface ITimeTrackingService : ITimeEntryManager, ITimerController, ITimeEntryTagManager
    {
        // All methods are now inherited from the composed interfaces:
        // - ITimeEntryManager: Provides time entry CRUD operations
        // - ITimerController: Provides timer control operations (start/stop/get running)
        // - ITimeEntryTagManager: Provides time entry tag management operations
    }
}
