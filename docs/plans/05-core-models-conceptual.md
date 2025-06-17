# Phase 2, Step 5: Define Core Models (Conceptual)

This document outlines the conceptual approach for translating OpenAPI schemas from the ClickUp API (as defined in `/docs/OpenApiSpec/ClickUp-6-17-25.json`) into C# models for the .NET client library.

## General Approach

The core idea is to represent each schema defined in the OpenAPI specification as a C# class or record. These models will primarily serve as Data Transfer Objects (DTOs) for request bodies and response payloads.

## C# Model Type: Records vs. Classes

- **Default Choice: `record` types.** For most models, C# `record` types (specifically `record class`) will be preferred over traditional `class` types.
    - **Immutability:** Records offer concise syntax for creating immutable objects, which is ideal for DTOs as API model state generally doesn't change after creation/deserialization.
    - **Value-based Equality:** Records provide built-in value-based equality, which can be useful for comparisons and testing.
    - **Conciseness:** They reduce boilerplate code for constructors, properties, and `ToString()` overrides.
- **Exceptions (Traditional `class`):**
    - If a model requires mutable state for a specific reason post-deserialization (though this should be rare for API DTOs).
    - If complex inheritance hierarchies are needed that don't fit well with record inheritance (though OpenAPI typically promotes composition over inheritance).

## Mapping OpenAPI Data Types to C#

| OpenAPI Type | OpenAPI Format | C# Equivalent        | Notes                                                                 |
|--------------|----------------|----------------------|-----------------------------------------------------------------------|
| `string`     | `date`         | `DateTime`           |                                                                       |
| `string`     | `date-time`    | `DateTimeOffset`     | Preferred over `DateTime` for time zone handling.                     |
| `string`     | `byte`         | `byte[]`             | For Base64 encoded strings.                                           |
| `string`     | `binary`       | `byte[]` or `Stream` | `Stream` might be better for request/response bodies directly.        |
| `string`     | *(none)*       | `string`             |                                                                       |
| `number`     | `float`        | `float`              |                                                                       |
| `number`     | `double`       | `double`             |                                                                       |
| `number`     | *(none)*       | `decimal` or `double`| `decimal` for financial data, `double` for general numeric values.    |
| `integer`    | `int32`        | `int`                |                                                                       |
| `integer`    | `int64`        | `long`               |                                                                       |
| `integer`    | *(none)*       | `int`                | Default, consider `long` if range is a concern.                       |
| `boolean`    | *(none)*       | `bool`               |                                                                       |
| `array`      | *(varies)*     | `List<T>` or `T[]`   | `List<T>` is generally more flexible. `T` is the item type.            |
| `object`     | *(none)*       | Custom C# `record`   | Corresponds to a nested schema.                                       |
| `object`     | `additionalProperties` | `Dictionary<string, T>` | For free-form objects. `T` is the type of the property values. |

## Naming Conventions

- **Classes/Records:** PascalCase, directly derived from the OpenAPI schema name (e.g., `Task`, `Comment`).
- **Properties:** PascalCase, matching the OpenAPI property names. If the OpenAPI names are not C#-idiomatic (e.g., contain underscores or start with numbers), `JsonPropertyName` attribute from `System.Text.Json.Serialization` will be used to map them to the original API names.
    - Example: OpenAPI `user_id` becomes C# `UserId` with `[JsonPropertyName("user_id")]`.

## Nullability and Required Properties

- C# nullable reference types (`?`) will be used extensively.
- A property will be nullable in C# if:
    - The OpenAPI schema defines it as `nullable: true`.
    - The property is not in the `required` list for the schema.
- If a property is in the `required` list and not `nullable: true`, it will be non-nullable in C#.
- For properties not in `required` and `nullable` is not explicitly set, they will be treated as optional and thus nullable in C# (this is a common default interpretation).

## Handling Relationships

- **Nested Objects:** Represented as properties of the corresponding custom C# record type.
    - Example: If a `Task` schema has a `creator` property referencing a `User` schema, the `Task` record will have a `User Creator { get; init; }` property.
- **Arrays of Objects:** Represented as `List<T>` where `T` is the C# record type for the items in the array.
    - Example: If a `Project` schema has a `members` property which is an array of `User` schemas, the `Project` record will have a `List<User> Members { get; init; }` property.

## Enumerations

- OpenAPI `enum`s will be translated to C# `enum` types.
- The `JsonStringEnumConverter` from `System.Text.Json.Serialization` will be configured globally to ensure proper serialization/deserialization of enums as strings (as is common in APIs).

## Inheritance and Polymorphism (AllOf, OneOf, AnyOf)

- **`allOf`:** Will generally be handled by composing properties from all subschemas into a single C# model. If actual C# inheritance makes sense and the OpenAPI structure supports it (e.g., a clear discriminator), it might be used.
- **`oneOf` / `anyOf`:** These represent polymorphic types and are more complex.
    - A common approach is to use a base class/interface and then derived classes for each possible schema in the `oneOf`/`anyOf` list.
    - `System.Text.Json` supports polymorphism with attributes like `JsonDerivedType` and potentially custom converters if the discriminator logic is complex. This needs careful review based on the specifics in the ClickUp OpenAPI.

## Example (Conceptual)

If OpenAPI has:

```yaml
components:
  schemas:
    TodoItem:
      type: object
      required:
        - id
        - description
      properties:
        id:
          type: string
          format: uuid
        description:
          type: string
        isComplete:
          type: boolean
          default: false
        dueDate:
          type: string
          format: date
          nullable: true
```

This might translate to (using `System.Text.Json.Serialization` attributes):

```csharp
// In Models project
public record TodoItem
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty; // Or ensure constructor enforces non-null

    [JsonPropertyName("isComplete")]
    public bool IsComplete { get; init; } // Default from OpenAPI can be handled by constructor or init

    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; init; }

    // Constructor if defaults or complex initialization needed
    public TodoItem(Guid id, string description)
    {
        Id = id;
        Description = description;
        IsComplete = false; // Explicitly setting default
    }
}
```

## Next Steps

- Obtain the actual `ClickUp-6-17-25.json` file.
- Start generating specific model definitions based on that file and this conceptual approach.
- Refine this approach as concrete examples and challenges arise from the actual API specification.
```
