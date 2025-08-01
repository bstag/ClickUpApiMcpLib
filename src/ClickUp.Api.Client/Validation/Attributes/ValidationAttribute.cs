using System;

namespace ClickUp.Api.Client.Validation.Attributes
{
    /// <summary>
    /// Base class for validation attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public abstract class ValidationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttribute"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message template.</param>
        protected ValidationAttribute(string? errorMessage = null)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets the error message template.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="propertyName">The name of the property being validated.</param>
        /// <returns>A validation result.</returns>
        public abstract ValidationResult Validate(object? value, string propertyName);

        /// <summary>
        /// Formats the error message for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The formatted error message.</returns>
        protected virtual string FormatErrorMessage(string propertyName)
        {
            return ErrorMessage ?? GetDefaultErrorMessage(propertyName);
        }

        /// <summary>
        /// Gets the default error message for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The default error message.</returns>
        protected abstract string GetDefaultErrorMessage(string propertyName);
    }
}