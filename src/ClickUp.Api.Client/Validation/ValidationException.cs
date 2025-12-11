using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClickUp.Api.Client.Validation
{
    /// <summary>
    /// Exception thrown when validation fails.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="validationResult">The validation result containing the errors.</param>
        public ValidationException(ValidationResult validationResult)
            : base(BuildErrorMessage(validationResult))
        {
            ValidationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="validationResult">The validation result containing the errors.</param>
        public ValidationException(string message, ValidationResult validationResult)
            : base(message)
        {
            ValidationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="validationResult">The validation result containing the errors.</param>
        public ValidationException(string message, Exception innerException, ValidationResult validationResult)
            : base(message, innerException)
        {
            ValidationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
        }

        /// <summary>
        /// Gets the validation result that caused this exception.
        /// </summary>
        public ValidationResult ValidationResult { get; }

        /// <summary>
        /// Gets the validation errors that caused this exception.
        /// </summary>
        public IReadOnlyList<ValidationError> Errors => ValidationResult.Errors;

        /// <summary>
        /// Builds an error message from the validation result.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <returns>A formatted error message.</returns>
        private static string BuildErrorMessage(ValidationResult validationResult)
        {
            if (validationResult == null || !validationResult.Errors.Any())
            {
                return "Validation failed.";
            }

            var sb = new StringBuilder("Validation failed with the following errors:");
            foreach (var error in validationResult.Errors)
            {
                sb.AppendLine();
                sb.Append("- ");
                sb.Append(error.ToString());
            }

            return sb.ToString();
        }
    }
}