# TICKET-1 — Implement RAG Demonstration

**Status:** Closed — Review signed off clean on `feature/rag-minimal-api` at `77b142a`. All acceptance criteria below met; note in ticket-level scope, `id` field is a `Guid` per the fix in Handoff-1 (not a raw int).

## Summary

Build a minimal API wrapping the RAG pipeline from the console prototype `rag-project`: Ollama embeddings → Qdrant vector search → Claude generation.

## Description

Two endpoints — `POST /documents` (index a document) and `POST /ask` (answer a question from indexed context) — replacing the prototype's hardcoded one-document/one-question console flow with a proper ASP.NET Core minimal API. `POST /documents` embeds submitted text via Ollama (`nomic-embed-text`) and upserts it into Qdrant. `POST /ask` embeds a question, retrieves the top Qdrant match, and calls Claude (`claude-sonnet-5`) to generate an answer grounded in that context. Includes error handling for unreachable/failing Ollama, Qdrant, or Claude, and for the case where no documents are indexed yet.

Plan: `../../Planning/Plan-rag-minimal-api.md`

## Story

As a developer evaluating this codebase, I want to index text documents and ask questions against them over HTTP, so that I can demonstrate a working RAG pipeline without relying on the console prototype's hardcoded input.

## Risk Statement

Ollama and Qdrant are local dependencies not guaranteed to be running; if either is down, indexing or querying fails, so failures must be surfaced as clear `502` responses rather than unhandled exceptions. Anthropic API failures carry the same risk. Because Qdrant persists across process restarts, an id-assignment scheme must not collide with or silently overwrite previously indexed documents.

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
