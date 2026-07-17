# Plan — RAG Minimal API

## Source

Adapted from console prototype at `C:\projects\Job Hunt\Skill-Up\rag-project` (`Program.cs`, `NOTES.md`). That app hardcodes one document and one question in a top-level-statements flow; this plan turns the same pipeline into a proper ASP.NET Core minimal API.

## Architecture (unchanged from prototype)

| Component | Role | Locality |
|---|---|---|
| Ollama (`nomic-embed-text`) | Text → 768-dim embedding vector | Local, `localhost:11434` |
| Qdrant | Vector storage + cosine-similarity nearest-neighbor search | Local, Docker container, `localhost:6333`, collection `docs` |
| Claude (`claude-sonnet-5`) | Generates the final answer from retrieved context | Anthropic API |

## Endpoints

- `POST /documents` — body: `{ text: string }`. Embeds the text via Ollama, upserts into Qdrant with an auto-incrementing/generated id. Returns the assigned id.
- `POST /ask` — body: `{ question: string }`. Embeds the question, retrieves the top match from Qdrant, builds the `Context:\n{context}\n\nQuestion: {question}\n\nAnswer using only the context above.` prompt, calls Claude, returns the answer text.

No document listing/deletion, no chunking, no multi-document discrimination logic — matches the "two endpoints" and "error handling only" scope decisions.

## Error handling (in scope)

- Ollama unreachable/non-2xx (`/api/embed`) → return `502 Bad Gateway` with a message identifying Ollama as the failed dependency, not a raw exception.
- Qdrant unreachable/non-2xx (upsert or search) → `502 Bad Gateway`, identifying Qdrant.
- `POST /ask` when Qdrant search returns zero results (no documents indexed yet) → `404 Not Found` with a message indicating no matching context exists, instead of passing `null`/empty context to Claude.
- Anthropic API failure → `502 Bad Gateway`, identifying Claude as the failed dependency.
- Empty/whitespace `text` or `question` in request bodies → `400 Bad Request`.

## Structure

- Minimal API, single project under `Engineering/` (per `coding-standards.md` — minimal API unless scope calls for MVC; two routes doesn't).
- `Services/EmbeddingService.cs` — wraps Ollama `/api/embed` call.
- `Services/VectorStoreService.cs` — wraps Qdrant upsert + search calls.
- `Services/AnswerService.cs` — builds prompt, calls Anthropic client.
- `Program.cs` — DI registration (typed `HttpClient` per service via `AddHttpClient`), endpoint mapping only.
- Config: Anthropic API key via user secrets (`UserSecretsId`, same pattern as prototype); Ollama/Qdrant base URLs via `appsettings.json` (defaulting to the localhost ports above) so they're overridable without code changes.

## Testing

- xUnit unit tests in sibling `*.Tests` project (per `coding-standards.md`):
  - `EmbeddingService`, `VectorStoreService`, `AnswerService` tested with mocked `HttpClient`/`HttpMessageHandler` — verify correct request shape and that dependency-failure responses map to the right exception/result type.
  - Endpoint tests (`POST /documents`, `POST /ask`) covering: happy path, empty body → 400, empty Qdrant search results → 404.
- Smoke test (Engineering's responsibility before handoff to Review): start the app, confirm Docker Qdrant container is running and Ollama is serving `nomic-embed-text` (`ollama list` / `ollama pull nomic-embed-text` if missing) before hitting real endpoints. If either dependency isn't running, start Qdrant via `docker run -p 6333:6333 qdrant/qdrant` (or existing compose setup, if any) and note actual observed behavior — not just a clean build — in the portfolio handoff.

## Not in scope (explicitly deferred, per user decision)

- Chunking of multi-paragraph documents.
- Document listing/deletion endpoints.
- Multi-document retrieval discrimination (top-1 match only, same as prototype).
