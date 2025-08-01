using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ClickUp.Api.Client.Abstractions.Helpers;

namespace ClickUp.Api.Client.Helpers
{
    /// <summary>
    /// URL template implementation for handling parameterized URLs with placeholder substitution.
    /// </summary>
    public class UrlTemplate : IUrlTemplate
    {
        private readonly string _template;
        private readonly Dictionary<string, string> _parameters;
        private readonly Regex _parameterRegex;
        private readonly List<string> _parameterNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlTemplate"/> class.
        /// </summary>
        /// <param name="template">The URL template with parameters in {name} format.</param>
        public UrlTemplate(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentException("Template cannot be null or empty.", nameof(template));

            _template = template;
            _parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _parameterRegex = new Regex(@"\{([^}]+)\}", RegexOptions.Compiled);
            _parameterNames = ExtractParameterNames();
        }

        /// <inheritdoc />
        public IUrlTemplate WithParameter(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Parameter name cannot be null or empty.", nameof(name));
            
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _parameters[name] = Uri.EscapeDataString(value);
            return this;
        }

        /// <inheritdoc />
        public IUrlTemplate WithParameterIfNotEmpty(string name, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                WithParameter(name, value);
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlTemplate WithParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null)
                return this;

            foreach (var kvp in parameters)
            {
                WithParameter(kvp.Key, kvp.Value);
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlTemplate WithParameter(string name, int value)
        {
            return WithParameter(name, value.ToString());
        }

        /// <inheritdoc />
        public IUrlTemplate WithParameter(string name, long value)
        {
            return WithParameter(name, value.ToString());
        }

        /// <inheritdoc />
        public IUrlTemplate Validate()
        {
            var missingParameters = GetMissingParameters();
            if (missingParameters.Any())
            {
                throw new InvalidOperationException(
                    $"Missing required parameters: {string.Join(", ", missingParameters)}");
            }
            return this;
        }

        /// <inheritdoc />
        public string Build()
        {
            Validate();

            var result = _template;
            foreach (var kvp in _parameters)
            {
                var placeholder = $"{{{kvp.Key}}}";
                result = result.Replace(placeholder, kvp.Value, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetParameterNames()
        {
            return _parameterNames.AsReadOnly();
        }

        /// <inheritdoc />
        public IReadOnlyList<string> GetMissingParameters()
        {
            return _parameterNames
                .Where(name => !_parameters.ContainsKey(name))
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Creates a new URL template instance.
        /// </summary>
        /// <param name="template">The URL template with parameters in {name} format.</param>
        /// <returns>A new UrlTemplate instance.</returns>
        public static IUrlTemplate Create(string template) => new UrlTemplate(template);

        /// <summary>
        /// Extracts parameter names from the template.
        /// </summary>
        /// <returns>A list of parameter names found in the template.</returns>
        private List<string> ExtractParameterNames()
        {
            var matches = _parameterRegex.Matches(_template);
            return matches
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    /// <summary>
    /// Extension methods for URL template creation.
    /// </summary>
    public static class UrlTemplateExtensions
    {
        /// <summary>
        /// Creates a URL template from a string.
        /// </summary>
        /// <param name="template">The URL template string.</param>
        /// <returns>A new IUrlTemplate instance.</returns>
        public static IUrlTemplate AsUrlTemplate(this string template)
        {
            return UrlTemplate.Create(template);
        }

        /// <summary>
        /// Combines a URL template with query parameters using a fluent builder.
        /// </summary>
        /// <param name="template">The URL template.</param>
        /// <returns>A new IUrlBuilder instance with the template as the base endpoint.</returns>
        public static IUrlBuilder ToUrlBuilder(this IUrlTemplate template)
        {
            return UrlBuilderHelper.CreateBuilder(template.Build());
        }
    }
}