using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Entities.Tasks; // For CuTask entity

namespace ClickUp.Api.Client.Models.ResponseModels.Tasks
{
    // CreateTaskResponse typically returns the full CuTask object that was created.
    // Instead of listing all properties of CuTask again, we can make this record inherit from CuTask.
    // However, the response might sometimes have a slightly different structure or additional metadata.
    // For now, assuming it's a direct CuTask representation.
    // If the API response is exactly a CuTask object, this file might not be strictly necessary
    // if the deserializer can map directly to the CuTask entity.
    // But having a distinct response type is good practice for potential future differences.

    public record CreateTaskResponse : CuTask
    {
        // Constructor to map from base CuTask properties
        public CreateTaskResponse(CuTask task)
            : base(task.Id, task.CustomId, task.CustomItemId, task.Name, task.TextContent, task.Description, task.MarkdownDescription,
                   task.Status, task.OrderIndex, task.DateCreated, task.DateUpdated, task.DateClosed, task.Archived,
                   task.Creator, task.Assignees, task.GroupAssignees, task.Watchers, task.Checklists, task.Tags,
                   task.Parent, task.Priority, task.DueDate, task.StartDate, task.Points, task.TimeEstimate,
                   task.TimeSpent, task.CustomFields, task.Dependencies, task.LinkedTasks, task.TeamId,
                   task.Url, task.Sharing, task.PermissionLevel, task.List, task.Folder, task.Space, task.Project)
        {
            // Any additional properties specific to CreateTaskResponse beyond the CuTask entity can be added here.
            // For example, if the response includes some extra metadata not present in the standard CuTask model.
        }

        // If CreateTaskResponse has its own distinct properties beyond what CuTask provides,
        // they would be declared here. For now, assuming it mirrors the CuTask structure.
        // Example: [property: JsonPropertyName("additional_info")] public string? AdditionalInfo { get; init; }
    }
}
