# Jules's Knowledge Base

This file contains specific notes, conventions, and information Jules needs to remember about this repository and the ongoing tasks.

## Project Overview & Goals
- Objective: Develop a professional-grade .NET client library for the ClickUp API, incorporating best practices for design, resilience, developer experience, and AI integration.
- Project Type: .NET 9 Core
- Master Conceptual Plan: `docs/plans/geminiPlan.md` (provides comprehensive architectural guidance).
- Operational Guidance: Derived from the latest user instructions, which supersede any previous specific operational plan documents.
- OpenAPI Spec: `docs/OpenApiSpec/ClickUp-6-17-25.json`
- API Reference Website: `https://developer.clickup.com/reference/`

## Current Project State:
- **Models (`src/ClickUp.Api.Client.Models`):** Considered complete as per `docs/plans/01-core-models-actual.md`.
- **Service Interfaces (`src/ClickUp.Api.Client.Abstractions`):** Defined, but require refinement to use concrete DTOs instead of generic `object` types and to ensure parameter correctness. `docs/plans/02-abstractions-interfaces-actual.md` tracks their existence.
- **Service Implementations (`src/ClickUp.Api.Client`):** Not yet started. The `Services` folder is currently empty.
- **`ClickUp.Net.Abstractions`:** This path appears to be deprecated or non-existent. The focus is on `ClickUp.Api.Client.Abstractions`.

## Current Task (as per prompt on 2025-06-20):
1.  **Review Plans:** Go over `docs/plans/`, especially `docs/plans/geminiPlan.md`.
2.  **Codebase Analysis:**
    *   Review the existing codebase (`src/`) to understand its current structure and implementation.
    *   Identify missing request objects, response objects, and entities. Examples mentioned: `MergeTasksRequest`, `AccessTokenResponse`, `Workspace`, `GetChatChannelsResponse`.
    *   The creation of these missing objects is expected to resolve CS0246 (type not found) errors.
3.  **Build and Test:** After addressing missing components, build the solution and run tests to ensure stability.

## Key Files & Directories:
- OpenAPI Spec: `docs/OpenApiSpec/ClickUp-6-17-25.json`
- API Reference: `https://developer.clickup.com/reference/`
- Master Conceptual Plan: `docs/plans/geminiPlan.md`
- Model Implementation Tracking: `docs/plans/01-core-models-actual.md`
- Interface Implementation Tracking: `docs/plans/02-abstractions-interfaces-actual.md`
- This file (Project Notes): `jules.md`
- Prompt history: `docs/prompts.md`
- Models: `src/ClickUp.Api.Client.Models/`
- Abstractions: `src/ClickUp.Api.Client.Abstractions/`
- Client Implementation: `src/ClickUp.Api.Client/`
- Tests: `src/ClickUp.Api.Client.Tests/`
- Examples: `examples/`
- .NET install script: `utilities/dotnet-install.sh` (note: leading slash removed as per `ls` output)

## Build and Test Commands:
- Install .NET 9: `utilities/dotnet-install.sh --channel 9.0` (run from the repository root)
- Build: `dotnet build src/ClickUp.Api.sln --nologo` (run from the repository root, or `dotnet build ClickUp.Api.sln --nologo` from the `src` folder)
- Test: `dotnet test src/ClickUp.Api.sln` (run from the repository root, or `dotnet test` from the `src` folder if the context is already there)


## Notes on `geminiPlan.md`:
The `geminiPlan.md` is the primary source for architectural principles, including:
- SOLID principles, Facade, Strategy, Builder, Observer patterns.
- Fluent interfaces for usability.
- Clean Architecture (Abstractions, Client, Examples projects).
- `IHttpClientFactory`, Polly for resilience.
- Custom exception hierarchy.
- Authentication (Personal Token, OAuth 2.0).
- Pagination, async streaming.
- Webhook helpers.
- DocFX for documentation.
- AI integration (Semantic Kernel, MCP).
The current tasks aim to implement these aspects systematically, guided by user prompts.

## General Instructions from User:
- Save all prompts to `/docs/prompts.md` with date and time.
- Keep this file (`jules.md`) updated with relevant repository/task information.
