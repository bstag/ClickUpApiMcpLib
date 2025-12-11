# ClickUp API Client - Enhanced Validation Framework

This document describes the new validation framework implemented as part of the ClickUp SDK refactoring initiative. The framework provides a comprehensive, extensible, and maintainable approach to input validation while maintaining backward compatibility.

## Overview

The enhanced validation framework addresses several key issues in the original codebase:
- **DRY Principle Violations**: Eliminates repetitive validation code across services
- **Consistency**: Provides uniform validation patterns and error handling
- **Extensibility**: Easy to add new validation rules and attributes
- **Maintainability**: Centralized validation logic reduces maintenance overhead
- **Type Safety**: Strongly-typed validation with compile-time checking

## Core Components

### 1. Validation Interfaces

```csharp
// Basic validator interface
public interface IValidator<T>
{
    ValidationResult Validate(T instance);
}

// Context-aware validator interface
public interface IValidator<T, TContext>
{
    ValidationResult Validate(T instance, TContext context);
}
```

### 2. ValidationResult

Represents the outcome of validation operations:

```csharp
var result = ValidationHelper.Validate(myObject);
if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
    }
}
```

### 3. Validation Attributes

The framework includes several built-in validation attributes:

- **`[Required]`**: Ensures a property has a value
- **`[StringLength]`**: Validates string length constraints
- **`[Range]`**: Validates numeric ranges
- **`[ClickUpId]`**: Validates ClickUp entity ID format

### 4. Fluent Validation API

Provides a fluent interface for building complex validation rules:

```csharp
var result = ValidationHelper.For(request)
    .Property(x => x.Name)
        .NotNull("Name is required")
        .Length(1, 255, "Name must be 1-255 characters")
    .Property(x => x.ListId)
        .ClickUpId("Invalid list ID format")
    .Validate();
```

## Usage Examples

### Attribute-Based Validation

```csharp
public class CreateTaskRequest
{
    [Required(ErrorMessage = "Task name is required")]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [ClickUpId(ErrorMessage = "Invalid list ID format")]
    public string ListId { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5")]
    public int? Priority { get; set; }
}

// Validate the object
var result = ValidationHelper.Validate(request);
if (!result.IsValid)
{
    throw new ValidationException(result);
}
```

### Fluent Validation

```csharp
var result = ValidationHelper.For(request)
    .Property(x => x.Name)
        .NotNull("Task name is required")
        .Length(1, 255, "Task name must be between 1 and 255 characters")
    .Property(x => x.ListId)
        .NotNull("List ID is required")
        .ClickUpId("Invalid list ID format")
    .Property(x => x.Priority)
        .Must(p => p == null || (p >= 1 && p <= 5), "Priority must be between 1 and 5")
    .Validate();
```

### Custom Validation Rules

```csharp
var result = ValidationHelper.For(request)
    .Property(x => x.Name)
        .Must(name => !name.Contains("forbidden"), "Task name cannot contain 'forbidden'")
    .Property(x => x.Description)
        .Must(desc => desc == null || !desc.Contains("spam"), "Description cannot contain 'spam'")
    .Validate();
```

## Migration Guide

### Before (Old Pattern)

```csharp
public async Task<TaskResponse> CreateTaskAsync(string listId, CreateTaskRequest request)
{
    // Manual validation scattered throughout the method
    if (string.IsNullOrWhiteSpace(listId))
        throw new ArgumentException("List ID cannot be null or empty", nameof(listId));
    
    ValidationHelper.ValidateId(listId, nameof(listId));
    
    if (request == null)
        throw new ArgumentNullException(nameof(request));
    
    if (string.IsNullOrWhiteSpace(request.Name))
        throw new ArgumentException("Task name is required", nameof(request.Name));
    
    if (request.Name.Length > 255)
        throw new ArgumentException("Task name cannot exceed 255 characters", nameof(request.Name));
    
    // ... rest of the method
}
```

### After (New Pattern)

```csharp
public async Task<TaskResponse> CreateTaskAsync(string listId, CreateTaskRequest request)
{
    // Consolidated validation using the new framework
    ValidationHelper.ValidateId(listId, nameof(listId)); // Backward compatible
    
    var validationResult = ValidationHelper.Validate(request);
    if (!validationResult.IsValid)
    {
        throw new ValidationException(validationResult);
    }
    
    // ... rest of the method
}
```

### Or using fluent validation:

```csharp
public async Task<TaskResponse> CreateTaskAsync(string listId, CreateTaskRequest request)
{
    var validationResult = ValidationHelper.For(request)
        .Property(x => x.Name)
            .NotNull("Task name is required")
            .Length(1, 255, "Task name must be between 1 and 255 characters")
        .Validate();
    
    if (!validationResult.IsValid)
    {
        throw new ValidationException(validationResult);
    }
    
    // ... rest of the method
}
```

## Backward Compatibility

The framework maintains full backward compatibility with existing validation methods:

- `ValidationHelper.ValidateId()` - Still works as before
- `ValidationHelper.ValidateRequiredString()` - Still works as before
- `ValidationHelper.ValidateRange()` - Still works as before

These methods now use the new framework internally but maintain the same public API.

## Benefits

1. **Reduced Code Duplication**: Common validation patterns are centralized
2. **Improved Consistency**: Uniform validation behavior across all services
3. **Better Error Messages**: Structured error information with property names
4. **Enhanced Maintainability**: Changes to validation logic only need to be made in one place
5. **Type Safety**: Compile-time checking of validation rules
6. **Extensibility**: Easy to add new validation attributes and rules
7. **Testability**: Validation logic can be easily unit tested

## Custom Validation Attributes

You can create custom validation attributes by inheriting from `ValidationAttribute`:

```csharp
public class EmailAttribute : ValidationAttribute
{
    public override bool IsValid(object? value, string propertyName)
    {
        if (value is not string email)
            return value == null; // Allow null unless Required is also specified
        
        return email.Contains("@") && email.Contains(".");
    }
    
    protected override string GetDefaultErrorMessage(string propertyName)
    {
        return $"{propertyName} must be a valid email address";
    }
}
```

## Performance Considerations

- Validation attributes are cached using reflection for better performance
- Fluent validation builders are lightweight and can be reused
- The framework is designed to minimize allocations during validation

## Future Enhancements

The framework is designed to support future enhancements such as:
- Async validation support
- Conditional validation rules
- Cross-property validation
- Integration with dependency injection
- Localization support for error messages

## Testing

The validation framework includes comprehensive examples in `ValidationExamples.cs` that demonstrate all major features and can serve as integration tests.