using System.Collections.Generic;

namespace ClickUp.Api.Client.Validation
{
    /// <summary>
    /// Defines a contract for validating objects of type T.
    /// </summary>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    public interface IValidator<in T>
    {
        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>A validation result containing any errors found.</returns>
        ValidationResult Validate(T obj);
    }

    /// <summary>
    /// Defines a contract for validating objects with context.
    /// </summary>
    /// <typeparam name="T">The type of object to validate.</typeparam>
    /// <typeparam name="TContext">The type of validation context.</typeparam>
    public interface IValidator<in T, in TContext>
    {
        /// <summary>
        /// Validates the specified object with the given context.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <param name="context">The validation context.</param>
        /// <returns>A validation result containing any errors found.</returns>
        ValidationResult Validate(T obj, TContext context);
    }
}