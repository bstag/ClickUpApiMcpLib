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
    public IEnumerable<string>? ListIds { get; set; } // Changed from long to string
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
    /// <returns>A list of key-value pairs for query parameters.</returns>
    public List<KeyValuePair<string, string>> ToQueryParametersList()
    {
        var parameters = new List<KeyValuePair<string, string>>();

        SpaceIds?.ToList().ForEach(id => parameters.Add(new KeyValuePair<string, string>("space_ids[]", id.ToString())));
        ProjectIds?.ToList().ForEach(id => parameters.Add(new KeyValuePair<string, string>("project_ids[]", id.ToString())));
        ListIds?.ToList().ForEach(id => parameters.Add(new KeyValuePair<string, string>("list_ids[]", id)));
        AssigneeIds?.ToList().ForEach(id => parameters.Add(new KeyValuePair<string, string>("assignees[]", id.ToString())));
        Statuses?.ToList().ForEach(s => parameters.Add(new KeyValuePair<string, string>("statuses[]", Uri.EscapeDataString(s))));
        Tags?.ToList().ForEach(t => parameters.Add(new KeyValuePair<string, string>("tags[]", Uri.EscapeDataString(t))));

        if (IncludeClosed.HasValue) parameters.Add(new KeyValuePair<string, string>("include_closed", IncludeClosed.Value.ToString().ToLowerInvariant()));
        if (Subtasks.HasValue) parameters.Add(new KeyValuePair<string, string>("subtasks", Subtasks.Value.ToString().ToLowerInvariant()));

        if (DueDateRange != null)
        {
            parameters.Add(new KeyValuePair<string, string>("due_date_gt", DueDateRange.StartDate.ToUnixTimeMilliseconds().ToString()));
            parameters.Add(new KeyValuePair<string, string>("due_date_lt", DueDateRange.EndDate.ToUnixTimeMilliseconds().ToString()));
        }
        if (DateCreatedRange != null)
        {
            parameters.Add(new KeyValuePair<string, string>("date_created_gt", DateCreatedRange.StartDate.ToUnixTimeMilliseconds().ToString()));
            parameters.Add(new KeyValuePair<string, string>("date_created_lt", DateCreatedRange.EndDate.ToUnixTimeMilliseconds().ToString()));
        }
        if (DateUpdatedRange != null)
        {
            parameters.Add(new KeyValuePair<string, string>("date_updated_gt", DateUpdatedRange.StartDate.ToUnixTimeMilliseconds().ToString()));
            parameters.Add(new KeyValuePair<string, string>("date_updated_lt", DateUpdatedRange.EndDate.ToUnixTimeMilliseconds().ToString()));
        }

        if (SortBy != null)
        {
            // ToQueryParameters itself returns a Dictionary, adapt this or make SortOption also return List<KVP>
            var sortParams = SortBy.ToQueryParameters("order_by", "reverse");
            foreach (var entry in sortParams)
            {
                parameters.Add(new KeyValuePair<string, string>(entry.Key, entry.Value));
            }
        }

        if (Page.HasValue) parameters.Add(new KeyValuePair<string, string>("page", Page.Value.ToString()));

        if (IncludeMarkdownDescription.HasValue) parameters.Add(new KeyValuePair<string, string>("include_markdown_description", IncludeMarkdownDescription.Value.ToString().ToLowerInvariant()));

        if (CustomFields?.Any() ?? false)
        {
            // Custom fields are expected as a single JSON string parameter by ClickUp API
            parameters.Add(new KeyValuePair<string, string>("custom_fields", JsonSerializer.Serialize(CustomFields)));
        }

        CustomItems?.ToList().ForEach(id => parameters.Add(new KeyValuePair<string, string>("custom_items", id.ToString())));

        if (Archived.HasValue) parameters.Add(new KeyValuePair<string, string>("archived", Archived.Value.ToString().ToLowerInvariant()));

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
