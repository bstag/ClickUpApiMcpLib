using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ClickUp.Api.Client.Validation.Attributes;

namespace ClickUp.Api.Client.Validation
{
    /// <summary>
    /// Provides a fluent interface for building validation rules.
    /// </summary>
    /// <typeparam name="T">The type of object being validated.</typeparam>
    public class FluentValidationBuilder<T>
    {
        private readonly T _object;
        private readonly List<Func<ValidationResult>> _validationRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidationBuilder{T}"/> class.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        public FluentValidationBuilder(T obj)
        {
            _object = obj ?? throw new ArgumentNullException(nameof(obj));
            _validationRules = new List<Func<ValidationResult>>();
        }

        /// <summary>
        /// Adds a validation rule for a property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>A property validation builder.</returns>
        public PropertyValidationBuilder<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return new PropertyValidationBuilder<T, TProperty>(this, propertyExpression);
        }

        /// <summary>
        /// Adds a custom validation rule.
        /// </summary>
        /// <param name="validationRule">The validation rule function.</param>
        /// <returns>The current builder instance.</returns>
        public FluentValidationBuilder<T> Must(Func<T, ValidationResult> validationRule)
        {
            _validationRules.Add(() => validationRule(_object));
            return this;
        }

        /// <summary>
        /// Adds a custom validation rule with a condition.
        /// </summary>
        /// <param name="condition">The condition that must be true.</param>
        /// <param name="errorMessage">The error message if the condition is false.</param>
        /// <param name="propertyName">The property name for the error.</param>
        /// <returns>The current builder instance.</returns>
        public FluentValidationBuilder<T> Must(Func<T, bool> condition, string errorMessage, string propertyName = "Object")
        {
            _validationRules.Add(() => 
                condition(_object) ? ValidationResult.Success() : ValidationResult.Failure(propertyName, errorMessage));
            return this;
        }

        /// <summary>
        /// Executes all validation rules and returns the combined result.
        /// </summary>
        /// <param name="throwOnFailure">Whether to throw an exception on validation failure.</param>
        /// <returns>The validation result.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails and throwOnFailure is true.</exception>
        public ValidationResult Validate(bool throwOnFailure = true)
        {
            var combinedResult = new ValidationResult();

            // First, validate using attributes
            ValidationHelper.Validate(_object, false);
            // Note: ValidationHelper.Validate doesn't return a result, so we skip combining

            // Then, execute custom validation rules
            foreach (var rule in _validationRules)
            {
                var ruleResult = rule();
                combinedResult = combinedResult.Combine(ruleResult);
            }

            if (throwOnFailure) combinedResult.ThrowIfInvalid();
            return combinedResult;
        }

        /// <summary>
        /// Returns to the parent builder for chaining additional property validations.
        /// </summary>
        /// <returns>The current builder instance.</returns>
        public FluentValidationBuilder<T> And()
        {
            return this;
        }

        /// <summary>
        /// Adds a validation rule to the builder.
        /// </summary>
        /// <param name="validationRule">The validation rule function.</param>
        internal void AddRule(Func<ValidationResult> validationRule)
        {
            _validationRules.Add(validationRule);
        }
    }

    /// <summary>
    /// Provides a fluent interface for building validation rules for a specific property.
    /// </summary>
    /// <typeparam name="T">The type of object being validated.</typeparam>
    /// <typeparam name="TProperty">The type of the property being validated.</typeparam>
    public class PropertyValidationBuilder<T, TProperty>
    {
        private readonly FluentValidationBuilder<T> _parentBuilder;
        private readonly Expression<Func<T, TProperty>> _propertyExpression;
        private readonly string _propertyName;
        private readonly Func<T, TProperty> _propertyGetter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValidationBuilder{T, TProperty}"/> class.
        /// </summary>
        /// <param name="parentBuilder">The parent builder.</param>
        /// <param name="propertyExpression">The property expression.</param>
        public PropertyValidationBuilder(FluentValidationBuilder<T> parentBuilder, Expression<Func<T, TProperty>> propertyExpression)
        {
            _parentBuilder = parentBuilder ?? throw new ArgumentNullException(nameof(parentBuilder));
            _propertyExpression = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpression));
            _propertyName = GetPropertyName(propertyExpression);
            _propertyGetter = propertyExpression.Compile();
        }

        /// <summary>
        /// Adds a required validation rule.
        /// </summary>
        /// <param name="errorMessage">The custom error message.</param>
        /// <returns>The current property builder instance.</returns>
        public PropertyValidationBuilder<T, TProperty> NotNull(string? errorMessage = null)
        {
            _parentBuilder.AddRule(() =>
            {
                var obj = GetObjectFromParent();
                var value = _propertyGetter(obj);
                var attribute = new RequiredAttribute(errorMessage);
                return attribute.Validate(value, _propertyName);
            });
            return this;
        }

        /// <summary>
        /// Adds a string length validation rule.
        /// </summary>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="errorMessage">The custom error message.</param>
        /// <returns>The current property builder instance.</returns>
        public PropertyValidationBuilder<T, TProperty> Length(int maxLength, int minLength = 0, string? errorMessage = null)
        {
            _parentBuilder.AddRule(() =>
            {
                var obj = GetObjectFromParent();
                var value = _propertyGetter(obj);
                var attribute = new StringLengthAttribute(maxLength, errorMessage) { MinimumLength = minLength };
                return attribute.Validate(value, _propertyName);
            });
            return this;
        }

        /// <summary>
        /// Adds a range validation rule.
        /// </summary>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        /// <param name="errorMessage">The custom error message.</param>
        /// <returns>The current property builder instance.</returns>
        public PropertyValidationBuilder<T, TProperty> Range(int minimum, int maximum, string? errorMessage = null)
        {
            _parentBuilder.AddRule(() =>
            {
                var obj = GetObjectFromParent();
                var value = _propertyGetter(obj);
                var attribute = new RangeAttribute(minimum, maximum, errorMessage);
                return attribute.Validate(value, _propertyName);
            });
            return this;
        }

        /// <summary>
        /// Adds a ClickUp ID validation rule.
        /// </summary>
        /// <param name="errorMessage">The custom error message.</param>
        /// <returns>The current property builder instance.</returns>
        public PropertyValidationBuilder<T, TProperty> ClickUpId(string? errorMessage = null)
        {
            _parentBuilder.AddRule(() =>
            {
                var obj = GetObjectFromParent();
                var value = _propertyGetter(obj);
                var attribute = new ClickUpIdAttribute(errorMessage);
                return attribute.Validate(value, _propertyName);
            });
            return this;
        }

        /// <summary>
        /// Adds a custom validation rule for the property.
        /// </summary>
        /// <param name="condition">The condition that must be true.</param>
        /// <param name="errorMessage">The error message if the condition is false.</param>
        /// <returns>The current property builder instance.</returns>
        public PropertyValidationBuilder<T, TProperty> Must(Func<TProperty, bool> condition, string errorMessage)
        {
            _parentBuilder.AddRule(() =>
            {
                var obj = GetObjectFromParent();
                var value = _propertyGetter(obj);
                return condition(value) ? ValidationResult.Success() : ValidationResult.Failure(_propertyName, errorMessage);
            });
            return this;
        }

        /// <summary>
        /// Returns to the parent builder to continue building validation rules.
        /// </summary>
        /// <returns>The parent builder instance.</returns>
        public FluentValidationBuilder<T> And()
        {
            return _parentBuilder;
        }

        /// <summary>
        /// Executes all validation rules and returns the combined result.
        /// </summary>
        /// <param name="throwOnFailure">Whether to throw an exception on validation failure.</param>
        /// <returns>The validation result.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails and throwOnFailure is true.</exception>
        public ValidationResult Validate(bool throwOnFailure = true)
        {
            return _parentBuilder.Validate(throwOnFailure);
        }

        /// <summary>
        /// Gets the property name from the expression.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The property name.</returns>
        private static string GetPropertyName(Expression<Func<T, TProperty>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            throw new ArgumentException("Expression must be a property access expression.", nameof(propertyExpression));
        }

        /// <summary>
        /// Gets the object being validated from the parent builder using reflection.
        /// </summary>
        /// <returns>The object being validated.</returns>
        private T GetObjectFromParent()
        {
            var objectField = typeof(FluentValidationBuilder<T>).GetField("_object", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)objectField!.GetValue(_parentBuilder)!;
        }
    }
}