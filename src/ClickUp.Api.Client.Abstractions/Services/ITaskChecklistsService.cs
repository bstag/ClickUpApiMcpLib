using ClickUp.Api.Client.Abstractions.Services.Checklists;

namespace ClickUp.Api.Client.Abstractions.Services
{
    /// <summary>
    /// Comprehensive service interface for ClickUp Task Checklists operations.
    /// This interface follows the Interface Segregation Principle by composing smaller, focused interfaces.
    /// </summary>
    /// <remarks>
    /// This service provides comprehensive methods for managing Checklists and their individual items within Tasks.
    /// The interface is composed of:
    /// - IChecklistManager: For checklist-level operations (create, edit, delete checklists)
    /// - IChecklistItemManager: For checklist item operations (create, edit, delete checklist items)
    /// 
    /// Covered API Endpoints:
    /// - Create Checklist: `POST /task/{task_id}/checklist`
    /// - Edit Checklist: `PUT /checklist/{checklist_id}`
    /// - Delete Checklist: `DELETE /checklist/{checklist_id}`
    /// - Create Checklist Item: `POST /checklist/{checklist_id}/checklist_item`
    /// - Edit Checklist Item: `PUT /checklist/{checklist_id}/checklist_item/{checklist_item_id}`
    /// - Delete Checklist Item: `DELETE /checklist/{checklist_id}/checklist_item/{checklist_item_id}`
    /// </remarks>
    public interface ITaskChecklistsService : IChecklistManager, IChecklistItemManager
    {
        // All methods are now inherited from the composed interfaces:
        // - IChecklistManager: Checklist-level operations (create, edit, delete checklists)
        // - IChecklistItemManager: Checklist item operations (create, edit, delete checklist items)
    }
}
