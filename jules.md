# Jules's Knowledge Base

This file contains specific notes, conventions, and information Jules needs to remember about this repository and the ongoing tasks.

## Project Overview & Goals
- Objective: Develop a professional-grade .NET client library for the ClickUp API, incorporating best practices for design, resilience, developer experience, and AI integration.
- Current Phase: Finalizing Core Contracts (Refining Service Interfaces) and Updating Documentation.
- Master Plan: `docs/plans/geminiPlan.md` (provides comprehensive architectural guidance).
- Consolidated Operational Plan: `docs/plans/NEW_OVERALL_PLAN.md` (this is the plan currently being executed).
- OpenAPI Spec: `docs/OpenApiSpec/ClickUp-6-17-25.json`.

## Current Project State:
- **Models (`src/ClickUp.Api.Client.Models`):** Considered complete as per `docs/plans/01-core-models-actual.md`.
- **Service Interfaces (`src/ClickUp.Api.Client.Abstractions`):** Defined, but require refinement to use concrete DTOs instead of generic `object` types and to ensure parameter correctness. `docs/plans/02-abstractions-interfaces-actual.md` tracks their existence.
- **Service Implementations (`src/ClickUp.Api.Client`):** Not yet started. The `Services` folder is currently empty.
- **`ClickUp.Net.Abstractions`:** This path appears to be deprecated or non-existent. The focus is on `ClickUp.Api.Client.Abstractions`.

## Current Task:
- Following the consolidated plan approved by the user.
- Current Step: "Refine Service Interfaces in `ClickUp.Api.Client.Abstractions`" (after this initial documentation update step is complete).

## Key Files & Directories:
- OpenAPI Spec: `docs/OpenApiSpec/ClickUp-6-17-25.json`
- Master Conceptual Plan: `docs/plans/geminiPlan.md`
- Current Operational Plan: `docs/plans/NEW_OVERALL_PLAN.md` (contains the detailed plan Jules is following)
- Model Implementation Tracking: `docs/plans/01-core-models-actual.md`
- Interface Implementation Tracking: `docs/plans/02-abstractions-interfaces-actual.md`
- This file (Project Notes): `jules.md`
- Prompt history: `docs/prompts.md`
- Models: `src/ClickUp.Api.Client.Models/`
- Abstractions: `src/ClickUp.Api.Client.Abstractions/`
- Client Implementation: `src/ClickUp.Api.Client/`
- Tests: `src/ClickUp.Api.Client.Tests/`
- Examples: `examples/`

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
The new consolidated plan aims to implement these aspects systematically.
