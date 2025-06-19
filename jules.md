# Jules' Knowledge Base

This file contains specific notes, conventions, and information Jules needs to remember about this repository and the ongoing tasks.

## Project Overview & Goals
- Objective: Develop a .NET client library for the ClickUp API.
- Current Phase: Model creation based on OpenAPI specification (`docs/OpenApiSpec/ClickUp-6-17-25.json`).
- Conceptual Plan: `docs/plans/01-core-models-conceptual.md` (guides naming, C# type mapping, etc.).
- Implementation Tracking: `docs/plans/01-core-models-actual.md` (tracks specific schemas and their completion status).

## Development Process

1.  **Model Creation**: Implement C# models based on the OpenAPI specification.
    *   Group models into folders based on their purpose and the base endpoint type they are for.
2.  **Service Implementation**: Create services to interact with the API endpoints.
3.  **Interface Definitions**: Define abstractions for models and services.
4.  **Helper Utilities**: Develop any necessary helper functions.
5.  **Exception Handling**: Implement a global exception handling system.
6.  **Testing**: Write unit tests for the client library.
7.  **Example Projects**: Create example console and worker projects to demonstrate usage.

## Key Decisions & Progress (as of YYYY-MM-DD HH:MM:SS (Placeholder)):
- Successfully implemented P0, P1, and numerous P2 models from `01-core-models-actual.md`. This includes core entities, request/response wrappers, and helper DTOs/enums for features like Tasks, Folders, Spaces, Goals, Custom Fields, Time Tracking, Webhooks, v3 Docs, and v3 Chat.
- Folder Structure: Models are organized under `src/ClickUp.Api.Client.Models/` into `Common/`, `Entities/`, `RequestModels/`, and `ResponseModels/`, further categorized by feature (e.g., `Entities/Tasks`, `RequestModels/Goals`).
- Logging: User prompts are logged in `docs/prompts.md`. This file (`jules.md`) tracks project notes and context.
- Next Steps (Post Model Implementation): The overall project plan involves developing abstractions, service implementations for API endpoints, HTTP client helpers, and a global exception handling system.

## Important Files & Directories:
- OpenAPI Spec: `docs/OpenApiSpec/ClickUp-6-17-25.json`
- Conceptual Model Plan: `docs/plans/01-core-models-conceptual.md`
- Actual Model Plan / Implementation Tracking: `docs/plans/01-core-models-actual.md`
- This file (Project Notes): `jules.md`
- Prompt history: `docs/prompts.md`
- Main model directory: `src/ClickUp.Api.Client.Models/`
- Client library source (future): `src/ClickUp.Api.Client/`
- Abstractions (future): `src/ClickUp.Api.Client.Abstractions/`
- Test Project (future): `tests/ClickUp.Api.Client.Tests/`
- Example Projects (future): `examples/`

## Current Task Context (from last user prompt):
Continue implementing C# models for the ClickUp API client library.
This involves:
- Using the OpenAPI definition: `/docs/OpenApiSpec/ClickUp-6-17-25.json`
- Following the conceptual plan: `docs/plans/01-core-models-conceptual.md`
- Tracking progress in: `docs/plans/01-core-models-actual.md`
- Immediate next steps: Creating models such as UserGroup, SharingOptions, Role, CustomRole, Guest, and View. Following these, specific Request/Response models will be implemented.
- Grouping models into folders based on purpose and endpoint type.
- Saving prompts to `/docs/prompts.md`.
- Keeping track of repository/project information in `jules.md`.

## Current Focus

Continuing with the model creation plan, focusing on grouping models into appropriate folders.
