// In file: src/ClickUp.Api.Client.Models/RequestModels/Tasks/MergeTasksRequest.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Tasks
{
    public class MergeTasksRequest
    {
        [JsonPropertyName("source_task_ids")]
        public List<string> SourceTaskIds { get; set; }
    }
}
