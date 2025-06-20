using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask entity

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks
{
    // UpdateTaskResponse is typically the full CuTask object.
    public record UpdateTaskResponse : CuTask
    {
        // Constructor to map from base CuTask properties
        public UpdateTaskResponse(CuTask task)
            : base(task.Id, task.CustomId, task.CustomItemId, task.Name, task.TextContent, task.Description, task.MarkdownDescription,
                   task.Status, task.OrderIndex, task.DateCreated, task.DateUpdated, task.DateClosed, task.Archived,
                   task.Creator, task.Assignees, task.GroupAssignees, task.Watchers, task.Checklists, task.Tags,
                   task.Parent, task.Priority, task.DueDate, task.StartDate, task.Points, task.TimeEstimate,
                   task.TimeSpent, task.CustomFields, task.Dependencies, task.LinkedTasks, task.TeamId,
                   task.Url, task.Sharing, task.PermissionLevel, task.List, task.Folder, task.Space, task.Project)
        {
            // Any additional properties specific to UpdateTaskResponse beyond the CuTask entity can be added here.
        }

        // If UpdateTaskResponse has its own distinct properties, declare them here.
    }
}
