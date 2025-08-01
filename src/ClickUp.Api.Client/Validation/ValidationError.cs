using System;

namespace ClickUp.Api.Client.Validation
{
    /// <summary>
    /// Represents a validation error for a specific property.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property that failed validation.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="errorCode">The optional error code.</param>
        /// <exception cref="ArgumentNullException">Thrown if propertyName or errorMessage is null.</exception>
        public ValidationError(string propertyName, string errorMessage, string? errorCode = null)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Gets the name of the property that failed validation.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets the optional error code.
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// Returns a string representation of the validation error.
        /// </summary>
        /// <returns>A string containing the property name and error message.</returns>
        public override string ToString()
        {
            var result = $"{PropertyName}: {ErrorMessage}";
            if (!string.IsNullOrEmpty(ErrorCode))
            {
                result += $" (Code: {ErrorCode})";
            }
            return result;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current validation error.
        /// </summary>
        /// <param name="obj">The object to compare with the current validation error.</param>
        /// <returns>true if the specified object is equal to the current validation error; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is ValidationError other)
            {
                return PropertyName == other.PropertyName &&
                       ErrorMessage == other.ErrorMessage &&
                       ErrorCode == other.ErrorCode;
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this validation error.
        /// </summary>
        /// <returns>A hash code for the current validation error.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(PropertyName, ErrorMessage, ErrorCode);
        }
    }
}