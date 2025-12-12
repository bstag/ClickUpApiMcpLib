using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClickUp.Api.Client.Abstractions.Helpers;

namespace ClickUp.Api.Client.Helpers
{
    /// <summary>
    /// Provides utility methods for building URLs with query parameters.
    /// </summary>
    public static class UrlBuilderHelper
    {
        /// <summary>
        /// Builds a query string from a list of key-value pairs where values are already URL-encoded.
        /// </summary>
        /// <param name="queryParams">The query parameters to include in the URL.</param>
        /// <returns>A properly formatted query string, or empty string if no parameters.</returns>
        /// <remarks>
        /// This method assumes parameter values are already URL-encoded (e.g., from ToQueryParametersList()).
        /// Only the keys are URL-encoded to prevent double-encoding of values.
        /// </remarks>
        public static string BuildQueryString(List<KeyValuePair<string, string>> queryParams)
        {
            if (queryParams == null || !queryParams.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            AppendQueryString(sb, queryParams);
            return sb.ToString();
        }

        /// <summary>
        /// Appends query parameters to the provided StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder to append to.</param>
        /// <param name="queryParams">The query parameters to include in the URL.</param>
        public static void AppendQueryString(StringBuilder sb, List<KeyValuePair<string, string>> queryParams)
        {
            if (queryParams == null || !queryParams.Any())
            {
                return;
            }

            var first = true;
            foreach (var kvp in queryParams)
            {
                // Escape only the key, as parameter values should be pre-encoded by the parameter building classes
                // This prevents double-encoding while ensuring URL structure is correct
                sb.Append(first ? '?' : '&');
                first = false;
                sb.Append(Uri.EscapeDataString(kvp.Key)).Append('=').Append(kvp.Value);
            }
        }

        /// <summary>
        /// Builds a query string from a dictionary of key-value pairs with full URL encoding.
        /// </summary>
        /// <param name="queryParams">The query parameters to include in the URL.</param>
        /// <returns>A properly formatted and URL-encoded query string, or empty string if no parameters.</returns>
        /// <remarks>
        /// This method properly URL-encodes both keys and values to handle special characters
        /// like {, ", spaces, etc. that would otherwise cause malformed URLs.
        /// Null values are skipped. Use this method when values are raw and need encoding.
        /// </remarks>
        public static string BuildQueryString(Dictionary<string, string?> queryParams)
        {
            if (queryParams == null || !queryParams.Any(kvp => kvp.Value != null))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            AppendQueryString(sb, queryParams);
            return sb.ToString();
        }

        /// <summary>
        /// Appends query parameters to the provided StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder to append to.</param>
        /// <param name="queryParams">The query parameters to include in the URL.</param>
        public static void AppendQueryString(StringBuilder sb, Dictionary<string, string?> queryParams)
        {
            if (queryParams == null || !queryParams.Any(kvp => kvp.Value != null))
            {
                return;
            }

            var first = true;
            foreach (var kvp in queryParams)
            {
                if (kvp.Value != null)
                {
                    // Escape both key and value for services that pass raw values
                    sb.Append(first ? '?' : '&');
                    first = false;
                    sb.Append(Uri.EscapeDataString(kvp.Key)).Append('=').Append(Uri.EscapeDataString(kvp.Value));
                }
            }
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

        /// <summary>
        /// Creates a new fluent URL builder instance.
        /// </summary>
        /// <returns>A new IUrlBuilder instance for fluent URL construction.</returns>
        public static IUrlBuilder CreateBuilder() => FluentUrlBuilder.Create();

        /// <summary>
        /// Creates a new fluent URL builder instance with a base endpoint.
        /// </summary>
        /// <param name="baseEndpoint">The base endpoint to start with.</param>
        /// <returns>A new IUrlBuilder instance for fluent URL construction.</returns>
        public static IUrlBuilder CreateBuilder(string baseEndpoint) => FluentUrlBuilder.Create(baseEndpoint);

        /// <summary>
        /// Combines a base endpoint with path segments.
        /// </summary>
        /// <param name="baseEndpoint">The base endpoint.</param>
        /// <param name="pathSegments">The path segments to append.</param>
        /// <returns>The combined URL path.</returns>
        public static string CombinePath(string baseEndpoint, params string[] pathSegments)
        {
            if (string.IsNullOrWhiteSpace(baseEndpoint))
                throw new ArgumentException("Base endpoint cannot be null or empty.", nameof(baseEndpoint));

            var builder = new StringBuilder(baseEndpoint.TrimEnd('/'));

            if (pathSegments != null)
            {
                foreach (var segment in pathSegments)
                {
                    if (!string.IsNullOrWhiteSpace(segment))
                    {
                        builder.Append('/');
                        builder.Append(segment.Trim('/'));
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Validates that a URL string is properly formatted.
        /// </summary>
        /// <param name="url">The URL to validate.</param>
        /// <returns>True if the URL is valid, false otherwise.</returns>
        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _);
        }

        /// <summary>
        /// Extracts query parameters from a URL string.
        /// </summary>
        /// <param name="url">The URL containing query parameters.</param>
        /// <returns>A dictionary of query parameters.</returns>
        public static Dictionary<string, string> ExtractQueryParameters(string url)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(url))
                return result;

            var queryIndex = url.IndexOf('?');
            if (queryIndex == -1)
                return result;

            var queryString = url.Substring(queryIndex + 1);
            var pairs = queryString.Split('&');

            foreach (var pair in pairs)
            {
                var idx = pair.IndexOf('=');
                if (idx >= 0)
                {
                    var key = Uri.UnescapeDataString(pair.Substring(0, idx));
                    var value = Uri.UnescapeDataString(pair.Substring(idx + 1));
                    result[key] = value;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a new URL template instance.
        /// </summary>
        /// <param name="template">The URL template with parameters in {name} format.</param>
        /// <returns>A new IUrlTemplate instance for parameterized URL construction.</returns>
        public static IUrlTemplate CreateTemplate(string template) => UrlTemplate.Create(template);

        /// <summary>
        /// Creates a URL builder from a template after parameter substitution.
        /// </summary>
        /// <param name="template">The URL template.</param>
        /// <param name="parameters">The parameters to substitute in the template.</param>
        /// <returns>A new IUrlBuilder instance with the template as the base endpoint.</returns>
        public static IUrlBuilder CreateBuilderFromTemplate(string template, Dictionary<string, string> parameters)
        {
            var urlTemplate = CreateTemplate(template).WithParameters(parameters);
            return CreateBuilder(urlTemplate.Build());
        }
    }
}
