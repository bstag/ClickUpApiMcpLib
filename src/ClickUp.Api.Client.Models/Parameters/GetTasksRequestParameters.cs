using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ClickUp.Api.Client.Models.Common.ValueObjects;

namespace ClickUp.Api.Client.Models.Parameters;

/// <summary>
/// Represents the parameters for filtering tasks when calling GetTasksAsync.
/// </summary>
public class GetTasksRequestParameters
{
    public IEnumerable<long>? SpaceIds { get; set; }
    public IEnumerable<long>? ProjectIds { get; set; } // In ClickUp API for tasks, 'project_ids' refers to Folder IDs
    public IEnumerable<long>? ListIds { get; set; }
    public IEnumerable<int>? AssigneeIds { get; set; }
    public IEnumerable<string>? Statuses { get; set; }
    public IEnumerable<string>? Tags { get; set; }
    public bool? IncludeClosed { get; set; }
    public bool? Subtasks { get; set; }
    public TimeRange? DueDateRange { get; set; }
    public TimeRange? DateCreatedRange { get; set; }
    public TimeRange? DateUpdatedRange { get; set; }
    public SortOption? SortBy { get; set; }
    public int? Page { get; set; }
    public bool? IncludeMarkdownDescription { get; set; }
    public IEnumerable<CustomFieldFilter>? CustomFields { get; set; }
    public IEnumerable<int>? CustomItems { get; set; } // For custom task types (0 for task, 1 for milestone, etc.)
    public bool? Archived { get; set; }

    /// <summary>
    /// Converts the set parameters into a dictionary suitable for an HTTP query.
    /// Array parameters are typically handled by the IApiConnection by creating multiple key-value pairs
    /// (e.g., assignees[]=1&amp;assignees[]=2) or by joining with commas, depending on the specific API endpoint.
    /// This method prepares the dictionary with keys that IApiConnection can then format.
    /// For custom_fields, it serializes to a JSON string as expected by ClickUp's GetTasks endpoint.
    /// </summary>
    /// <returns>A dictionary of query parameters.</returns>
    public Dictionary<string, string> ToDictionary()
    {
        var parameters = new Dictionary<string, string>();

        if (SpaceIds?.Any() ?? false) parameters["space_ids"] = string.Join(",", SpaceIds);
        if (ProjectIds?.Any() ?? false) parameters["project_ids"] = string.Join(",", ProjectIds);
        if (ListIds?.Any() ?? false) parameters["list_ids"] = string.Join(",", ListIds);
        if (AssigneeIds?.Any() ?? false) parameters["assignees"] = string.Join(",", AssigneeIds);
        if (Statuses?.Any() ?? false) parameters["statuses"] = string.Join(",", Statuses.Select(Uri.EscapeDataString));
        if (Tags?.Any() ?? false) parameters["tags"] = string.Join(",", Tags.Select(Uri.EscapeDataString));

        if (IncludeClosed.HasValue) parameters["include_closed"] = IncludeClosed.Value.ToString().ToLowerInvariant();
        if (Subtasks.HasValue) parameters["subtasks"] = Subtasks.Value.ToString().ToLowerInvariant();

        if (DueDateRange != null)
        {
            parameters["due_date_gt"] = DueDateRange.StartDate.ToUnixTimeMilliseconds().ToString();
            parameters["due_date_lt"] = DueDateRange.EndDate.ToUnixTimeMilliseconds().ToString();
        }
        if (DateCreatedRange != null)
        {
            parameters["date_created_gt"] = DateCreatedRange.StartDate.ToUnixTimeMilliseconds().ToString();
            parameters["date_created_lt"] = DateCreatedRange.EndDate.ToUnixTimeMilliseconds().ToString();
        }
        if (DateUpdatedRange != null)
        {
            parameters["date_updated_gt"] = DateUpdatedRange.StartDate.ToUnixTimeMilliseconds().ToString();
            parameters["date_updated_lt"] = DateUpdatedRange.EndDate.ToUnixTimeMilliseconds().ToString();
        }

        if (SortBy != null)
        {
            var sortParams = SortBy.ToQueryParameters("order_by", "reverse");
            foreach (var entry in sortParams)
            {
                parameters[entry.Key] = entry.Value;
            }
        }

        if (Page.HasValue) parameters["page"] = Page.Value.ToString();

        if (IncludeMarkdownDescription.HasValue) parameters["include_markdown_description"] = IncludeMarkdownDescription.Value.ToString().ToLowerInvariant();

        if (CustomFields?.Any() ?? false)
        {
            parameters["custom_fields"] = JsonSerializer.Serialize(CustomFields);
        }

        if (CustomItems?.Any() ?? false) parameters["custom_items"] = string.Join(",", CustomItems);

        if (Archived.HasValue) parameters["archived"] = Archived.Value.ToString().ToLowerInvariant();

        return parameters;
    }
}

/// <summary>
/// Represents a filter for a custom field when querying tasks.
/// </summary>
public class CustomFieldFilter
{
    /// <summary>
    /// Gets or sets the ID of the custom field.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("field_id")]
    public required string FieldId { get; set; }

    /// <summary>
    /// Gets or sets the operator for the filter.
    /// Examples: "=", "&lt;", "&gt;", "IS NULL", "IS NOT NULL".
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("operator")]
    public required string Operator { get; set; }

    /// <summary>
    /// Gets or sets the value to filter by. Can be a string, number, boolean, or array for certain operators/custom field types.
    /// For 'IS NULL' or 'IS NOT NULL', this can be omitted or null.
    /// For dropdowns, this would be the UUID of the option. For labels, an array of label names.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("value")]
    public object? Value { get; set; }
}
