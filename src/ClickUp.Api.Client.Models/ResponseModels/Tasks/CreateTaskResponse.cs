using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Tasks; // For Task entity

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks
{
    // CreateTaskResponse typically returns the full Task object that was created.
    // Instead of listing all properties of Task again, we can make this record inherit from Task.
    // However, the response might sometimes have a slightly different structure or additional metadata.
    // For now, assuming it's a direct Task representation.
    // If the API response is exactly a Task object, this file might not be strictly necessary
    // if the deserializer can map directly to the Task entity.
    // But having a distinct response type is good practice for potential future differences.

    public record CreateTaskResponse : Task
    {
        // Constructor to map from base Task properties
        public CreateTaskResponse(Task task)
            : base(task.Id, task.CustomId, task.CustomItemId, task.Name, task.TextContent, task.Description, task.MarkdownDescription,
                   task.Status, task.OrderIndex, task.DateCreated, task.DateUpdated, task.DateClosed, task.Archived,
                   task.Creator, task.Assignees, task.GroupAssignees, task.Watchers, task.Checklists, task.Tags,
                   task.Parent, task.Priority, task.DueDate, task.StartDate, task.Points, task.TimeEstimate,
                   task.TimeSpent, task.CustomFields, task.Dependencies, task.LinkedTasks, task.TeamId,
                   task.Url, task.Sharing, task.PermissionLevel, task.List, task.Folder, task.Space, task.Project)
        {
            // Any additional properties specific to CreateTaskResponse beyond the Task entity can be added here.
            // For example, if the response includes some extra metadata not present in the standard Task model.
        }

        // If CreateTaskResponse has its own distinct properties beyond what Task provides,
        // they would be declared here. For now, assuming it mirrors the Task structure.
        // Example: [property: JsonPropertyName("additional_info")] public string? AdditionalInfo { get; init; }
    }
}
