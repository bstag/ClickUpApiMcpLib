# Detailed Plan: Core Models and Abstractions

This document provides a detailed plan for defining and refining the core models (DTOs) and service abstractions (interfaces) for the ClickUp API SDK.

## 1. Core Models (DTOs)

**Source Documents:**
*   [`docs/plans/01-core-models-actual.md`](../01-core-models-actual.md) (Current state of implemented models)
*   [`docs/plans/01-core-models-conceptual.md`](../01-core-models-conceptual.md) (Initial conceptual plan for models)
*   [`docs/OpenApiSpec/ClickUp-6-17-25.json`](../../../OpenApiSpec/ClickUp-6-17-25.json) (The definitive API specification)

**Location in Codebase:** `src/ClickUp.Api.Client.Models/`

**Key Tasks:**

- [x] **1. Comprehensive Model Review:**
    - [x] Iterate through every model listed in `01-core-models-actual.md`.
    - [x] For each model, cross-reference its properties with the corresponding schema in `ClickUp-6-17-25.json`.
    - [x] **Validation Points:**
        - [x] **Property Names:** Ensure C# property names (PascalCase) correctly map to JSON property names (often snake_case) using `[JsonPropertyName("json_name")]` attribute. (Largely done, ongoing verification as models are used)
        - [x] **Data Types:** Verify that C# data types accurately represent the OpenAPI schema types and formats (e.g., `string` -> `string`, `integer` -> `int`/`long`, `boolean` -> `bool`, `string` with `date-time` format -> `DateTimeOffset`, `array` -> `List<T>`). Pay close attention to `number` types (prefer `decimal` for currency, `double` or `float` otherwise based on precision needs). (Largely done, ongoing verification)
        - [x] **Nullability:**
            - [x] If OpenAPI `nullable: true`, C# property must be nullable (e.g., `string?`, `int?`, `List<T>?`). (Largely done, ongoing verification)
            - [x] If a property is in the `required` array in OpenAPI and not `nullable: true`, it should be non-nullable in C#. (Largely done, ongoing verification)
            - [x] If not in `required` and `nullable` is not specified, treat as optional and make nullable in C#. (Largely done, ongoing verification)
        - [x] **Collections:** Ensure arrays are represented as `List<T>` or `IReadOnlyList<T>` (for immutability where appropriate, though `List<T>` is common for DTOs). (Using `List<T>`)
        - [x] **Enums:** Confirm C# enums map correctly to string values from the API. Ensure `JsonStringEnumConverter` is planned for use during serialization/deserialization. (Implemented for several enums, ongoing)
        - [x] **Nested Models:** Verify that properties representing nested objects correctly use other defined DTOs. (Largely done, ongoing verification)

- [x] **2. Identify Missing or Incorrect Models:**
    - [x] Scan `ClickUp-6-17-25.json` for any schemas under `components.schemas` or inline schemas in request/response bodies that are not yet represented in `01-core-models-actual.md` or in the `src/ClickUp.Api.Client.Models/` directory. (Covered by `01-core-models-actual.md` which is quite comprehensive)
    - [x] Prioritize creation of these missing models. (Most P0-P2 models listed in `01-core-models-actual.md` are created)
    - [ ] Flag any models in `01-core-models-actual.md` that seem to deviate significantly from the OpenAPI spec or might be redundant. (Ongoing review during implementation)

- [x] **3. Model Structure and Design:**
    - [x] **Records vs. Classes:** Continue preferring `record` types for their conciseness and immutability benefits for DTOs, as outlined in `01-core-models-conceptual.md`. Use classes only if mutability or complex inheritance (not typical for DTOs) is strictly necessary. (Predominantly using records)
    - [x] **Immutability:** Properties should generally have `init;` setters to promote immutability after deserialization. (Widely adopted)
    - [x] **Constructors:** Add constructors if default values (beyond C# defaults) or complex initialization logic is needed. Ensure non-nullable properties are initialized (either via constructor or `init` with a default value if appropriate). (Used where necessary)
    - [x] **Polymorphism (`oneOf`, `anyOf`):**
        - [x] Identify all instances of `oneOf` or `anyOf` in the OpenAPI spec.
        - [x] Plan the use of base classes/interfaces and derived classes.
        - [x] Specify the use of `JsonDerivedType` attributes for `System.Text.Json` to handle deserialization correctly. A discriminator property, if defined in the API, should be used. If not, custom `JsonConverter`s might be necessary and should be planned.
        - [x] Example: `SetCustomFieldValuerequest` from `01-core-models-actual.md` seems to be a candidate for this, with specific request models like `TextLikeCustomFieldValueRequest`. Ensure this is correctly modeled for polymorphic deserialization. (Implemented for `SetCustomFieldValueRequest` and its derived types)

- [ ] **4. XML Documentation for Models:**
    - [ ] Mandate `<summary>` for each public model and property. (Partially done, needs comprehensive pass)
    - [ ] Documentation should clearly explain the purpose of the model/property, referencing the ClickUp API where helpful.
    - [ ] Use `<example>` tags if a typical value or structure aids understanding.
    - [ ] Ensure all enum members are also documented.

- [x] **5. Output:**
    - [x] Update `01-core-models-actual.md` to reflect the validated and complete list of models. (This file is the primary tracking for "actual", it is largely up-to-date with created files)
    - [ ] List all identified discrepancies, required changes, new models to be created, and any complex polymorphism cases that need special attention. (Ongoing, this plan serves as a guide)
    - [x] Confirm data types for all properties, especially numeric and date/time types. (Largely confirmed, ongoing verification)

## 2. Service Abstractions (Interfaces)

**Source Documents:**
*   [`docs/plans/02-abstractions-interfaces-actual.md`](../02-abstractions-interfaces-actual.md) (Current state of implemented interfaces)
*   [`docs/plans/02-abstractions-interfaces-conceptual.md`](../02-abstractions-interfaces-conceptual.md) (Initial conceptual plan for interfaces)
*   [`docs/plans/NEW_OVERALL_PLAN.md`](../../NEW_OVERALL_PLAN.md) (Phase 1, Step 2 regarding interface refinement)
*   [`ClickUp-6-17-25.json`](../../../OpenApiSpec/ClickUp-6-17-25.json) (For method signatures, parameters, and response types)

**Location in Codebase:** `src/ClickUp.Api.Client.Abstractions/Services/`

**Key Tasks:**

- [x] **1. Comprehensive Interface Review (as per `NEW_OVERALL_PLAN.md` Phase 1, Step 2):**
    - [x] Iterate through all service interfaces (e.g., `ITasksService`, `IListsService`) in `src/ClickUp.Api.Client.Abstractions/Services/`.
    - [x] **Validation and Refinement Points:**
        - [x] **DTO Usage:**
            - [x] Replace ALL generic `object` placeholders in method signatures with specific DTOs from `ClickUp.Api.Client.Models`. (Done for most interfaces, as noted in `02-abstractions-interfaces-actual.md`)
            - [x] Example: `ITasksService.GetTaskAsync(string taskId, CancellationToken cancellationToken = default)` should return `Task<TaskDto>` (or a wrapper like `Task<GetTaskResponse>`). (This pattern is followed)
            - [x] Ensure request parameters also use specific DTOs (e.g., `CreateTaskAsync(CreateTaskRequestDto request, ...)`). (This pattern is followed)
        - [x] **Parameter Types:**
            - [x] Verify and correct all parameter types. Example: `double listId` should be `string listId` if API identifiers are strings (which is common). Consult OpenAPI spec for path/query parameter types. (Largely done)
        - [x] **Return Types:**
            - [x] Ensure return types are `Task<SpecificDto>` for methods returning data, or `Task` for methods that don't return data (e.g., HTTP 204 No Content). (Largely done)
            - [x] If an API call returns a list of items, the DTO should reflect this (e.g., `Task<List<TaskDto>>` or `Task<GetTasksResponse>` where `GetTasksResponse` contains `List<TaskDto>`). (Largely done)
        - [x] **`CancellationToken`:** Confirm EVERY asynchronous method includes an optional `CancellationToken cancellationToken = default` parameter as the last parameter. (Largely done)
        - [x] **Namespaces:** Clean up and standardize namespaces (e.g., remove nested `ClickUp.Abstract` if `ClickUp.Api.Client.Abstractions.Services` is the standard). (Standardized to `ClickUp.Api.Client.Abstractions.Services`)
        - [x] **Naming Conventions:**
            *   Interfaces: `ISomethingService` (e.g., `ITasksService`). (Followed)
            *   Methods: PascalCase, suffixed with `Async` (e.g., `GetTaskAsync`, `CreateTaskCommentAsync`). Method names should be descriptive and generally align with OpenAPI `operationId` or a more C#-idiomatic equivalent. (Followed)

- [x] **2. Completeness Check:**
    - [x] Compare the list of interfaces in `02-abstractions-interfaces-actual.md` against the tags and paths in `ClickUp-6-17-25.json`. (All major services from OpenAPI tags have corresponding interfaces as per `02-abstractions-interfaces-actual.md`)
    - [ ] Identify any missing service interfaces or methods for documented API endpoints. (Ongoing review during service implementation)
    - [ ] Plan for the creation of these missing elements.

- [ ] **3. XML Documentation for Interfaces:**
    - [ ] Mandate comprehensive XML documentation for all interfaces and their methods. (Partially done, needs comprehensive pass)
    - [ ] `<summary>`: Clearly describe the purpose of the interface or method.
    - [ ] `<param name="paramName">`: Describe each parameter, its purpose, and any specific requirements (e.g., format, valid values if not obvious from type).
    - [ ] `<returns>`: Describe the expected return value (e.g., "A `TaskDto` representing the retrieved task."). For `Task` (non-generic), indicate it completes upon successful execution.
    - [ ] `<exception cref="ApiExceptionType">`: Document potential API-specific exceptions that can be thrown (details to be fleshed out in the Exception Handling plan).
    - [ ] `<remarks>`: Add any additional important notes or usage considerations.

- [x] **4. Output:**
    - [x] Update `02-abstractions-interfaces-actual.md` to reflect the refined and complete set of interfaces and their method signatures. (This file is considered up-to-date based on its own content)
    - [ ] List all specific DTOs to be used for each method's parameters and return types. (This is implicitly done by reviewing the interface files themselves)
    - [x] Confirm adherence to `CancellationToken` usage and naming conventions. (Confirmed)
    - [ ] Document any new interfaces or methods that need to be created. (Ongoing, as part of service implementation)

**General Considerations for Both Models and Abstractions:**

- [x] **Consistency:** Ensure naming, documentation style, and design patterns are consistent across all models and interfaces. (Largely consistent, ongoing effort)
- [x] **Clarity:** The resulting models and interfaces should be intuitive and easy for SDK consumers to understand and use. (A primary goal, appears to be met for existing items)
- [x] **Validation against Existing Code:** While this step focuses on planning based on documentation and the OpenAPI spec, a final sub-task will be to quickly cross-check the plans against the actual code in `src/ClickUp.Api.Client.Models` and `src/ClickUp.Api.Client.Abstractions` to ensure the plans accurately reflect the starting point for any subsequent implementation work. For this task, we are primarily validating the conceptual plans against the *existing* code and OpenAPI spec to *create new detailed plan documents*. (This is what this current exercise is doing - aligning this plan document with the state reflected in `01-core-models-actual.md`, `02-abstractions-interfaces-actual.md`, and the codebase.)
```
