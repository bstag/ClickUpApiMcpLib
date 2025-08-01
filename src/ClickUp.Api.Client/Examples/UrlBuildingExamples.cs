using System.Collections.Generic;
using ClickUp.Api.Client.Helpers;
using ClickUp.Api.Client.Abstractions.Helpers;

namespace ClickUp.Api.Client.Examples
{
    /// <summary>
    /// Examples demonstrating the new URL building utilities and patterns.
    /// This class shows how to refactor existing URL construction code.
    /// </summary>
    public static class UrlBuildingExamples
    {
        // Example base endpoints
        private const string TasksEndpoint = "/api/v2/list/{listId}/task";
        private const string TaskEndpoint = "/api/v2/task/{taskId}";
        private const string CommentsEndpoint = "/api/v2/task/{taskId}/comment";

        /// <summary>
        /// Example: Old way of building URLs with string concatenation and manual query building.
        /// </summary>
        public static class OldWay
        {
            public static string BuildTasksUrl(string listId, bool? archived = null, int? page = null, string? orderBy = null)
            {
                var url = $"/api/v2/list/{listId}/task";
                var queryParams = new List<string>();
                
                if (archived.HasValue)
                    queryParams.Add($"archived={archived.Value.ToString().ToLower()}");
                if (page.HasValue)
                    queryParams.Add($"page={page.Value}");
                if (!string.IsNullOrEmpty(orderBy))
                    queryParams.Add($"order_by={orderBy}");
                
                if (queryParams.Count > 0)
                    url += "?" + string.Join("&", queryParams);
                
                return url;
            }

            public static string BuildTaskUrl(string taskId)
            {
                return $"/api/v2/task/{taskId}";
            }

            public static string BuildCommentsUrl(string taskId, int? start = null, bool? reverse = null)
            {
                var url = $"/api/v2/task/{taskId}/comment";
                var queryParams = new List<string>();
                
                if (start.HasValue)
                    queryParams.Add($"start={start.Value}");
                if (reverse.HasValue)
                    queryParams.Add($"reverse={reverse.Value.ToString().ToLower()}");
                
                if (queryParams.Count > 0)
                    url += "?" + string.Join("&", queryParams);
                
                return url;
            }
        }

        /// <summary>
        /// Example: New way using fluent URL builder.
        /// </summary>
        public static class FluentBuilderWay
        {
            public static string BuildTasksUrl(string listId, bool? archived = null, int? page = null, string? orderBy = null)
            {
                return UrlBuilderHelper.CreateBuilder($"/api/v2/list/{listId}/task")
                    .WithQueryParameter("archived", archived)
                    .WithQueryParameter("page", page)
                    .WithQueryParameterIfNotEmpty("order_by", orderBy)
                    .Build();
            }

            public static string BuildTaskUrl(string taskId)
            {
                return UrlBuilderHelper.CreateBuilder("/api/v2/task")
                    .WithPathSegment(taskId)
                    .Build();
            }

            public static string BuildCommentsUrl(string taskId, int? start = null, bool? reverse = null)
            {
                return UrlBuilderHelper.CreateBuilder($"/api/v2/task/{taskId}/comment")
                    .WithQueryParameter("start", start)
                    .WithQueryParameter("reverse", reverse)
                    .Build();
            }
        }

        /// <summary>
        /// Example: New way using URL templates for parameterized endpoints.
        /// </summary>
        public static class TemplateWay
        {
            public static string BuildTasksUrl(string listId, bool? archived = null, int? page = null, string? orderBy = null)
            {
                return UrlBuilderHelper.CreateTemplate(TasksEndpoint)
                    .WithParameter("listId", listId)
                    .ToUrlBuilder()
                    .WithQueryParameter("archived", archived)
                    .WithQueryParameter("page", page)
                    .WithQueryParameterIfNotEmpty("order_by", orderBy)
                    .Build();
            }

            public static string BuildTaskUrl(string taskId)
            {
                return UrlBuilderHelper.CreateTemplate(TaskEndpoint)
                    .WithParameter("taskId", taskId)
                    .Build();
            }

            public static string BuildCommentsUrl(string taskId, int? start = null, bool? reverse = null)
            {
                return UrlBuilderHelper.CreateTemplate(CommentsEndpoint)
                    .WithParameter("taskId", taskId)
                    .ToUrlBuilder()
                    .WithQueryParameter("start", start)
                    .WithQueryParameter("reverse", reverse)
                    .Build();
            }
        }

        /// <summary>
        /// Example: Advanced usage with complex query parameters.
        /// </summary>
        public static class AdvancedExamples
        {
            public static string BuildTasksWithFilters(string listId, string[] assignees, string[] statuses, string[] tags)
            {
                return UrlBuilderHelper.CreateTemplate(TasksEndpoint)
                    .WithParameter("listId", listId)
                    .ToUrlBuilder()
                    .WithArrayQueryParameter("assignees", assignees)
                    .WithArrayQueryParameter("statuses", statuses)
                    .WithCommaSeparatedQueryParameter("tags", tags)
                    .Build();
            }

            public static string BuildTasksWithConditionalParams(string listId, bool includeArchived, bool isManager)
            {
                return UrlBuilderHelper.CreateTemplate(TasksEndpoint)
                    .WithParameter("listId", listId)
                    .ToUrlBuilder()
                    .WithQueryParameterIf(includeArchived, "archived", "true")
                    .WithQueryParameterIf(isManager, "include_closed", "true")
                    .WithQueryParameterIf(isManager, "subtasks", "true")
                    .Build();
            }

            public static string BuildComplexUrl()
            {
                var parameters = new Dictionary<string, string>
                {
                    { "teamId", "123" },
                    { "spaceId", "456" }
                };

                return UrlBuilderHelper.CreateBuilderFromTemplate("/api/v2/team/{teamId}/space/{spaceId}/folder", parameters)
                    .WithQueryParameter("archived", false)
                    .WithQueryParameter("page", 1)
                    .Build();
            }
        }

        /// <summary>
        /// Example: Utility methods for common patterns.
        /// </summary>
        public static class UtilityExamples
        {
            public static string CombineEndpointPaths()
            {
                // Safely combine path segments
                return UrlBuilderHelper.CombinePath("/api/v2", "team", "123", "space", "456");
            }

            public static bool ValidateUrl(string url)
            {
                return UrlBuilderHelper.IsValidUrl(url);
            }

            public static Dictionary<string, string> ParseQueryString(string url)
            {
                return UrlBuilderHelper.ExtractQueryParameters(url);
            }
        }
    }
}