# Development Portfolio — ASPNetCoreApp

The single shared handoff/status file across all four tabs. Overwrite — never append. Whichever tab you're in, read this first.

## Current state

**Stage:** Implemented — handed off to Review
**Owner:** Review

## Handoff log

_(newest first — one entry per handoff between tabs)_

- **2026-07-17 — Engineering → Review.** Implemented TICKET-1 on branch `feature/rag-minimal-api` (branched from `master` after merging `LiveCoding`'s ASP.NET Core scaffold in). Added `Services/EmbeddingService.cs` (Ollama), `Services/VectorStoreService.cs` (Qdrant), `Services/AnswerService.cs` (Claude via `Anthropic` NuGet package, `HttpClient` injected for testability), `Services/DependencyUnavailableException.cs`, and mapped `POST /documents` / `POST /ask` in `Program.cs`. All five error-handling cases from the ticket are implemented (400 on empty text/question, 502 identifying Ollama/Qdrant/Claude on failure, 404 on zero Qdrant search results). Config: Ollama/Qdrant base URLs in `appsettings.json`; Anthropic API key in user secrets (set by user directly, not by Claude).
  - Unit tests: `ASPNetCoreApp.Tests` (sibling project, xUnit) — 17 tests total, all passing. Mocked-`HttpClient` tests for each service (request shape + all failure-mapping cases) plus `WebApplicationFactory`-based endpoint tests (happy path, 400, 404) with typed clients' primary handlers swapped for fakes.
  - Smoke test: ran the app for real against live local dependencies — Ollama (`nomic-embed-text` already pulled) and Qdrant (Docker container already running, `docs` collection present). `POST /documents` with a real sentence about the Eiffel Tower returned `{"id":1}`. `POST /ask` with "Where is the Eiffel Tower located?" returned a correct grounded answer from Claude. Empty-body 400s verified live for both endpoints. The 404 (empty search results) and 502 (dependency failure) paths were **not** exercised live — doing so would require emptying/breaking the shared local Qdrant collection — but both are covered by the automated endpoint/unit tests above.
  - Not pushed. Awaiting Review sign-off before requesting push permission.

- **2026-07-17 — Documentation → Engineering.** Wrote `Documentation/Tickets/TICKET-1-rag-minimal-api.md` from `Planning/Plan-rag-minimal-api.md`. Ticket covers both endpoints, all five error-handling cases, service structure, and required test coverage (unit + smoke). Engineering should implement per the ticket and plan.
- **2026-07-17 — Planning → Documentation.** Approved `Planning/Plan-rag-minimal-api.md`: minimal API (`POST /documents`, `POST /ask`) wrapping the RAG pipeline from the console prototype at `C:\projects\Job Hunt\Skill-Up\rag-project` (Ollama embeddings → Qdrant vector search → Claude generation). Scope: error handling for unreachable/failing Ollama/Qdrant/Claude and empty search results; explicitly excludes chunking, doc list/delete, and multi-doc discrimination. Documentation should write the ticket from this plan.

## Open items

- Review needs to check out `feature/rag-minimal-api` for its review pass.
- 404 and 502 paths are verified only via automated tests, not a live run — flag if Review wants a live repro (would need a disposable Qdrant collection).
