using ClickUp.Api.Client.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent.Builders;

/// <summary>
/// Base class for all fluent builders providing common functionality and patterns.
/// </summary>
/// <typeparam name="TBuilder">The concrete builder type for fluent chaining</typeparam>
/// <typeparam name="TResult">The type of result this builder produces</typeparam>
public abstract class FluentBuilderBase<TBuilder, TResult> : IFluentBuilder<TBuilder, TResult>
    where TBuilder : FluentBuilderBase<TBuilder, TResult>
{
    private readonly List<string> _validationErrors = new();
    private readonly Dictionary<string, object?> _state = new();
    private bool _isValidated = false;

    /// <summary>
    /// Gets the concrete builder instance for fluent chaining.
    /// </summary>
    protected TBuilder Self => (TBuilder)this;

    /// <summary>
    /// Gets the current validation errors.
    /// </summary>
    protected IReadOnlyList<string> ValidationErrors => _validationErrors.AsReadOnly();

    /// <summary>
    /// Gets or sets a value in the builder state.
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="key">The state key</param>
    /// <param name="value">The value to set (optional for getter)</param>
    /// <returns>The current value or default if not set</returns>
    protected T? GetOrSetState<T>(string key, T? value = default)
    {
        if (value != null)
        {
            _state[key] = value;
            _isValidated = false; // Reset validation when state changes
            return value;
        }
        
        return _state.TryGetValue(key, out var existingValue) && existingValue is T typedValue 
            ? typedValue 
            : default;
    }

    /// <summary>
    /// Checks if a state value has been set.
    /// </summary>
    /// <param name="key">The state key</param>
    /// <returns>True if the value has been set</returns>
    protected bool HasState(string key) => _state.ContainsKey(key);

    /// <summary>
    /// Conditionally applies configuration based on a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate</param>
    /// <param name="configure">The configuration action to apply if condition is true</param>
    /// <returns>The builder instance for chaining</returns>
    public TBuilder When(bool condition, Func<TBuilder, TBuilder> configure)
    {
        return condition ? configure(Self) : Self;
    }

    /// <summary>
    /// Conditionally applies configuration based on a condition with an else clause.
    /// </summary>
    /// <param name="condition">The condition to evaluate</param>
    /// <param name="configureTrue">The configuration action to apply if condition is true</param>
    /// <param name="configureFalse">The configuration action to apply if condition is false</param>
    /// <returns>The builder instance for chaining</returns>
    public TBuilder When(bool condition, Func<TBuilder, TBuilder> configureTrue, Func<TBuilder, TBuilder> configureFalse)
    {
        return condition ? configureTrue(Self) : configureFalse(Self);
    }

    /// <summary>
    /// Applies configuration when a value is not null.
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to check</param>
    /// <param name="configure">The configuration action to apply if value is not null</param>
    /// <returns>The builder instance for chaining</returns>
    public TBuilder WhenNotNull<T>(T? value, Func<TBuilder, T, TBuilder> configure) where T : class
    {
        return value != null ? configure(Self, value) : Self;
    }

    /// <summary>
    /// Applies configuration when a nullable value has a value.
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The nullable value to check</param>
    /// <param name="configure">The configuration action to apply if value has a value</param>
    /// <returns>The builder instance for chaining</returns>
    public TBuilder WhenHasValue<T>(T? value, Func<TBuilder, T, TBuilder> configure) where T : struct
    {
        return value.HasValue ? configure(Self, value.Value) : Self;
    }

    /// <summary>
    /// Validates the current builder state.
    /// </summary>
    /// <exception cref="ClickUpRequestValidationException">Thrown when validation fails</exception>
    public virtual void Validate()
    {
        if (_isValidated) return;

        _validationErrors.Clear();
        ValidateCore();

        if (_validationErrors.Any())
        {
            throw new ClickUpRequestValidationException(
                "Request validation failed.", 
                _validationErrors.ToList());
        }

        _isValidated = true;
    }

    /// <summary>
    /// Adds a validation error to the collection.
    /// </summary>
    /// <param name="error">The validation error message</param>
    protected void AddValidationError(string error)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            _validationErrors.Add(error);
        }
    }

    /// <summary>
    /// Validates a required string field.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    protected void ValidateRequired(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AddValidationError($"{fieldName} is required.");
        }
    }

    /// <summary>
    /// Validates a required object field.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    protected void ValidateRequired<T>(T? value, string fieldName) where T : class
    {
        if (value == null)
        {
            AddValidationError($"{fieldName} is required.");
        }
    }

    /// <summary>
    /// Validates a required nullable value type field.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    protected void ValidateRequired<T>(T? value, string fieldName) where T : struct
    {
        if (!value.HasValue)
        {
            AddValidationError($"{fieldName} is required.");
        }
    }

    /// <summary>
    /// Validates that a numeric value is within a specified range.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="min">The minimum allowed value</param>
    /// <param name="max">The maximum allowed value</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    protected void ValidateRange(int? value, int min, int max, string fieldName)
    {
        if (value.HasValue && (value.Value < min || value.Value > max))
        {
            AddValidationError($"{fieldName} must be between {min} and {max}.");
        }
    }

    /// <summary>
    /// Validates that a collection is not empty when provided.
    /// </summary>
    /// <param name="collection">The collection to validate</param>
    /// <param name="fieldName">The name of the field for error messages</param>
    protected void ValidateNotEmpty<T>(IEnumerable<T>? collection, string fieldName)
    {
        if (collection != null && !collection.Any())
        {
            AddValidationError($"{fieldName} cannot be empty when provided.");
        }
    }

    /// <summary>
    /// Core validation logic to be implemented by derived classes.
    /// </summary>
    protected abstract void ValidateCore();

    /// <summary>
    /// Builds the result object. This method should validate before building.
    /// </summary>
    /// <returns>The built result</returns>
    public virtual TResult Build()
    {
        Validate();
        return BuildCore();
    }

    /// <summary>
    /// Core build logic to be implemented by derived classes.
    /// </summary>
    /// <returns>The built result</returns>
    protected abstract TResult BuildCore();

    /// <summary>
    /// Executes the builder asynchronously. Default implementation calls Build().
    /// Override for async operations.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The result</returns>
    public virtual Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = Build();
        return Task.FromResult(result);
    }

    /// <summary>
    /// Resets the builder to its initial state.
    /// </summary>
    public virtual void Reset()
    {
        _state.Clear();
        _validationErrors.Clear();
        _isValidated = false;
    }

    /// <summary>
    /// Creates a copy of the current builder state.
    /// </summary>
    /// <returns>A new builder instance with the same state</returns>
    public virtual TBuilder Clone()
    {
        var clone = CreateInstance();
        foreach (var kvp in _state)
        {
            clone._state[kvp.Key] = kvp.Value;
        }
        return clone;
    }

    /// <summary>
    /// Creates a new instance of the builder. Must be implemented by derived classes.
    /// </summary>
    /// <returns>A new builder instance</returns>
    protected abstract TBuilder CreateInstance();
}

/// <summary>
/// Interface for fluent builders.
/// </summary>
/// <typeparam name="TBuilder">The concrete builder type</typeparam>
/// <typeparam name="TResult">The result type</typeparam>
public interface IFluentBuilder<TBuilder, TResult>
    where TBuilder : IFluentBuilder<TBuilder, TResult>
{
    /// <summary>
    /// Conditionally applies configuration.
    /// </summary>
    TBuilder When(bool condition, Func<TBuilder, TBuilder> configure);
    
    /// <summary>
    /// Validates the builder state.
    /// </summary>
    void Validate();
    
    /// <summary>
    /// Builds the result.
    /// </summary>
    TResult Build();
    
    /// <summary>
    /// Executes the builder asynchronously.
    /// </summary>
    Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default);
}