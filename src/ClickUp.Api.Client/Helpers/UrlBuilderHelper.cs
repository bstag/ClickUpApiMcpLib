using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClickUp.Api.Client.Helpers
{
    /// <summary>
    /// Provides utility methods for building URLs with query parameters.
    /// </summary>
    public static class UrlBuilderHelper
    {
        /// <summary>
        /// Builds a query string from a list of key-value pairs.
        /// </summary>
        /// <param name="queryParams">The query parameters to include in the URL.</param>
        /// <returns>A properly formatted and URL-encoded query string, or empty string if no parameters.</returns>
        /// <remarks>
        /// This method properly URL-encodes both keys and values to handle special characters
        /// like {, ", spaces, etc. that would otherwise cause malformed URLs.
        /// </remarks>
        public static string BuildQueryString(List<KeyValuePair<string, string>> queryParams)
        {
            if (queryParams == null || !queryParams.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var first = true;
            foreach (var kvp in queryParams)
            {
                // Always escape both key and value to ensure proper URL encoding
                // This handles special characters like {, ", spaces, etc. in parameter values
                sb.Append(first ? '?' : '&');
                first = false;
                sb.Append($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Builds a query string from a dictionary of key-value pairs.
        /// </summary>
        /// <param name="queryParams">The query parameters to include in the URL.</param>
        /// <returns>A properly formatted and URL-encoded query string, or empty string if no parameters.</returns>
        /// <remarks>
        /// This method properly URL-encodes both keys and values to handle special characters
        /// like {, ", spaces, etc. that would otherwise cause malformed URLs.
        /// Null values are skipped.
        /// </remarks>
        public static string BuildQueryString(Dictionary<string, string?> queryParams)
        {
            if (queryParams == null || !queryParams.Any(kvp => kvp.Value != null))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var first = true;
            foreach (var kvp in queryParams)
            {
                if (kvp.Value != null)
                {
                    // Always escape both key and value to ensure proper URL encoding
                    // This handles special characters like {, ", spaces, etc. in parameter values
                    sb.Append(first ? '?' : '&');
                    first = false;
                    sb.Append($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Builds a query string for array parameters (with [] notation).
        /// </summary>
        /// <typeparam name="T">The type of values in the array.</typeparam>
        /// <param name="key">The parameter key name.</param>
        /// <param name="values">The array of values.</param>
        /// <returns>A properly formatted and URL-encoded query string for array parameters, or empty string if no values.</returns>
        /// <remarks>
        /// This method creates array-style query parameters with [] notation (e.g., "key[]=value1&amp;key[]=value2").
        /// Null values are converted to empty strings.
        /// </remarks>
        public static string BuildQueryStringFromArray<T>(string key, IEnumerable<T>? values)
        {
            if (values == null || !values.Any()) return string.Empty;
            return string.Join("&", values.Select(v => $"{Uri.EscapeDataString(key)}[]={Uri.EscapeDataString(v?.ToString() ?? string.Empty)}"));
        }
    }
}