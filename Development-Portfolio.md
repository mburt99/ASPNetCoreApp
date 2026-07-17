# Development Portfolio — ASPNetCoreApp

The single shared handoff/status file across all four tabs. Overwrite — never append. Whichever tab you're in, read this first.

## Current state

**Stage:** TICKET-1 closed — no open handoff
**Owner:** none

## Handoff log

_(newest first — one entry per handoff between tabs)_

- **2026-07-17 — Documentation.** Closed out `Documentation/Tickets/TICKET-1-rag-minimal-api.md` (marked Status: Closed, all acceptance criteria met, noted the `Guid` id shape from Handoff-1's fix). Closed ticket needs no further handoff.
- **2026-07-17 — Engineering → Documentation.** Review signed off clean on `feature/rag-minimal-api` at `77b142a` (TICKET-1). Not yet pushed further or merged — push to `main` still requires explicit user authorization per `git-workflow.md`, not requested this round. Handing off to Documentation per user direction.

- **2026-07-17 — Review → Engineering.** Re-reviewed `feature/rag-minimal-api` at `77b142a`. Handoff-1 fix confirmed correct: `VectorStoreService.UpsertAsync` now returns `Guid.NewGuid()`, propagated through `/documents`' response; unused `QdrantPoint.Id` field dropped cleanly (never read post-search). Build clean, 17/17 tests pass. No new issues found. Signed off — clean. Engineering may request push authorization from the user per `git-workflow.md`.

- **2026-07-17 — Engineering → Review.** Fixed `Review/Handoffs/Handoff-1.md` issue #1: `VectorStoreService.UpsertAsync` now assigns a `Guid.NewGuid()` per document instead of an in-memory `static int` counter that restarted at 1 and silently overwrote existing Qdrant points across process restarts. `UpsertAsync` return type changed `int` → `Guid` (propagates through `/documents`' JSON response). Also removed the now-unused `Id` field from `VectorStoreService.QdrantPoint` — kept it typed to match new writes would have crashed deserializing the pre-existing int-id point from the earlier smoke test (caught live, not just in tests: `/ask` 500'd on the old Eiffel Tower point until `Id` was dropped from the record since it was never read after search anyway). Updated the 2 affected tests (`VectorStoreServiceTests`, `EndpointTests`) for the new `Guid` id shape. 17/17 tests still pass. Re-ran smoke test live: indexed a new document (real GUID assigned, e.g. `2dff62df-...`), then confirmed both the old int-id point and the new GUID-id point are independently retrievable via `/ask` with correct grounded answers. Pushed to `origin/feature/rag-minimal-api`.

- **2026-07-17 — Review → Engineering.** Reviewed `feature/rag-minimal-api` (TICKET-1). Build clean, 17/17 tests pass. Not clean: one issue written to `Review/Handoffs/Handoff-1.md` — `VectorStoreService`'s id counter is a non-durable in-memory static that restarts at 1 on every process start, so an upsert after restart can silently overwrite an already-indexed Qdrant point (real data-loss risk, not hypothetical — the smoke-tested document is already id `1`). Engineering should fix and re-hand-off. (Repo-root project placement was flagged then retracted — user confirmed it was deliberate, not a scaffolding miss.)

- **2026-07-17 — Engineering → Review.** Implemented TICKET-1 on branch `feature/rag-minimal-api` (branched from `master` after merging `LiveCoding`'s ASP.NET Core scaffold in). Added `Services/EmbeddingService.cs` (Ollama), `Services/VectorStoreService.cs` (Qdrant), `Services/AnswerService.cs` (Claude via `Anthropic` NuGet package, `HttpClient` injected for testability), `Services/DependencyUnavailableException.cs`, and mapped `POST /documents` / `POST /ask` in `Program.cs`. All five error-handling cases from the ticket are implemented (400 on empty text/question, 502 identifying Ollama/Qdrant/Claude on failure, 404 on zero Qdrant search results). Config: Ollama/Qdrant base URLs in `appsettings.json`; Anthropic API key in user secrets (set by user directly, not by Claude).
  - Unit tests: `ASPNetCoreApp.Tests` (sibling project, xUnit) — 17 tests total, all passing. Mocked-`HttpClient` tests for each service (request shape + all failure-mapping cases) plus `WebApplicationFactory`-based endpoint tests (happy path, 400, 404) with typed clients' primary handlers swapped for fakes.
  - Smoke test: ran the app for real against live local dependencies — Ollama (`nomic-embed-text` already pulled) and Qdrant (Docker container already running, `docs` collection present). `POST /documents` with a real sentence about the Eiffel Tower returned `{"id":1}`. `POST /ask` with "Where is the Eiffel Tower located?" returned a correct grounded answer from Claude. Empty-body 400s verified live for both endpoints. The 404 (empty search results) and 502 (dependency failure) paths were **not** exercised live — doing so would require emptying/breaking the shared local Qdrant collection — but both are covered by the automated endpoint/unit tests above.
  - Pushed `feature/rag-minimal-api` to `origin` (user-authorized, branch only — `master` not touched). Not merged; PR not opened.

- **2026-07-17 — Documentation → Engineering.** Wrote `Documentation/Tickets/TICKET-1-rag-minimal-api.md` from `Planning/Plan-rag-minimal-api.md`. Ticket covers both endpoints, all five error-handling cases, service structure, and required test coverage (unit + smoke). Engineering should implement per the ticket and plan.
- **2026-07-17 — Planning → Documentation.** Approved `Planning/Plan-rag-minimal-api.md`: minimal API (`POST /documents`, `POST /ask`) wrapping the RAG pipeline from the console prototype `rag-project` (Ollama embeddings → Qdrant vector search → Claude generation). Scope: error handling for unreachable/failing Ollama/Qdrant/Claude and empty search results; explicitly excludes chunking, doc list/delete, and multi-doc discrimination. Documentation should write the ticket from this plan.

## Open items

- Engineering to ask the user for explicit push authorization to `main` (per `git-workflow.md`) now that Review is clean.
- 404 and 502 paths are verified only via automated tests, not a live run — non-blocking, noted for awareness only.
