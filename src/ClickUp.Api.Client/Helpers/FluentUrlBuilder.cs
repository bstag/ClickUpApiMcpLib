using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ClickUp.Api.Client.Abstractions.Helpers;

namespace ClickUp.Api.Client.Helpers
{
    /// <summary>
    /// Fluent URL builder implementation for constructing URLs with method chaining.
    /// </summary>
    public class FluentUrlBuilder : IUrlBuilder
    {
        private readonly StringBuilder _pathBuilder;
        private readonly List<KeyValuePair<string, string>> _queryParameters;
        private string? _baseEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentUrlBuilder"/> class.
        /// </summary>
        public FluentUrlBuilder()
        {
            _pathBuilder = new StringBuilder();
            _queryParameters = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentUrlBuilder"/> class with a base endpoint.
        /// </summary>
        /// <param name="baseEndpoint">The base endpoint to start with.</param>
        public FluentUrlBuilder(string baseEndpoint) : this()
        {
            _baseEndpoint = baseEndpoint;
        }

        /// <inheritdoc />
        public IUrlBuilder WithEndpoint(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint cannot be null or empty.", nameof(endpoint));

            _baseEndpoint = endpoint.TrimEnd('/');
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithPathSegment(string segment)
        {
            if (string.IsNullOrWhiteSpace(segment))
                return this;

            var cleanSegment = segment.Trim('/').Trim();
            if (!string.IsNullOrEmpty(cleanSegment))
            {
                if (_pathBuilder.Length > 0)
                    _pathBuilder.Append('/');
                _pathBuilder.Append(Uri.EscapeDataString(cleanSegment));
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithPathSegments(params string[] segments)
        {
            if (segments == null || segments.Length == 0)
                return this;

            foreach (var segment in segments)
            {
                WithPathSegment(segment);
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithQueryParameter(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Query parameter key cannot be null or empty.", nameof(key));

            if (value != null)
            {
                _queryParameters.Add(new KeyValuePair<string, string>(key, value));
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithQueryParameterIfNotEmpty(string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                WithQueryParameter(key, value);
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithQueryParameter(string key, bool? value)
        {
            if (value.HasValue)
            {
                WithQueryParameter(key, value.Value.ToString().ToLowerInvariant());
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithQueryParameter(string key, int? value)
        {
            if (value.HasValue)
            {
                WithQueryParameter(key, value.Value.ToString());
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithQueryParameter<TEnum>(string key, TEnum? value) where TEnum : struct, Enum
        {
            if (value.HasValue)
            {
                WithQueryParameter(key, value.Value.ToString().ToLowerInvariant());
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithQueryParameters(Dictionary<string, string?> parameters)
        {
            if (parameters == null)
                return this;

            foreach (var kvp in parameters)
            {
                WithQueryParameter(kvp.Key, kvp.Value);
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithQueryParameters(List<KeyValuePair<string, string>> parameters)
        {
            if (parameters == null)
                return this;

            foreach (var kvp in parameters)
            {
                WithQueryParameter(kvp.Key, kvp.Value);
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithArrayQueryParameter<T>(string key, IEnumerable<T>? values)
        {
            if (values == null)
                return this;

            foreach (var value in values)
            {
                if (value != null)
                {
                    WithQueryParameter($"{key}[]", value.ToString());
                }
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithCommaSeparatedQueryParameter<T>(string key, IEnumerable<T>? values)
        {
            if (values == null)
                return this;

            var valueStrings = values.Where(v => v != null).Select(v => v!.ToString()).ToList();
            if (valueStrings.Any())
            {
                WithQueryParameter(key, string.Join(",", valueStrings));
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder WithQueryParameterIf(bool condition, string key, string? value)
        {
            if (condition)
            {
                WithQueryParameter(key, value);
            }
            return this;
        }

        /// <inheritdoc />
        public IUrlBuilder Validate()
        {
            if (string.IsNullOrWhiteSpace(_baseEndpoint))
                throw new InvalidOperationException("Base endpoint must be set before building the URL.");

            return this;
        }

        /// <inheritdoc />
        public string Build()
        {
            Validate();

            var urlBuilder = new StringBuilder(_baseEndpoint!);

            // Add path segments
            if (_pathBuilder.Length > 0)
            {
                if (!_baseEndpoint!.EndsWith("/"))
                    urlBuilder.Append('/');
                urlBuilder.Append(_pathBuilder.ToString());
            }

            // Add query parameters
            if (_queryParameters.Any())
            {
                urlBuilder.Append('?');
                var queryString = UrlBuilderHelper.BuildQueryString(_queryParameters);
                urlBuilder.Append(queryString);
            }

            return urlBuilder.ToString();
        }

        /// <inheritdoc />
        public IUrlBuilder Reset()
        {
            _baseEndpoint = null;
            _pathBuilder.Clear();
            _queryParameters.Clear();
            return this;
        }

        /// <summary>
        /// Creates a new instance of FluentUrlBuilder.
        /// </summary>
        /// <returns>A new FluentUrlBuilder instance.</returns>
        public static IUrlBuilder Create() => new FluentUrlBuilder();

        /// <summary>
        /// Creates a new instance of FluentUrlBuilder with a base endpoint.
        /// </summary>
        /// <param name="baseEndpoint">The base endpoint to start with.</param>
        /// <returns>A new FluentUrlBuilder instance.</returns>
        public static IUrlBuilder Create(string baseEndpoint) => new FluentUrlBuilder(baseEndpoint);
    }
}