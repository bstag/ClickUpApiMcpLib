using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ClickUp.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Exception thrown when validation errors occur in the ClickUp API client.
    /// </summary>
    [Serializable]
    public class ClickUpValidationException : ClickUpApiException
    {
        /// <summary>
        /// Gets the validation errors that occurred.
        /// </summary>
        public IReadOnlyList<ValidationError> ValidationErrors { get; }

        /// <summary>
        /// Gets the name of the property or parameter that failed validation, if applicable.
        /// </summary>
        public string? PropertyName { get; }

        /// <summary>
        /// Gets the invalid value that caused the validation error, if applicable.
        /// </summary>
        public object? InvalidValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpValidationException"/> class.
        /// </summary>
        public ClickUpValidationException() : this("Validation failed.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpValidationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ClickUpValidationException(string message) : base(message)
        {
            ValidationErrors = new List<ValidationError>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpValidationException"/> class with a specified error message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ClickUpValidationException(string message, Exception? innerException) : base(message, innerException!)
        {
            ValidationErrors = new List<ValidationError>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpValidationException"/> class with validation errors.
        /// </summary>
        /// <param name="validationErrors">The validation errors.</param>
        public ClickUpValidationException(IEnumerable<ValidationError> validationErrors) 
            : base(BuildMessage(validationErrors))
        {
            ValidationErrors = validationErrors?.ToList() ?? new List<ValidationError>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpValidationException"/> class with a single validation error.
        /// </summary>
        /// <param name="propertyName">The name of the property that failed validation.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <param name="invalidValue">The invalid value.</param>
        public ClickUpValidationException(string propertyName, string errorMessage, object? invalidValue = null)
            : base($"Validation failed for '{propertyName}': {errorMessage}")
        {
            PropertyName = propertyName;
            InvalidValue = invalidValue;
            ValidationErrors = new List<ValidationError>
            {
                new ValidationError(propertyName, errorMessage, invalidValue)
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpValidationException"/> class with detailed validation information.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="validationErrors">The validation errors.</param>
        /// <param name="propertyName">The property name that failed validation.</param>
        /// <param name="invalidValue">The invalid value.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClickUpValidationException(
            string message,
            IEnumerable<ValidationError>? validationErrors = null,
            string? propertyName = null,
            object? invalidValue = null,
            Exception? innerException = null) : base(message, innerException!)
        {
            ValidationErrors = validationErrors?.ToList() ?? new List<ValidationError>();
            PropertyName = propertyName;
            InvalidValue = invalidValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickUpValidationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
#pragma warning disable SYSLIB0051
        protected ClickUpValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ValidationErrors = (IReadOnlyList<ValidationError>?)info.GetValue(nameof(ValidationErrors), typeof(IReadOnlyList<ValidationError>)) ?? new List<ValidationError>();
            PropertyName = info.GetString(nameof(PropertyName));
            InvalidValue = info.GetValue(nameof(InvalidValue), typeof(object));
        }
#pragma warning restore SYSLIB0051

        /// <summary>
        /// Sets the serialization data for the exception.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
#pragma warning disable SYSLIB0051
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ValidationErrors), ValidationErrors);
            info.AddValue(nameof(PropertyName), PropertyName);
            info.AddValue(nameof(InvalidValue), InvalidValue);
        }
#pragma warning restore SYSLIB0051

        /// <summary>
        /// Gets a value indicating whether the exception has validation errors.
        /// </summary>
        public bool HasValidationErrors => ValidationErrors.Any();

        /// <summary>
        /// Gets validation errors for a specific property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The validation errors for the specified property.</returns>
        public IEnumerable<ValidationError> GetErrorsForProperty(string propertyName)
        {
            return ValidationErrors.Where(e => string.Equals(e.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates a string representation of the exception with validation details.
        /// </summary>
        /// <returns>A detailed string representation of the exception.</returns>
        public override string ToString()
        {
            var details = new List<string> { base.ToString() };

            if (HasValidationErrors)
            {
                details.Add("Validation Errors:");
                foreach (var error in ValidationErrors)
                {
                    details.Add($"  - {error}");
                }
            }

            return string.Join(Environment.NewLine, details);
        }

        /// <summary>
        /// Builds an error message from validation errors.
        /// </summary>
        /// <param name="validationErrors">The validation errors.</param>
        /// <returns>A formatted error message.</returns>
        private static string BuildMessage(IEnumerable<ValidationError>? validationErrors)
        {
            if (validationErrors == null || !validationErrors.Any())
                return "Validation failed.";

            var errors = validationErrors.ToList();
            if (errors.Count == 1)
                return $"Validation failed: {errors[0].ErrorMessage}";

            return $"Validation failed with {errors.Count} errors: {string.Join("; ", errors.Select(e => e.ErrorMessage))}";
        }
    }

    /// <summary>
    /// Represents a single validation error.
    /// </summary>
    [Serializable]
    public class ValidationError
    {
        /// <summary>
        /// Gets the name of the property that failed validation.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the validation error message.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets the invalid value that caused the validation error.
        /// </summary>
        public object? InvalidValue { get; }

        /// <summary>
        /// Gets the validation rule that was violated, if applicable.
        /// </summary>
        public string? ValidationRule { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property that failed validation.</param>
        /// <param name="errorMessage">The validation error message.</param>
        /// <param name="invalidValue">The invalid value.</param>
        /// <param name="validationRule">The validation rule that was violated.</param>
        public ValidationError(string propertyName, string errorMessage, object? invalidValue = null, string? validationRule = null)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            InvalidValue = invalidValue;
            ValidationRule = validationRule;
        }

        /// <summary>
        /// Returns a string representation of the validation error.
        /// </summary>
        /// <returns>A string representation of the validation error.</returns>
        public override string ToString()
        {
            var result = $"{PropertyName}: {ErrorMessage}";
            
            if (InvalidValue != null)
                result += $" (Invalid value: {InvalidValue})";
            
            if (!string.IsNullOrEmpty(ValidationRule))
                result += $" [Rule: {ValidationRule}]";
            
            return result;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current validation error.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            return obj is ValidationError other &&
                   PropertyName == other.PropertyName &&
                   ErrorMessage == other.ErrorMessage &&
                   Equals(InvalidValue, other.InvalidValue) &&
                   ValidationRule == other.ValidationRule;
        }

        /// <summary>
        /// Returns a hash code for the validation error.
        /// </summary>
        /// <returns>A hash code for the validation error.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(PropertyName, ErrorMessage, InvalidValue, ValidationRule);
        }
    }
}