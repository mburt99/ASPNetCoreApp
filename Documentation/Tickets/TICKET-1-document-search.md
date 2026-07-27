# TICKET-1 — Implement Document Search

**Status:** Closed — Review signed off clean on `LiveCoding` at `889f034`. All acceptance criteria below met; Engineering to request push permission to `main`/`master` per `git-workflow.md`.

## Summary

Build a web page with a search box backed by an ASP.NET Core minimal API. A user types a question, submits, and gets back the single most relevant entry from a small in-project Product FAQ knowledge base, along with its source/title. No database, no external services — dataset is a JSON file shipped with the project.

## Description

A single `POST /api/search` endpoint scores a 7-entry Product FAQ dataset (`ASPNetCoreApp/Data/documents.json`) against the submitted query using tokenized, weighted term-overlap (title matches weighted 2x content matches — no ML/embeddings). The highest-scoring document is returned as `{ title, source, content }`; a score of zero across all documents returns 404. `wwwroot/index.html` provides a plain HTML/JS frontend (input, submit, result div) served via `UseDefaultFiles`/`UseStaticFiles` — no build step, no framework.

Plan: `../../Planning/Plan-document-search.md`

## Story

As a developer evaluating this codebase, I want to type a question into a search box and get back the most relevant FAQ entry with its source, so that I can demonstrate a working search feature without a database or external service dependency.

## Risk Statement

The dataset is a static in-project JSON file loaded once at startup into DI as a singleton, so there is no runtime file I/O or external dependency risk. Because scoring is exact-token overlap rather than semantic matching, near-synonym queries (e.g. "cancel" vs "canceling") can fail to match or pick a less-intuitive best match — an accepted, called-out limitation of the no-ML design, not a defect. A zero-score result must return a clean 404 rather than a null-content 200, so the frontend always has a well-defined "no match" state to render.

## Acceptance Criteria

- `POST /api/search` accepts `{ query: string }`, scores all documents in `Data/documents.json`, and returns the highest-scoring `{ title, source, content }`.
- Score of zero across all documents → `404 Not Found` (no match).
- Empty/whitespace query → `400 Bad Request`.
- `wwwroot/index.html` presents a text input and a submit action, and renders the returned title, source, and content after search.
- No database or external service calls; dataset lives entirely in `ASPNetCoreApp/Data/documents.json`.
- Backend is .NET 8 / ASP.NET Core, minimal API style, constructor DI, async method signatures per `.claude/coding-standards.md`.
- Structure: `Models/Document.cs`, `Models/SearchModels.cs`, `Services/IDocumentSearchService.cs` / `DocumentSearchService.cs`, `Program.cs` for DI + endpoint mapping and static file serving only.
- Unit tests cover the relevance-scoring logic (`ASPNetCoreApp.Tests`), following `MethodName_Scenario_ExpectedBehavior` naming: title-term match, no-overlap query, title match outweighing content-only match, empty query.
- Smoke test performed and results recorded in the portfolio handoff — running app hit for `/` and `POST /api/search` with real queries, a gibberish query (404), and an empty query (400), not just a clean build.
