using ClickUp.Api.Client.Abstractions.Options;
using System;
using System.Text.RegularExpressions;

namespace ClickUp.Api.Client.Helpers
{
    /// <summary>
    /// Provides validation utilities for ClickUp API client inputs.
    /// </summary>
    public static class ValidationHelper
    {
        private static readonly Regex IdFormatRegex = new(@"^[a-zA-Z0-9_-]+$", RegexOptions.Compiled);

        /// <summary>
        /// Validates a ClickUp entity ID (list, task, space, etc.).
        /// </summary>
        /// <param name="id">The ID to validate.</param>
        /// <param name="paramName">The parameter name for exception messages.</param>
        /// <exception cref="ArgumentNullException">Thrown if the ID is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if the ID format is invalid or too long.</exception>
        public static void ValidateId(string? id, string paramName)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null or whitespace.");

            if (id.Length > ClickUpDefaults.MaxIdLength)
                throw new ArgumentException($"{paramName} exceeds maximum length of {ClickUpDefaults.MaxIdLength} characters.", paramName);

            if (!IdFormatRegex.IsMatch(id))
                throw new ArgumentException($"{paramName} contains invalid characters. Only alphanumeric characters, hyphens, and underscores are allowed.", paramName);
        }

        /// <summary>
        /// Validates a ClickUp entity ID with a custom maximum length.
        /// </summary>
        /// <param name="id">The ID to validate.</param>
        /// <param name="paramName">The parameter name for exception messages.</param>
        /// <param name="maxLength">The maximum allowed length.</param>
        /// <exception cref="ArgumentNullException">Thrown if the ID is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if the ID format is invalid or too long.</exception>
        public static void ValidateId(string? id, string paramName, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null or whitespace.");

            if (id.Length > maxLength)
                throw new ArgumentException($"{paramName} exceeds maximum length of {maxLength} characters.", paramName);

            if (!IdFormatRegex.IsMatch(id))
                throw new ArgumentException($"{paramName} contains invalid characters. Only alphanumeric characters, hyphens, and underscores are allowed.", paramName);
        }

        /// <summary>
        /// Validates a required string parameter.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="paramName">The parameter name for exception messages.</param>
        /// <exception cref="ArgumentNullException">Thrown if the value is null or whitespace.</exception>
        public static void ValidateRequiredString(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null or whitespace.");
        }

        /// <summary>
        /// Validates a required string parameter with maximum length.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="paramName">The parameter name for exception messages.</param>
        /// <param name="maxLength">The maximum allowed length.</param>
        /// <exception cref="ArgumentNullException">Thrown if the value is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if the value is too long.</exception>
        public static void ValidateRequiredString(string? value, string paramName, int maxLength)
        {
            ValidateRequiredString(value, paramName);

            if (value!.Length > maxLength)
                throw new ArgumentException($"{paramName} exceeds maximum length of {maxLength} characters.", paramName);
        }

        /// <summary>
        /// Validates that a numeric value is within a specified range.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="paramName">The parameter name for exception messages.</param>
        /// <param name="min">The minimum allowed value (inclusive).</param>
        /// <param name="max">The maximum allowed value (inclusive).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is outside the specified range.</exception>
        public static void ValidateRange(int value, string paramName, int min, int max)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be between {min} and {max} (inclusive).");
        }

        /// <summary>
        /// Validates that a TimeSpan value is within a specified range.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="paramName">The parameter name for exception messages.</param>
        /// <param name="min">The minimum allowed value (inclusive).</param>
        /// <param name="max">The maximum allowed value (inclusive).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is outside the specified range.</exception>
        public static void ValidateRange(TimeSpan value, string paramName, TimeSpan min, TimeSpan max)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be between {min} and {max} (inclusive).");
        }
    }
}