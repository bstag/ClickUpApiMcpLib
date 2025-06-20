using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickUp.Api.Client.Models.Common;
using ClickUp.Api.Client.Models.Entities.Checklists;
using ClickUp.Api.Client.Models.Entities.Tags;
using ClickUp.Api.Client.Models.Entities.Users;

namespace ClickUp.Api.Client.Models.Entities.Templates;

public class TaskTemplate
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("project_id")]
    public string? ProjectId { get; set; }

    [JsonPropertyName("list_id")]
    public string? ListId { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("assignees")]
    public List<User> Assignees { get; set; } = new();

    [JsonPropertyName("tags")]
    public List<Tag> Tags { get; set; } = new();

    [JsonPropertyName("status")]
    public Status? Status { get; set; }

    [JsonPropertyName("priority")]
    public Priority? Priority { get; set; }

    [JsonPropertyName("due_date")]
    public string? DueDate { get; set; } // Unix time in milliseconds as string

    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; set; }

    [JsonPropertyName("time_estimate")]
    public int? TimeEstimate { get; set; }

    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; } // Unix time in milliseconds as string

    [JsonPropertyName("start_date_time")]
    public bool? StartDateTime { get; set; }

    [JsonPropertyName("notify_all")]
    public bool? NotifyAll { get; set; }

    [JsonPropertyName("parent")]
    public string? Parent { get; set; }

    [JsonPropertyName("links_to")]
    public string? LinksTo { get; set; }

    [JsonPropertyName("checklists")]
    public List<Checklist> Checklists { get; set; } = new();

    [JsonPropertyName("custom_fields")]
    public List<CustomFieldDefinitionFromTemplate> CustomFields { get; set; } = new();
}
