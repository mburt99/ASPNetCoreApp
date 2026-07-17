# TICKET-1: Document Search

**Status:** Done — Review signed off clean (2026-07-17). Engineering to request push permission.

## Summary

Build a web page with a search box backed by an ASP.NET Core minimal API. A user types a question, submits, and gets back the single most relevant entry from a small in-project Product FAQ knowledge base, along with its source/title. No database, no external services — dataset is a JSON file shipped with the project.

## Acceptance Criteria

- [x] A web page (`wwwroot/index.html`) presents a text input and a submit action.
- [x] Submitting calls `POST /api/search` with the query text.
- [x] The API scores a small JSON dataset of FAQ documents (title, source, content) against the query and returns the single best match (title, source, content) or a "no match" result if nothing scores above zero.
- [x] The result — title, source, and content — is rendered on the page after search.
- [x] No database or external service calls; dataset lives in `ASPNetCoreApp/Data/documents.json`.
- [x] Backend is .NET 8 / ASP.NET Core, minimal API style, constructor DI, async method signatures per `.claude/coding-standards.md`.
- [x] Unit tests cover the relevance-scoring logic (`ASPNetCoreApp.Tests`), following `MethodName_Scenario_ExpectedBehavior` naming.
- [x] App is smoke-tested end-to-end (running app, real browser/HTTP request) before handoff to Review, not just a clean build.

Verified independently by Review (`dotnet test`: 4/4 pass; live `dotnet run` hit for `/` and `POST /api/search` — see `Development-Portfolio.md` 2026-07-17 Review → Engineering entry for full detail). Known, called-out limitation: exact-token scoring can miss near-synonym stems (e.g. "cancel" vs "canceling") — not a bug, per plan.

## Linked Plan

`Planning/Plan-document-search.md`
