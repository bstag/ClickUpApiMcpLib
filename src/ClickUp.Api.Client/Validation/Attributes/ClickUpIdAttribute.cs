using System;
using System.Text.RegularExpressions;
using ClickUp.Api.Client.Abstractions.Options;

namespace ClickUp.Api.Client.Validation.Attributes
{
    /// <summary>
    /// Specifies that a property value must be a valid ClickUp entity ID.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ClickUpIdAttribute : ValidationAttribute
    {
        private static readonly Regex IdFormatRegex = new(@"^[a-zA-Z0-9_-]+$", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpIdAttribute"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message template.</param>
        public ClickUpIdAttribute(string? errorMessage = null) : base(errorMessage)
        {
            MaxLength = ClickUpDefaults.MaxIdLength;
        }

        /// <summary>
        /// Gets or sets the maximum length of the ID.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether null values are allowed.
        /// </summary>
        public bool AllowNull { get; set; } = false;

        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="propertyName">The name of the property being validated.</param>
        /// <returns>A validation result.</returns>
        public override ValidationResult Validate(object? value, string propertyName)
        {
            if (value == null)
            {
                return AllowNull ? ValidationResult.Success() : 
                    ValidationResult.Failure(propertyName, $"{propertyName} cannot be null.", ErrorCode);
            }

            if (value is not string stringValue)
            {
                return ValidationResult.Failure(propertyName, $"{propertyName} must be a string.", ErrorCode);
            }

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return ValidationResult.Failure(propertyName, $"{propertyName} cannot be empty or whitespace.", ErrorCode);
            }

            if (stringValue.Length > MaxLength)
            {
                return ValidationResult.Failure(propertyName, 
                    $"{propertyName} exceeds maximum length of {MaxLength} characters.", ErrorCode);
            }

            if (!IdFormatRegex.IsMatch(stringValue))
            {
                return ValidationResult.Failure(propertyName, 
                    $"{propertyName} contains invalid characters. Only alphanumeric characters, hyphens, and underscores are allowed.", 
                    ErrorCode);
            }

            return ValidationResult.Success();
        }

        /// <summary>
        /// Gets the default error message for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The default error message.</returns>
        protected override string GetDefaultErrorMessage(string propertyName)
        {
            return $"{propertyName} must be a valid ClickUp ID.";
        }
    }
}