using ClickUp.Api.Client.Abstractions.Options;
using ClickUp.Api.Client.Validation;
using ClickUp.Api.Client.Validation.Attributes;
using System;
using System.Text.RegularExpressions;

namespace ClickUp.Api.Client.Helpers
{
    /// <summary>
    /// Provides validation utilities for ClickUp API client inputs.
    /// This class maintains backward compatibility while leveraging the new validation framework.
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
            var attribute = new ClickUpIdAttribute();
            var result = attribute.Validate(id, paramName);
            
            if (!result.IsValid)
            {
                var error = result.Errors[0];
                if (id == null)
                    throw new ArgumentNullException(paramName, error.ErrorMessage);
                else
                    throw new ArgumentException(error.ErrorMessage, paramName);
            }
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
            var attribute = new ClickUpIdAttribute { MaxLength = maxLength };
            var result = attribute.Validate(id, paramName);
            
            if (!result.IsValid)
            {
                var error = result.Errors[0];
                if (id == null)
                    throw new ArgumentNullException(paramName, error.ErrorMessage);
                else
                    throw new ArgumentException(error.ErrorMessage, paramName);
            }
        }

        /// <summary>
        /// Validates a required string parameter.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="paramName">The parameter name for exception messages.</param>
        /// <exception cref="ArgumentNullException">Thrown if the value is null or whitespace.</exception>
        public static void ValidateRequiredString(string? value, string paramName)
        {
            var attribute = new RequiredAttribute();
            var result = attribute.Validate(value, paramName);
            
            if (!result.IsValid)
            {
                var error = result.Errors[0];
                throw new ArgumentNullException(paramName, error.ErrorMessage);
            }
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
            var requiredResult = Validation.ValidationHelper.ValidateValue(value, paramName, 
                new RequiredAttribute(), 
                new StringLengthAttribute(maxLength));
            
            if (!requiredResult.IsValid)
            {
                var error = requiredResult.Errors[0];
                if (value == null)
                    throw new ArgumentNullException(paramName, error.ErrorMessage);
                else
                    throw new ArgumentException(error.ErrorMessage, paramName);
            }
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
            var attribute = new RangeAttribute(min, max);
            var result = attribute.Validate(value, paramName);
            
            if (!result.IsValid)
            {
                var error = result.Errors[0];
                throw new ArgumentOutOfRangeException(paramName, value, error.ErrorMessage);
            }
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
            var key = new TimeSpanRangeKey(min, max);
            var attribute = TimeSpanRangeAttributeCache.GetOrAdd(key, k => new RangeAttribute(typeof(TimeSpan), k.Min.ToString(), k.Max.ToString()));
            var result = attribute.Validate(value, paramName);
            
            if (!result.IsValid)
            {
                var error = result.Errors[0];
                throw new ArgumentOutOfRangeException(paramName, value, error.ErrorMessage);
            }
        }

        /// <summary>
        /// Creates a fluent validation builder for the specified object.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="obj">The object to validate.</param>
        /// <returns>A fluent validation builder.</returns>
        public static FluentValidationBuilder<T> For<T>(T obj)
        {
            return Validation.ValidationHelper.For(obj);
        }

        /// <summary>
        /// Validates an object using the new validation framework.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="obj">The object to validate.</param>
        /// <param name="throwOnFailure">Whether to throw an exception on validation failure.</param>
        /// <returns>A validation result.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails and throwOnFailure is true.</exception>
        public static ValidationResult Validate<T>(T obj, bool throwOnFailure = true)
        {
            return Validation.ValidationHelper.Validate(obj, throwOnFailure);
        }
    }
}