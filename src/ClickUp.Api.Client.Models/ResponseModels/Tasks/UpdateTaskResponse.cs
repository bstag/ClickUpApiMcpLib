using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Tasks; // For Task entity

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks
{
    // UpdateTaskResponse is typically the full Task object.
    public record UpdateTaskResponse : Task
    {
        // Constructor to map from base Task properties
        public UpdateTaskResponse(Task task)
            : base(task.Id, task.CustomId, task.CustomItemId, task.Name, task.TextContent, task.Description, task.MarkdownDescription,
                   task.Status, task.OrderIndex, task.DateCreated, task.DateUpdated, task.DateClosed, task.Archived,
                   task.Creator, task.Assignees, task.GroupAssignees, task.Watchers, task.Checklists, task.Tags,
                   task.Parent, task.Priority, task.DueDate, task.StartDate, task.Points, task.TimeEstimate,
                   task.TimeSpent, task.CustomFields, task.Dependencies, task.LinkedTasks, task.TeamId,
                   task.Url, task.Sharing, task.PermissionLevel, task.List, task.Folder, task.Space, task.Project)
        {
            // Any additional properties specific to UpdateTaskResponse beyond the Task entity can be added here.
        }

        // If UpdateTaskResponse has its own distinct properties, declare them here.
    }
}
