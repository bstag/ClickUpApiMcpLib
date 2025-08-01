using System;

namespace ClickUp.Api.Client.Validation.Attributes
{
    /// <summary>
    /// Specifies the minimum and maximum length of characters that are allowed in a string property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class StringLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthAttribute"/> class.
        /// </summary>
        /// <param name="maximumLength">The maximum length of the string.</param>
        /// <param name="errorMessage">The error message template.</param>
        public StringLengthAttribute(int maximumLength, string? errorMessage = null) : base(errorMessage)
        {
            MaximumLength = maximumLength;
        }

        /// <summary>
        /// Gets the maximum length of the string.
        /// </summary>
        public int MaximumLength { get; }

        /// <summary>
        /// Gets or sets the minimum length of the string.
        /// </summary>
        public int MinimumLength { get; set; } = 0;

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
                return ValidationResult.Success(); // Null values are handled by RequiredAttribute
            }

            if (value is not string stringValue)
            {
                return ValidationResult.Failure(propertyName, $"{propertyName} must be a string.", ErrorCode);
            }

            var length = stringValue.Length;

            if (length < MinimumLength)
            {
                return ValidationResult.Failure(propertyName, FormatMinLengthErrorMessage(propertyName), ErrorCode);
            }

            if (length > MaximumLength)
            {
                return ValidationResult.Failure(propertyName, FormatMaxLengthErrorMessage(propertyName), ErrorCode);
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
            if (MinimumLength > 0)
            {
                return $"{propertyName} must be between {MinimumLength} and {MaximumLength} characters long.";
            }
            return $"{propertyName} must be no more than {MaximumLength} characters long.";
        }

        /// <summary>
        /// Formats the minimum length error message.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The formatted error message.</returns>
        private string FormatMinLengthErrorMessage(string propertyName)
        {
            return $"{propertyName} must be at least {MinimumLength} characters long.";
        }

        /// <summary>
        /// Formats the maximum length error message.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The formatted error message.</returns>
        private string FormatMaxLengthErrorMessage(string propertyName)
        {
            return $"{propertyName} must be no more than {MaximumLength} characters long.";
        }
    }
}