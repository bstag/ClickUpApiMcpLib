# FluentNext & Core SDK Consistency Refactor Plan

> **Status:** Draft ☐  
> **Supersedes:** `docs/plans/fluentNext.md`

This document enumerates the concrete refactor steps required to align the Core SDK **services** and the **FluentNext** layer under a single, consistent design philosophy.  
Each step contains:

* _Why_ – the rationale for the change.  
* **Tasks** – check-boxes to be ticked off via PRs.  
* **Validation Rule** – how we confirm the step is complete (build & test gates, analyzers, etc.).

---

## 1 · Unified Identifier Ordering
**Why:** Inconsistent parameter ordering across services causes cognitive load and usage errors.

**Tasks**
- [ ] 1.1 Add section to `CONTRIBUTING.md` specifying canonical order: `workspaceId → spaceId → folderId → listId → entityId`.
- [ ] 1.2 Introduce Roslyn analyzer `ClickUp.IdOrderAnalyzer` & fixer.
- [ ] 1.3 Refactor existing service & fluent signatures to follow rule (breaking change acceptable — no existing consumers).

**Validation Rule**
```bash
# Gate in CI
 dotnet build -warnaserror
 dotnet test
# Analyzer must report 0 diagnostics in src/*
```

---

## 2 · DTO Naming Scheme – *Request / Response*
**Why:** We currently mix `*Dto`, `*Response`, etc. Uniform names improve discoverability.

**Tasks**
- [ ] 2.1 Script to scan for DTO suffixes and emit report.
- [ ] 2.2 Rename classes to `XxxRequest` / `XxxResponse`. 
- [ ] 2.3 Update serializers & tests.

**Validation Rule:** `grep -R "class .*Dto" src | wc -l` → 0  
All unit tests green.

---

## 3 · Pagination Abstraction (`IPagedResult<T>`)
**Why:** Paging parameters are re-invented in multiple services.

**Tasks**
- [ ] 3.1 Add `IPagedResult<T>` & helper extension `AsPagedResult()`.
- [ ] 3.2 Refactor services to return `IPagedResult<T>` where paging applies.
- [ ] 3.3 Add fluent helpers `.Page(number,size)`.

**Validation Rule:** No method exposes raw `page`/`pageSize` parameters; contract tests verify.

---

## 4 · Mandatory `CancellationToken`
**Why:** Missing tokens make graceful shutdown impossible.

**Tasks**
- [ ] 4.1 Enable nullable and `CA2016` ruleset (`async` method missing token).
- [ ] 4.2 Fix violations across codebase.

**Validation Rule:** `dotnet build -p:TreatWarningsAsErrors=true` passes with CA2016 enabled.

---

## 5 · Centralised Exception Handling
**Why:** Today, services throw mixed exception types.

**Tasks**
- [ ] 5.1 Create `ClickUpApiExceptionFactory`.
- [ ] 5.2 Convert services to use factory.

**Validation Rule:** Contract tests simulate 4xx/5xx responses and assert thrown type derives from `ClickUpApiException`.

---

## 6 · Shared Value Objects (Filters, TimeRange, etc.)
**Why:** Reduce duplicated query-string logic.

**Tasks**
- [ ] 6.1 Introduce `TimeRange`, `SortDirection`, `DateFilter` value objects.
- [ ] 6.2 Migrate existing services/builders.

**Validation Rule:** No custom string builders remain for date ranges (`grep "start_date="` limited to value-object files).

---

## 7 · Builder Validation & `Validate()` API
**Why:** Eager validation prevents late runtime failures.

**Tasks**
- [ ] 7.1 Add optional `.Validate()` that throws `ValidationException` synchronously.
- [ ] 7.2 Wire into `ExecuteAsync()` to auto-validate if not previously called.

**Validation Rule:** New unit tests pass for missing ids, expecting `ValidationException`.

---

## 8 · Service ↔ Fluent Contract Tests
**Why:** Guard against divergence (method missing in one layer).

**Tasks**
- [ ] 8.1 Reflection-based test that for each service public async method there is a fluent equivalent.

**Validation Rule:** Test project `ClickUp.Api.ContractTests` passes.

---

## 9 · Helper Consolidation
**Why:** Remove duplicated `QueryStringBuilder`, etc.

**Tasks**
- [ ] 9.1 Move shared helpers into a dedicated `Helpers` folder within an existing project (e.g., `ClickUp.Api.Client`).
- [ ] 9.2 Delete redundant implementations.

**Validation Rule:** Single implementation per helper namespace; build + tests green.

---

## 10 · Nullable Reference Types & Code Clean-up
**Why:** Ensure null-safety and consistent `!` suppression.

**Tasks**
- [ ] 10.1 Enable `<Nullable>enable</Nullable>` in all csproj files.
- [ ] 10.2 Fix warnings.

**Validation Rule:** `dotnet build -warnaserror` produces 0 nullable warnings.

---

## Continuous Integration Updates
- [ ] Update GitHub Actions to run `dotnet build`, `dotnet test`, analyzer scan, and markdown link check on every PR.

**Validation Rule:** New CI pipeline passes before merge.

---

## Glossary
| Term | Description |
|------|-------------|
| **Core SDK** | `Services/*` layer interacting with ClickUp HTTP API |
| **FluentNext** | Fluent builders (`Fluent/*`) providing ergonomic façade |
| **Contract Test** | Test ensuring parity between layers |
| **Analyzer** | Roslyn rule enforcing conventions |

---

*End of plan.*
