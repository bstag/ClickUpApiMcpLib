using System;
using System.Collections.Generic;

namespace ClickUp.Api.Client.Abstractions.Helpers
{
    /// <summary>
    /// Interface for building URLs with fluent API support.
    /// </summary>
    public interface IUrlBuilder
    {
        /// <summary>
        /// Sets the base endpoint for the URL.
        /// </summary>
        /// <param name="endpoint">The base endpoint.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithEndpoint(string endpoint);

        /// <summary>
        /// Adds a path segment to the URL.
        /// </summary>
        /// <param name="segment">The path segment to add.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithPathSegment(string segment);

        /// <summary>
        /// Adds multiple path segments to the URL.
        /// </summary>
        /// <param name="segments">The path segments to add.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithPathSegments(params string[] segments);

        /// <summary>
        /// Adds a query parameter to the URL.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithQueryParameter(string key, string? value);

        /// <summary>
        /// Adds a query parameter to the URL if the value is not null or empty.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithQueryParameterIfNotEmpty(string key, string? value);

        /// <summary>
        /// Adds a boolean query parameter to the URL.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The boolean value.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithQueryParameter(string key, bool? value);

        /// <summary>
        /// Adds an integer query parameter to the URL.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The integer value.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithQueryParameter(string key, int? value);

        /// <summary>
        /// Adds an enum query parameter to the URL.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The enum value.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithQueryParameter<TEnum>(string key, TEnum? value) where TEnum : struct, Enum;

        /// <summary>
        /// Adds multiple query parameters from a dictionary.
        /// </summary>
        /// <param name="parameters">The parameters to add.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithQueryParameters(Dictionary<string, string?> parameters);

        /// <summary>
        /// Adds multiple query parameters from a list of key-value pairs.
        /// </summary>
        /// <param name="parameters">The parameters to add.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithQueryParameters(List<KeyValuePair<string, string>> parameters);

        /// <summary>
        /// Adds array-style query parameters (with [] notation).
        /// </summary>
        /// <typeparam name="T">The type of values in the array.</typeparam>
        /// <param name="key">The parameter key.</param>
        /// <param name="values">The array of values.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithArrayQueryParameter<T>(string key, IEnumerable<T>? values);

        /// <summary>
        /// Adds comma-separated query parameter values.
        /// </summary>
        /// <typeparam name="T">The type of values.</typeparam>
        /// <param name="key">The parameter key.</param>
        /// <param name="values">The values to join with commas.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithCommaSeparatedQueryParameter<T>(string key, IEnumerable<T>? values);

        /// <summary>
        /// Conditionally adds a query parameter based on a predicate.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder WithQueryParameterIf(bool condition, string key, string? value);

        /// <summary>
        /// Validates the current URL configuration.
        /// </summary>
        /// <returns>The URL builder instance for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the URL configuration is invalid.</exception>
        IUrlBuilder Validate();

        /// <summary>
        /// Builds the final URL string.
        /// </summary>
        /// <returns>The constructed URL.</returns>
        string Build();

        /// <summary>
        /// Resets the URL builder to its initial state.
        /// </summary>
        /// <returns>The URL builder instance for method chaining.</returns>
        IUrlBuilder Reset();
    }
}