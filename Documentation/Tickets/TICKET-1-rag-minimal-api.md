# TICKET-1 — RAG Minimal API

## Summary

Build a minimal API wrapping the RAG pipeline from the console prototype at `C:\projects\Job Hunt\Skill-Up\rag-project`: Ollama embeddings → Qdrant vector search → Claude generation. Two endpoints — `POST /documents` (index a document) and `POST /ask` (answer a question from indexed context) — with error handling for unreachable/failing dependencies and empty search results.

Plan: `../../Planning/Plan-rag-minimal-api.md`

## Acceptance Criteria

- `POST /documents` accepts `{ text: string }`, embeds via Ollama (`nomic-embed-text`), upserts into Qdrant collection `docs`, returns the assigned id.
- `POST /ask` accepts `{ question: string }`, embeds the question, retrieves the top Qdrant match, builds the `Context:\n{context}\n\nQuestion: {question}\n\nAnswer using only the context above.` prompt, calls Claude (`claude-sonnet-5`), returns the answer text.
- Ollama unreachable/non-2xx → `502 Bad Gateway`, message identifies Ollama.
- Qdrant unreachable/non-2xx (upsert or search) → `502 Bad Gateway`, message identifies Qdrant.
- `POST /ask` with zero Qdrant search results → `404 Not Found`, message indicates no matching context — no null/empty context passed to Claude.
- Anthropic API failure → `502 Bad Gateway`, message identifies Claude.
- Empty/whitespace `text` or `question` in request body → `400 Bad Request`.
- No document listing/deletion, no chunking, no multi-document discrimination (top-1 match only) — out of scope per plan.
- Structure: `Services/EmbeddingService.cs`, `Services/VectorStoreService.cs`, `Services/AnswerService.cs`, `Program.cs` for DI + endpoint mapping only. Anthropic key via user secrets; Ollama/Qdrant base URLs via `appsettings.json`.
- xUnit unit tests: mocked-`HttpClient` tests for each service (request shape + failure mapping), endpoint tests for happy path, 400, and 404 cases.
- Smoke test performed and results (not just a clean build) recorded in the portfolio handoff — including verifying/starting Qdrant (Docker) and Ollama (`nomic-embed-text` pulled) beforehand.
