using System;
using System.Collections.Generic;
using System.Linq;

namespace ClickUp.Api.Client.Validation
{
    /// <summary>
    /// Represents the result of a validation operation.
    /// </summary>
    public class ValidationResult
    {
        private readonly List<ValidationError> _errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        public ValidationResult()
        {
            _errors = new List<ValidationError>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class with the specified errors.
        /// </summary>
        /// <param name="errors">The validation errors.</param>
        public ValidationResult(IEnumerable<ValidationError> errors)
        {
            _errors = errors?.ToList() ?? new List<ValidationError>();
        }

        /// <summary>
        /// Gets a value indicating whether the validation was successful (no errors).
        /// </summary>
        public bool IsValid => !_errors.Any();

        /// <summary>
        /// Gets the collection of validation errors.
        /// </summary>
        public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

        /// <summary>
        /// Adds a validation error to the result.
        /// </summary>
        /// <param name="error">The validation error to add.</param>
        public void AddError(ValidationError error)
        {
            if (error != null)
            {
                _errors.Add(error);
            }
        }

        /// <summary>
        /// Adds a validation error to the result.
        /// </summary>
        /// <param name="propertyName">The name of the property that failed validation.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorCode">The optional error code.</param>
        public void AddError(string propertyName, string errorMessage, string? errorCode = null)
        {
            AddError(new ValidationError(propertyName, errorMessage, errorCode));
        }

        /// <summary>
        /// Combines this validation result with another validation result.
        /// </summary>
        /// <param name="other">The other validation result to combine with.</param>
        /// <returns>A new validation result containing errors from both results.</returns>
        public ValidationResult Combine(ValidationResult other)
        {
            if (other == null) return this;
            
            var combinedErrors = _errors.Concat(other._errors);
            return new ValidationResult(combinedErrors);
        }

        /// <summary>
        /// Creates a successful validation result.
        /// </summary>
        /// <returns>A validation result with no errors.</returns>
        public static ValidationResult Success() => new ValidationResult();

        /// <summary>
        /// Creates a failed validation result with a single error.
        /// </summary>
        /// <param name="propertyName">The name of the property that failed validation.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorCode">The optional error code.</param>
        /// <returns>A validation result with the specified error.</returns>
        public static ValidationResult Failure(string propertyName, string errorMessage, string? errorCode = null)
        {
            var result = new ValidationResult();
            result.AddError(propertyName, errorMessage, errorCode);
            return result;
        }

        /// <summary>
        /// Throws a <see cref="ValidationException"/> if the validation result contains errors.
        /// </summary>
        /// <exception cref="ValidationException">Thrown if the validation result is not valid.</exception>
        public void ThrowIfInvalid()
        {
            if (!IsValid)
            {
                throw new ValidationException(this);
            }
        }
    }
}