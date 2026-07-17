# Handoff 1 — TICKET-1 Review

Branch: `feature/rag-minimal-api` (`c2f4a4d`, `2d63544`)

Build: clean. Tests: 17/17 passing (`dotnet test` from `ASPNetCoreApp.Tests/`).

## Issues

### 1. Non-durable, colliding document IDs (`Services/VectorStoreService.cs:7,11`)

```csharp
private static int _nextId;
...
var id = Interlocked.Increment(ref _nextId);
```

`_nextId` is an in-memory static counter starting at 0 on every process start. It has no relationship to what's already stored in Qdrant. After a restart, the next `POST /documents` call is assigned id `1` again — `PUT /collections/docs/points` with an existing id **overwrites** the point already there (Qdrant upsert semantics), silently destroying previously indexed data. This isn't a hypothetical: the smoke test in the portfolio handoff already indexed a document as id `1`; the very next server restart + document post will clobber it.

Fix: generate a value guaranteed unique across restarts — a GUID (Qdrant point ids accept UUIDs), or read the current max id from Qdrant before assigning. A GUID is the simpler fix and doesn't require an extra round-trip.

## Not flagged (reviewed, no issue)

- Project location at repo root instead of `Engineering/` — user confirmed this was a deliberate placement, not a scaffolding oversight. No `.sln` exists yet; not treating that as a defect given the deliberate layout.
- Error-mapping (400/404/502) in `Program.cs`, `EmbeddingService`, `AnswerService` — matches ticket acceptance criteria exactly, verified against tests.
- `AnswerService` Anthropic key/client wiring — correct, testable via injected `HttpClient`.
- Qdrant payload/search shape — consistent with prototype's API version.
