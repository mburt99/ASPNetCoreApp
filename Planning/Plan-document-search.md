# Plan: Document Search (Live Coding Interview Ticket)

## Context

This is the live-coding interview task, worked on branch `LiveCoding`. The ask: a web page with a search box that hits a backend API, searches a small in-project JSON knowledge base, and returns the best-matching entry plus its source. No database, no external services, .NET 8/ASP.NET Core, minimal API preferred, ~20-30 min live build.

Current repo state: on `LiveCoding` branch, `ASPNetCoreApp/` has only the default weatherforecast minimal-API stub; `ASPNetCoreApp.Tests/` folder exists but is empty (no `.csproj` yet).

## Approach

**Domain:** Product FAQ, 7 sample entries, JSON file in-project.

**Scoring:** Simple tokenize + weighted term-overlap (title matches worth 2x content matches). No ML/embeddings — keeps it explainable live and satisfies "keep it simple."

### Structure (all under existing `ASPNetCoreApp/` scaffold)
```
ASPNetCoreApp/
  Program.cs                      — rewritten: DI, /api/search endpoint, static files (drop weatherforecast stub)
  Data/documents.json             — 7-entry FAQ knowledge base
  Models/Document.cs              — record Document(Id, Title, Source, Content)
  Models/SearchModels.cs          — record SearchRequest(Query); record SearchResult(Title, Source, Content, Score)
  Services/IDocumentSearchService.cs / DocumentSearchService.cs
  wwwroot/index.html              — single-file frontend: input, button, fetch(), result div
  ASPNetCoreApp.csproj            — exclude Tests subfolder from globs; ensure Data/*.json copies to output
ASPNetCoreApp.Tests/
  ASPNetCoreApp.Tests.csproj      — new, xUnit, references main project
  DocumentSearchServiceTests.cs
ASPNetCoreApp.slnx                — new, at ASPNetCoreApp/ level
```

### Key design decisions
- `DocumentSearchService` takes `List<Document>` directly in its constructor (pure, testable); a tiny loader reads/deserializes `Data/documents.json` once in `Program.cs` and the built list is registered as a singleton via DI — avoids file-path plumbing in unit tests.
- Scoring: lowercase + split query/title/content into token sets, count query terms present in title (×2 weight) + content (×1), highest score wins; score 0 → no match (endpoint returns 404).
- Endpoint: `POST /api/search` with `SearchRequest { Query }` → `SearchResult { Title, Source, Content, Score }` or 404.
- Frontend: plain HTML/JS in `wwwroot/`, served via `UseDefaultFiles`/`UseStaticFiles` — no build step, no framework.
- Async signatures (`FindBestMatchAsync`) to match coding standards even though the work is in-memory/synchronous under the hood.

### Test plan
`DocumentSearchServiceTests` (xUnit, naming `MethodName_Scenario_ExpectedBehavior`):
- title-term match returns expected doc
- no-overlap query returns null
- title match outweighs content-only match
- empty query returns null
Integration test via `WebApplicationFactory<Program>` is optional/stretch given the time box.

### Live build order (checkpointed so there's always a working increment)
1. Strip weatherforecast stub; add `Models/Document.cs`, `Models/SearchModels.cs`.
2. Add `Data/documents.json` (paste pre-written 7 entries).
3. Implement `DocumentSearchService` scoring logic.
4. Wire DI + `POST /api/search` in `Program.cs`; verify via `.http` file/curl — **first working increment**.
5. Add `wwwroot/index.html`; verify full flow in browser — **safe stopping point if time is short**.
6. Add `ASPNetCoreApp.Tests` project + scoring unit tests; `dotnet test`.
7. Add `.slnx`, smoke-test end-to-end, polish edge cases if time remains.

## Verification
- `dotnet build` succeeds; `dotnet test` passes the four scoring unit tests.
- `dotnet run`, then in browser hit `/`, submit a few queries (e.g. "how do I reset my password", "cancel my plan", gibberish query) and confirm correct title/source returned, and a graceful "no match" for the gibberish case.
- Confirm `ASPNetCoreApp.Tests/**` is excluded from the main project's compiled/content items (per coding standards) and that `.slnx` builds both projects.
