using ClickUp.Api.Client.Models.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Models.RequestModels.Parameters
{
    public class GetTasksRequestParameters
    {
        public IEnumerable<long>? SpaceIds { get; set; }
        public IEnumerable<long>? ProjectIds { get; set; } // In ClickUp, these are Folders
        public IEnumerable<long>? ListIds { get; set; }
        public IEnumerable<string>? Statuses { get; set; }
        public IEnumerable<long>? Assignees { get; set; }
        public bool? IncludeClosed { get; set; }
        public bool? Subtasks { get; set; }
        public TimeRange? DueDateRange { get; set; }
        public TimeRange? DateCreatedRange { get; set; }
        public TimeRange? DateUpdatedRange { get; set; }
        public SortOption? SortBy { get; set; }
        public int? Page { get; set; }
        public bool? Archived { get; set; } // Added based on common API patterns
        public IEnumerable<CustomFieldParameter>? CustomFields { get; set; } // Added for custom field filtering

        public Dictionary<string, string> ToDictionary()
        {
            var queryParams = new Dictionary<string, string>();

            if (SpaceIds?.Any() == true) queryParams["space_ids[]"] = string.Join(",", SpaceIds);
            if (ProjectIds?.Any() == true) queryParams["project_ids[]"] = string.Join(",", ProjectIds); // map to folder_ids if necessary based on API
            if (ListIds?.Any() == true) queryParams["list_ids[]"] = string.Join(",", ListIds);
            if (Statuses?.Any() == true) queryParams["statuses[]"] = string.Join(",", Statuses.Select(s => Uri.EscapeDataString(s)));
            if (Assignees?.Any() == true) queryParams["assignees[]"] = string.Join(",", Assignees);
            if (IncludeClosed.HasValue) queryParams["include_closed"] = IncludeClosed.Value.ToString().ToLower();
            if (Subtasks.HasValue) queryParams["subtasks"] = Subtasks.Value.ToString().ToLower();
            if (Page.HasValue) queryParams["page"] = Page.Value.ToString();
            if (Archived.HasValue) queryParams["archived"] = Archived.Value.ToString().ToLower();

            if (DueDateRange != null)
            {
                foreach (var kvp in DueDateRange.ToQueryParameters("due_date_gt", "due_date_lt"))
                {
                    queryParams[kvp.Key] = kvp.Value;
                }
            }

            if (DateCreatedRange != null)
            {
                foreach (var kvp in DateCreatedRange.ToQueryParameters("date_created_gt", "date_created_lt"))
                {
                    queryParams[kvp.Key] = kvp.Value;
                }
            }

            if (DateUpdatedRange != null)
            {
                foreach (var kvp in DateUpdatedRange.ToQueryParameters("date_updated_gt", "date_updated_lt"))
                {
                    queryParams[kvp.Key] = kvp.Value;
                }
            }

            if (SortBy != null)
            {
                // Assuming ClickUp's GetTasks uses order_by and reverse
                foreach (var kvp in SortBy.ToQueryParameters("order_by", "reverse"))
                {
                    queryParams[kvp.Key] = kvp.Value;
                }
            }

            if (CustomFields?.Any() == true)
            {
                // Format: custom_fields=[{"id":"field_id","value":"field_value"},{"id":"field_id","operator":">","value":"field_value"}]
                // This basic implementation assumes direct value matching. More complex operators would need a richer CustomFieldParameter model.
                var customFieldList = new List<string>();
                foreach (var cf in CustomFields)
                {
                    customFieldList.Add($"{{\"id\":\"{cf.Id}\",\"value\":\"{Uri.EscapeDataString(cf.Value)}\"}}");
                }
                queryParams["custom_fields"] = $"[{string.Join(",", customFieldList)}]";
            }

            return queryParams;
        }
    }

    // Basic custom field parameter representation
    public record CustomFieldParameter(string Id, string Value); // Add operator if needed
}
