using System.Text.Json.Serialization;

namespace ClickUp.Api.Client.Models.RequestModels.Views
{
    /// <summary>
    /// Request model for getting tasks from a view.
    /// </summary>
    public class GetViewTasksRequest
    {
        /// <summary>
        /// Gets or sets the page number for pagination.
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; } = 0; // Default to page 0
    }
}
