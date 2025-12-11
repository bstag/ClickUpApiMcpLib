using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClickUp.Api.Client.Validation.Attributes;

namespace ClickUp.Api.Client.Validation
{
    /// <summary>
    /// Provides enhanced validation utilities for ClickUp API client inputs.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates an object using reflection and validation attributes.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="obj">The object to validate.</param>
        /// <param name="throwOnFailure">Whether to throw an exception on validation failure.</param>
        /// <returns>A validation result.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails and throwOnFailure is true.</exception>
        public static ValidationResult Validate<T>(T obj, bool throwOnFailure = true)
        {
            if (obj == null)
            {
                var result = ValidationResult.Failure(nameof(obj), "Object cannot be null.");
                if (throwOnFailure) result.ThrowIfInvalid();
                return result;
            }

            var validationResult = new ValidationResult();
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                var propertyResult = ValidateProperty(value, property.Name, property);
                validationResult = validationResult.Combine(propertyResult);
            }

            if (throwOnFailure) validationResult.ThrowIfInvalid();
            return validationResult;
        }

        /// <summary>
        /// Validates a property value using its validation attributes.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>A validation result.</returns>
        public static ValidationResult ValidateProperty(object? value, string propertyName, PropertyInfo propertyInfo)
        {
            var validationResult = new ValidationResult();
            var validationAttributes = propertyInfo.GetCustomAttributes<ValidationAttribute>(true);

            foreach (var attribute in validationAttributes)
            {
                var result = attribute.Validate(value, propertyName);
                validationResult = validationResult.Combine(result);
            }

            return validationResult;
        }

        /// <summary>
        /// Validates a value using the specified validation attributes.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="attributes">The validation attributes to apply.</param>
        /// <returns>A validation result.</returns>
        public static ValidationResult ValidateValue(object? value, string propertyName, params ValidationAttribute[] attributes)
        {
            var validationResult = new ValidationResult();

            foreach (var attribute in attributes)
            {
                var result = attribute.Validate(value, propertyName);
                validationResult = validationResult.Combine(result);
            }

            return validationResult;
        }

        /// <summary>
        /// Validates multiple objects and combines their results.
        /// </summary>
        /// <param name="objects">The objects to validate.</param>
        /// <param name="throwOnFailure">Whether to throw an exception on validation failure.</param>
        /// <returns>A combined validation result.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails and throwOnFailure is true.</exception>
        public static ValidationResult ValidateMultiple(IEnumerable<object> objects, bool throwOnFailure = true)
        {
            var combinedResult = new ValidationResult();

            foreach (var obj in objects)
            {
                if (obj != null)
                {
                    var objType = obj.GetType();
                    var validateMethod = typeof(ValidationHelper)
                        .GetMethod(nameof(Validate), BindingFlags.Public | BindingFlags.Static)
                        ?.MakeGenericMethod(objType);

                    if (validateMethod != null)
                    {
                        var result = (ValidationResult)validateMethod.Invoke(null, new object[] { obj, false })!;
                        combinedResult = combinedResult.Combine(result);
                    }
                }
            }

            if (throwOnFailure) combinedResult.ThrowIfInvalid();
            return combinedResult;
        }

        /// <summary>
        /// Creates a fluent validation builder for the specified object.
        /// </summary>
        /// <typeparam name="T">The type of object to validate.</typeparam>
        /// <param name="obj">The object to validate.</param>
        /// <returns>A fluent validation builder.</returns>
        public static FluentValidationBuilder<T> For<T>(T obj)
        {
            return new FluentValidationBuilder<T>(obj);
        }

        /// <summary>
        /// Validates that a collection is not null or empty.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to validate.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="throwOnFailure">Whether to throw an exception on validation failure.</param>
        /// <returns>A validation result.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails and throwOnFailure is true.</exception>
        public static ValidationResult ValidateNotNullOrEmpty<T>(IEnumerable<T>? collection, string propertyName, bool throwOnFailure = true)
        {
            ValidationResult result;

            if (collection == null)
            {
                result = ValidationResult.Failure(propertyName, $"{propertyName} cannot be null.");
            }
            else if (!collection.Any())
            {
                result = ValidationResult.Failure(propertyName, $"{propertyName} cannot be empty.");
            }
            else
            {
                result = ValidationResult.Success();
            }

            if (throwOnFailure) result.ThrowIfInvalid();
            return result;
        }

        /// <summary>
        /// Validates that all items in a collection are valid.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to validate.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="throwOnFailure">Whether to throw an exception on validation failure.</param>
        /// <returns>A validation result.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails and throwOnFailure is true.</exception>
        public static ValidationResult ValidateCollection<T>(IEnumerable<T>? collection, string propertyName, bool throwOnFailure = true)
        {
            if (collection == null)
            {
                var result = ValidationResult.Success(); // Null collections are handled by other validators
                if (throwOnFailure) result.ThrowIfInvalid();
                return result;
            }

            var combinedResult = new ValidationResult();
            var index = 0;

            foreach (var item in collection)
            {
                var itemResult = Validate(item, false);
                
                // Prefix property names with collection index
                var indexedResult = new ValidationResult();
                foreach (var error in itemResult.Errors)
                {
                    indexedResult.AddError($"{propertyName}[{index}].{error.PropertyName}", error.ErrorMessage, error.ErrorCode);
                }
                
                combinedResult = combinedResult.Combine(indexedResult);
                index++;
            }

            if (throwOnFailure) combinedResult.ThrowIfInvalid();
            return combinedResult;
        }
    }
}