using ClickUp.Api.Client.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Fluent.Validation;

/// <summary>
/// A pipeline for executing validation rules in a fluent manner.
/// </summary>
public class FluentValidationPipeline
{
    private readonly List<IValidationRule> _rules = new();
    private readonly List<string> _errors = new();
    private bool _stopOnFirstError = false;

    /// <summary>
    /// Adds a validation rule to the pipeline.
    /// </summary>
    /// <param name="rule">The validation rule to add</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline AddRule(IValidationRule rule)
    {
        _rules.Add(rule);
        return this;
    }

    /// <summary>
    /// Adds a simple validation rule with a condition and error message.
    /// </summary>
    /// <param name="condition">The condition to validate (should return true for valid)</param>
    /// <param name="errorMessage">The error message if validation fails</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline AddRule(Func<bool> condition, string errorMessage)
    {
        return AddRule(new SimpleValidationRule(condition, errorMessage));
    }

    /// <summary>
    /// Adds a validation rule for a required field.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="fieldName">The name of the field</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline RequiredField(string? value, string fieldName)
    {
        return AddRule(() => !string.IsNullOrWhiteSpace(value), $"{fieldName} is required.");
    }

    /// <summary>
    /// Adds a validation rule for a required object field.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="fieldName">The name of the field</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline RequiredField<T>(T? value, string fieldName) where T : class
    {
        return AddRule(() => value != null, $"{fieldName} is required.");
    }

    /// <summary>
    /// Adds a validation rule for a required nullable value type field.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="fieldName">The name of the field</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline RequiredField<T>(T? value, string fieldName) where T : struct
    {
        return AddRule(() => value.HasValue, $"{fieldName} is required.");
    }

    /// <summary>
    /// Adds a validation rule for numeric range validation.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="min">The minimum allowed value</param>
    /// <param name="max">The maximum allowed value</param>
    /// <param name="fieldName">The name of the field</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline RangeField(int? value, int min, int max, string fieldName)
    {
        return AddRule(() => !value.HasValue || (value.Value >= min && value.Value <= max), 
                      $"{fieldName} must be between {min} and {max}.");
    }

    /// <summary>
    /// Adds a validation rule for string length validation.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="maxLength">The maximum allowed length</param>
    /// <param name="fieldName">The name of the field</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline MaxLengthField(string? value, int maxLength, string fieldName)
    {
        return AddRule(() => string.IsNullOrEmpty(value) || value.Length <= maxLength, 
                      $"{fieldName} cannot exceed {maxLength} characters.");
    }

    /// <summary>
    /// Adds a validation rule for minimum string length validation.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="minLength">The minimum required length</param>
    /// <param name="fieldName">The name of the field</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline MinLengthField(string? value, int minLength, string fieldName)
    {
        return AddRule(() => !string.IsNullOrEmpty(value) && value.Length >= minLength, 
                      $"{fieldName} must be at least {minLength} characters long.");
    }

    /// <summary>
    /// Adds a validation rule for email format validation.
    /// </summary>
    /// <param name="email">The email to validate</param>
    /// <param name="fieldName">The name of the field</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline EmailField(string? email, string fieldName)
    {
        return AddRule(() => string.IsNullOrEmpty(email) || IsValidEmail(email), 
                      $"{fieldName} must be a valid email address.");
    }

    /// <summary>
    /// Adds a validation rule for URL format validation.
    /// </summary>
    /// <param name="url">The URL to validate</param>
    /// <param name="fieldName">The name of the field</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline UrlField(string? url, string fieldName)
    {
        return AddRule(() => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _), 
                      $"{fieldName} must be a valid URL.");
    }

    /// <summary>
    /// Adds a validation rule for collection validation.
    /// </summary>
    /// <param name="collection">The collection to validate</param>
    /// <param name="fieldName">The name of the field</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline NotEmptyCollection<T>(IEnumerable<T>? collection, string fieldName)
    {
        return AddRule(() => collection == null || collection.Any(), 
                      $"{fieldName} cannot be empty when provided.");
    }

    /// <summary>
    /// Adds a custom validation rule with async support.
    /// </summary>
    /// <param name="rule">The async validation rule</param>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline AddAsyncRule(IAsyncValidationRule rule)
    {
        return AddRule(new AsyncValidationRuleWrapper(rule));
    }

    /// <summary>
    /// Configures the pipeline to stop on the first validation error.
    /// </summary>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline StopOnFirstError()
    {
        _stopOnFirstError = true;
        return this;
    }

    /// <summary>
    /// Configures the pipeline to collect all validation errors.
    /// </summary>
    /// <returns>The pipeline for chaining</returns>
    public FluentValidationPipeline CollectAllErrors()
    {
        _stopOnFirstError = false;
        return this;
    }

    /// <summary>
    /// Executes all validation rules in the pipeline.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The validation result</returns>
    public async Task<ValidationResult> ValidateAsync(CancellationToken cancellationToken = default)
    {
        _errors.Clear();

        foreach (var rule in _rules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await rule.ValidateAsync(cancellationToken);
            if (!result.IsValid)
            {
                _errors.AddRange(result.Errors);
                if (_stopOnFirstError)
                    break;
            }
        }

        return new ValidationResult(_errors.Count == 0, _errors.ToList());
    }

    /// <summary>
    /// Executes all validation rules synchronously.
    /// </summary>
    /// <returns>The validation result</returns>
    public ValidationResult Validate()
    {
        return ValidateAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Validates and throws an exception if validation fails.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <exception cref="ClickUpRequestValidationException">Thrown when validation fails</exception>
    public async Task ValidateAndThrowAsync(CancellationToken cancellationToken = default)
    {
        var result = await ValidateAsync(cancellationToken);
        if (!result.IsValid)
        {
            throw new ClickUpRequestValidationException("Validation failed.", result.Errors);
        }
    }

    /// <summary>
    /// Validates and throws an exception if validation fails (synchronous).
    /// </summary>
    /// <exception cref="ClickUpRequestValidationException">Thrown when validation fails</exception>
    public void ValidateAndThrow()
    {
        ValidateAndThrowAsync().GetAwaiter().GetResult();
    }

    private static bool IsValidEmail(string email)
    {
        // Use TryCreate to avoid exception-based flow control for validation.
        // This is more performant and cleaner than using the MailAddress constructor in a try-catch block.
        return !string.IsNullOrWhiteSpace(email) && System.Net.Mail.MailAddress.TryCreate(email, out _);
    }
}

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    public ValidationResult(bool isValid, List<string> errors)
    {
        IsValid = isValid;
        Errors = errors ?? new List<string>();
    }

    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the list of validation errors.
    /// </summary>
    public List<string> Errors { get; }
}

/// <summary>
/// Interface for validation rules.
/// </summary>
public interface IValidationRule
{
    /// <summary>
    /// Validates the rule asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The validation result</returns>
    Task<ValidationResult> ValidateAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for async validation rules.
/// </summary>
public interface IAsyncValidationRule
{
    /// <summary>
    /// Validates the rule asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The validation result</returns>
    Task<ValidationResult> ValidateAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Simple validation rule implementation.
/// </summary>
internal class SimpleValidationRule : IValidationRule
{
    private readonly Func<bool> _condition;
    private readonly string _errorMessage;

    public SimpleValidationRule(Func<bool> condition, string errorMessage)
    {
        _condition = condition;
        _errorMessage = errorMessage;
    }

    public Task<ValidationResult> ValidateAsync(CancellationToken cancellationToken = default)
    {
        var isValid = _condition();
        var errors = isValid ? new List<string>() : new List<string> { _errorMessage };
        return Task.FromResult(new ValidationResult(isValid, errors));
    }
}

/// <summary>
/// Wrapper for async validation rules.
/// </summary>
internal class AsyncValidationRuleWrapper : IValidationRule
{
    private readonly IAsyncValidationRule _asyncRule;

    public AsyncValidationRuleWrapper(IAsyncValidationRule asyncRule)
    {
        _asyncRule = asyncRule;
    }

    public Task<ValidationResult> ValidateAsync(CancellationToken cancellationToken = default)
    {
        return _asyncRule.ValidateAsync(cancellationToken);
    }
}