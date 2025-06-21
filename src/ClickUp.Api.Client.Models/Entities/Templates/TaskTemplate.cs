using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Checklists;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.Entities.Templates;

/// <summary>
/// Represents a CuTask Template in ClickUp.
/// </summary>
public class TaskTemplate
{
    /// <summary>
    /// Gets or sets the unique identifier of the task template.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the task template.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the identifier of the project (Folder) this template is associated with.
    /// </summary>
    [JsonPropertyName("project_id")]
    public string? ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the List this template is associated with.
    /// </summary>
    [JsonPropertyName("list_id")]
    public string? ListId { get; set; }

    /// <summary>
    /// Gets or sets the description for tasks created from this template.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the list of default assignees for tasks created from this template.
    /// </summary>
    [JsonPropertyName("assignees")]
    public List<User> Assignees { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of default tags for tasks created from this template.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<Tag> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets the default status for tasks created from this template.
    /// </summary>
    [JsonPropertyName("status")]
    public Status? Status { get; set; }

    /// <summary>
    /// Gets or sets the default priority for tasks created from this template.
    /// </summary>
    [JsonPropertyName("priority")]
    public Priority? Priority { get; set; }

    /// <summary>
    /// Gets or sets the default due date for tasks created from this template (Unix time in milliseconds as string).
    /// </summary>
    [JsonPropertyName("due_date")]
    public string? DueDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the due date includes a time component.
    /// </summary>
    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; set; }

    /// <summary>
    /// Gets or sets the default time estimate for tasks created from this template (in milliseconds).
    /// </summary>
    [JsonPropertyName("time_estimate")]
    public int? TimeEstimate { get; set; }

    /// <summary>
    /// Gets or sets the default start date for tasks created from this template (Unix time in milliseconds as string).
    /// </summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the start date includes a time component.
    /// </summary>
    [JsonPropertyName("start_date_time")]
    public bool? StartDateTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to notify all assignees when a task is created from this template.
    /// </summary>
    [JsonPropertyName("notify_all")]
    public bool? NotifyAll { get; set; }

    /// <summary>
    /// Gets or sets the identifier of a parent task if tasks created from this template should be subtasks.
    /// </summary>
    [JsonPropertyName("parent")]
    public string? Parent { get; set; }

    /// <summary>
    /// Gets or sets the identifier of a task that tasks created from this template should link to.
    /// </summary>
    [JsonPropertyName("links_to")]
    public string? LinksTo { get; set; }

    /// <summary>
    /// Gets or sets the list of default checklists for tasks created from this template.
    /// </summary>
    [JsonPropertyName("checklists")]
    public List<Checklist> Checklists { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of default custom field definitions and values for tasks created from this template.
    /// </summary>
    [JsonPropertyName("custom_fields")]
    public List<CustomFieldDefinitionFromTemplate> CustomFields { get; set; } = new();
}
