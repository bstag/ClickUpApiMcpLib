# ClickUp API Client Fluent API Guidelines

## Overview

This document establishes naming conventions, patterns, and best practices for the ClickUp API Client's fluent API design to ensure consistency, discoverability, and maintainability across all fluent interfaces.

## Naming Conventions

### 1. Class Naming

#### Fluent API Classes
- **Pattern**: `{Entity}FluentApi`
- **Examples**: `TasksFluentApi`, `ListsFluentApi`, `SpacesFluentApi`
- **Purpose**: Main entry points for fluent operations on entities

#### Fluent Request Classes
- **Pattern**: `{Entity}Fluent{Operation}Request`
- **Examples**: `TaskFluentCreateRequest`, `ListFluentUpdateRequest`, `SpaceFluentCreateRequest`
- **Purpose**: Fluent builders for specific operations

### 2. Method Naming

#### Entry Point Methods
- **Create Operations**: `Create{Entity}()` or `Create{Entity}In{Container}()`
- **Update Operations**: `Update{Entity}(string id)`
- **Get Operations**: `Get{Entity}(string id)` or `Get{Entities}()`
- **Delete Operations**: `Delete{Entity}(string id)`

#### Fluent Builder Methods
- **Property Setters**: `With{PropertyName}(value)`
- **Collection Setters**: `With{CollectionName}(collection)` or `Add{ItemName}(item)`
- **Boolean Flags**: `With{FeatureName}Enabled(bool)` or `Enable{FeatureName}()` / `Disable{FeatureName}()`
- **Conditional Logic**: `When{Condition}()` or `If{Condition}()`

#### Terminal Methods
- **Async Operations**: `{Operation}Async(CancellationToken)`
- **Validation**: `Validate()`
- **Build**: `Build()` (for non-executing builders)

### 3. Parameter Naming

- Use camelCase for parameters
- Use descriptive names that match the API documentation
- Include units in parameter names when applicable (e.g., `dueDate`, `timeEstimateMs`)

## Fluent API Patterns

### 1. Builder Pattern Structure

```csharp
public class EntityFluentCreateRequest
{
    // Private fields for state
    private string? _name;
    private readonly string _containerId;
    private readonly IEntityService _service;
    private readonly List<string> _validationErrors = new();

    // Constructor
    public EntityFluentCreateRequest(string containerId, IEntityService service)
    {
        _containerId = containerId;
        _service = service;
    }

    // Fluent methods
    public EntityFluentCreateRequest WithName(string name)
    {
        _name = name;
        return this;
    }

    // Validation
    public void Validate()
    {
        _validationErrors.Clear();
        // Validation logic
        if (_validationErrors.Any())
            throw new ClickUpRequestValidationException("Validation failed", _validationErrors);
    }

    // Terminal method
    public async Task<Entity> CreateAsync(CancellationToken cancellationToken = default)
    {
        Validate();
        // Implementation
    }
}
```

### 2. Method Chaining Safety

- All fluent methods must return `this` or the appropriate builder type
- Validate parameters at the method level when possible
- Use nullable reference types to indicate optional parameters
- Implement compile-time safety through generic constraints where applicable

### 3. State Validation

- Implement validation at multiple levels:
  - Parameter validation in individual methods
  - State validation in `Validate()` method
  - Final validation before execution
- Use descriptive error messages
- Collect all validation errors before throwing exceptions

## Advanced Patterns

### 1. Conditional Chaining

```csharp
public EntityFluentCreateRequest When(bool condition, Func<EntityFluentCreateRequest, EntityFluentCreateRequest> configure)
{
    return condition ? configure(this) : this;
}
```

### 2. Fluent Composition

```csharp
public EntityFluentCreateRequest WithConfiguration(Action<EntityConfigurationBuilder> configure)
{
    var builder = new EntityConfigurationBuilder();
    configure(builder);
    _configuration = builder.Build();
    return this;
}
```

### 3. Type-Safe Chaining

```csharp
public interface IEntityFluentBuilder<TBuilder> where TBuilder : IEntityFluentBuilder<TBuilder>
{
    TBuilder WithName(string name);
}
```

## Error Handling

### 1. Validation Exceptions

- Use `ClickUpRequestValidationException` for validation errors
- Include all validation errors in a single exception
- Provide clear, actionable error messages

### 2. State Management

- Maintain immutable state where possible
- Use defensive copying for mutable collections
- Clear validation errors before re-validation

## Documentation Standards

### 1. XML Documentation

- Document all public methods and properties
- Include parameter descriptions and constraints
- Provide usage examples for complex scenarios
- Document exceptions that may be thrown

### 2. Code Examples

```csharp
/// <summary>
/// Creates a new task with the specified configuration.
/// </summary>
/// <param name="name">The task name (required)</param>
/// <returns>The fluent builder for method chaining</returns>
/// <exception cref="ArgumentNullException">Thrown when name is null or empty</exception>
/// <example>
/// <code>
/// var task = await client.Tasks
///     .CreateTask(listId)
///     .WithName("My Task")
///     .WithDescription("Task description")
///     .WithPriority(1)
///     .CreateAsync();
/// </code>
/// </example>
public TaskFluentCreateRequest WithName(string name)
```

## Performance Considerations

### 1. Memory Management

- Minimize object allocations in fluent chains
- Use object pooling for frequently created builders
- Implement `IDisposable` when managing resources

### 2. Async Patterns

- Always provide `CancellationToken` support
- Use `ConfigureAwait(false)` in library code
- Implement proper async enumerable patterns for collections

## Testing Guidelines

### 1. Unit Testing

- Test each fluent method in isolation
- Test method chaining scenarios
- Test validation logic thoroughly
- Test error conditions and edge cases

### 2. Integration Testing

- Test complete fluent chains end-to-end
- Verify API contract compliance
- Test cancellation token behavior

## Migration Strategy

### 1. Backward Compatibility

- Maintain existing method signatures
- Add new methods alongside existing ones
- Use `[Obsolete]` attribute for deprecated methods
- Provide migration documentation

### 2. Gradual Adoption

- Implement new patterns in new features first
- Refactor existing code incrementally
- Update documentation and examples progressively

## Code Analysis Rules

### 1. Naming Conventions

- Enforce consistent naming patterns
- Validate method return types
- Check for proper async method naming

### 2. Pattern Compliance

- Ensure fluent methods return appropriate types
- Validate presence of terminal methods
- Check for proper validation implementation

## Examples

### Basic Usage

```csharp
// Task creation with fluent API
var task = await client.Tasks
    .CreateTask(listId)
    .WithName("Important Task")
    .WithDescription("This task needs attention")
    .WithPriority(1)
    .WithDueDate(DateTimeOffset.Now.AddDays(7))
    .WithAssignees(new[] { userId1, userId2 })
    .CreateAsync();
```

### Advanced Usage with Conditional Logic

```csharp
// Conditional configuration
var taskBuilder = client.Tasks.CreateTask(listId)
    .WithName("Conditional Task")
    .When(isUrgent, t => t.WithPriority(1))
    .When(hasDeadline, t => t.WithDueDate(deadline));

var task = await taskBuilder.CreateAsync();
```

### Composition Pattern

```csharp
// Using configuration builders
var space = await client.Spaces
    .CreateSpace(workspaceId)
    .WithName("Development Space")
    .WithFeatures(features => features
        .EnableTimeTracking()
        .EnableCustomFields()
        .DisablePortfolios())
    .CreateAsync();
```

This document serves as the foundation for consistent fluent API design across the ClickUp API Client library.