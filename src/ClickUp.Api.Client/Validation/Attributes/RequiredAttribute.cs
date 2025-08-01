using System;

namespace ClickUp.Api.Client.Validation.Attributes
{
    /// <summary>
    /// Specifies that a property value is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class RequiredAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredAttribute"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message template.</param>
        public RequiredAttribute(string? errorMessage = null) : base(errorMessage)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether empty strings are allowed.
        /// </summary>
        public bool AllowEmptyStrings { get; set; } = false;

        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="propertyName">The name of the property being validated.</param>
        /// <returns>A validation result.</returns>
        public override ValidationResult Validate(object? value, string propertyName)
        {
            if (IsValid(value))
            {
                return ValidationResult.Success();
            }

            return ValidationResult.Failure(propertyName, FormatErrorMessage(propertyName), ErrorCode);
        }

        /// <summary>
        /// Determines whether the specified value is valid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>true if the value is valid; otherwise, false.</returns>
        private bool IsValid(object? value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is string stringValue)
            {
                return AllowEmptyStrings || !string.IsNullOrWhiteSpace(stringValue);
            }

            return true;
        }

        /// <summary>
        /// Gets the default error message for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The default error message.</returns>
        protected override string GetDefaultErrorMessage(string propertyName)
        {
            return $"{propertyName} is required.";
        }
    }
}