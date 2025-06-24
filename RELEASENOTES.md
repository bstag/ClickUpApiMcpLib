# Release Notes – ClickUp .NET SDK

## v0.1.0 – Initial Public Preview (2025-06-24)

### 🚀 Highlights

* **Full API Coverage** – Typed service classes for Tasks, Comments, Time Tracking, Spaces, Lists, Folders, Goals, Users, Webhooks, and more.
* **Dependency-Injection Ready** – One-liner `services.AddClickUpClient()` registers a resilient `HttpClient` plus all service interfaces.
* **Resilient Networking** – Polly retry policy with exponential back-off baked in; easily overridden.
* **Nullable & XML-Documented Models** – 300+ request/response DTOs, fully nullable-annotated with IntelliSense docs.
* **Examples** – `ConsoleDemo`, `RetryPolicyDemo`, `WebhookListener` show common scenarios.
* **Testing** – Unit test suite and opt-in integration tests hitting the live ClickUp API.

### 🔔 Notes

* Versioning follows **Semantic Versioning**. Pre-1.0 releases may change public APIs.
* Built against **.NET 9**; should also work on .NET 8 with `EnablePreviewFeatures`.

### ⚠️ Breaking Changes

_None – first release._

### 🐞 Known Issues / Limitations

1. Webhook validation helper not yet implemented.
2. API rate-limit headers are not surfaced; retry policy uses status codes only.
3. gRPC transport pending until ClickUp publishes official proto definitions.

### 📈 Roadmap

* Strongly typed webhook signature verifier.
* SourceLink & symbol upload for step-through debugging.
* Auto-generated API docs site via DocFX.

---

Thank you for trying the early preview!  Feedback and PRs are welcome – open an issue on GitHub.
