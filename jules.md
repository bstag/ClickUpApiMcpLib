# Jules's Knowledge Base

This file contains specific notes, conventions, and information Jules needs to remember about this repository and the ongoing tasks.

## Project Overview & Goals
- Objective: Develop a professional-grade .NET client library for the ClickUp API, incorporating best practices for design, resilience, developer experience, and AI integration.
- Master Conceptual Plan: `docs/plans/geminiPlan.md` (provides comprehensive architectural guidance).
- Operational Guidance: Derived from the latest user instructions, which supersede any previous specific operational plan documents.
- OpenAPI Spec: `docs/OpenApiSpec/ClickUp-6-17-25.json`.

## Current Project State:
- **Models (`src/ClickUp.Api.Client.Models`):** Considered complete as per `docs/plans/01-core-models-actual.md`.
- **Service Interfaces (`src/ClickUp.Api.Client.Abstractions`):** Defined, but require refinement to use concrete DTOs instead of generic `object` types and to ensure parameter correctness. `docs/plans/02-abstractions-interfaces-actual.md` tracks their existence.
- **Service Implementations (`src/ClickUp.Api.Client`):** Not yet started. The `Services` folder is currently empty.
- **`ClickUp.Net.Abstractions`:** This path appears to be deprecated or non-existent. The focus is on `ClickUp.Api.Client.Abstractions`.

## Current Task:
Review existing plans (especially `docs/plans/geminiPlan.md`), the OpenAPI spec (`docs/OpenApiSpec/ClickUp-6-17-25.json`), and the current codebase to identify missing components and necessary refinements. Following this review, build and test the project. Subsequently, refine service interfaces and implement any missing parts, guided by the user's latest instructions.

## Key Files & Directories:
- OpenAPI Spec: `docs/OpenApiSpec/ClickUp-6-17-25.json`
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
- .NET install script: `/utilities/dotnet-install.sh`

## Build and Test Commands:
- Install .NET 9: `/utilities/dotnet-install.sh --channel 9.0` (run from the repository root)
- Build: `dotnet build src/ClickUp.Api.sln --nologo` (run from the repository root, or `dotnet build ClickUp.Api.sln --nologo` from the `src` folder)
- Test: `dotnet test src/ClickUp.Api.sln` (run from the repository root, or `dotnet test` from the `src` folder)


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
