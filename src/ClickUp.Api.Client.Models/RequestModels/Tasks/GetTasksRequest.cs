using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    /// <summary>
    /// Represents the request parameters for getting tasks.
    /// </summary>
    public class GetTasksRequest
    {
        [JsonPropertyName("archived")]
        public bool? Archived { get; set; }

        [JsonPropertyName("include_markdown_description")]
        public bool? IncludeMarkdownDescription { get; set; }

        [JsonPropertyName("page")]
        public int? Page { get; set; }

        [JsonPropertyName("order_by")]
        public string? OrderBy { get; set; }

        [JsonPropertyName("reverse")]
        public bool? Reverse { get; set; }

        [JsonPropertyName("subtasks")]
        public bool? Subtasks { get; set; }

        [JsonPropertyName("statuses")]
        public IEnumerable<string>? Statuses { get; set; }

        [JsonPropertyName("include_closed")]
        public bool? IncludeClosed { get; set; }

        [JsonPropertyName("assignees")]
        public IEnumerable<string>? Assignees { get; set; }

        [JsonPropertyName("watchers")]
        public IEnumerable<string>? Watchers { get; set; }

        [JsonPropertyName("tags")]
        public IEnumerable<string>? Tags { get; set; }

        [JsonPropertyName("due_date_gt")]
        public long? DueDateGreaterThan { get; set; }

        [JsonPropertyName("due_date_lt")]
        public long? DueDateLessThan { get; set; }

        [JsonPropertyName("date_created_gt")]
        public long? DateCreatedGreaterThan { get; set; }

        [JsonPropertyName("date_created_lt")]
        public long? DateCreatedLessThan { get; set; }

        [JsonPropertyName("date_updated_gt")]
        public long? DateUpdatedGreaterThan { get; set; }

        [JsonPropertyName("date_updated_lt")]
        public long? DateUpdatedLessThan { get; set; }

        [JsonPropertyName("date_done_gt")]
        public long? DateDoneGreaterThan { get; set; }

        [JsonPropertyName("date_done_lt")]
        public long? DateDoneLessThan { get; set; }

        [JsonPropertyName("custom_fields")]
        public string? CustomFields { get; set; } // Assuming JSON string as per ITasksService

        [JsonPropertyName("custom_items")]
        public IEnumerable<long>? CustomItems { get; set; }

        // Constructor to match usage in examples if simple initialization is preferred
        public GetTasksRequest() { }

        // Optional: A constructor could be added if there are common required fields,
        // but for a request DTO with many optional fields, property initializers are common.
    }
}
