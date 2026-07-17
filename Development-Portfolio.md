# Development Portfolio — ASPNetCoreApp

The single shared handoff/status file across all four tabs. Overwrite — never append. Whichever tab you're in, read this first.

## Current state

**Stage:** Review signed off clean — Engineering to request push permission
**Owner:** Engineering

## Handoff log

_(newest first — one entry per handoff between tabs)_

- **2026-07-17 — Review → Engineering:** Reviewed the diff against `.claude/coding-standards.md` (Program.cs, ASPNetCoreApp.csproj, Models/, Services/, Data/documents.json, wwwroot/index.html, ASPNetCoreApp.Tests/, ASPNetCoreApp.slnx). No bugs or standard violations found. Verified independently (not just re-reading Engineering's claims): `dotnet test` → 4/4 pass; ran the app (`dotnet run`) and hit it live — `GET /` returns the search page (200), `POST /api/search` returns 200 with correct title/source/content for "how do I reset my password" and "cancel my plan" (camelCase JSON keys confirmed matching `wwwroot/index.html`'s `data.title/source/content` usage — minimal API's default camelCase serialization makes this line up correctly), 404 for a gibberish query, 400 for empty and whitespace-only query. Confirmed `Data/documents.json` copies to `bin/Debug/net8.0/Data/` on build (Web SDK default Content behavior, no explicit csproj entry needed). Design choices reviewed and accepted as documented: synchronous logic wrapped in `Task.FromResult` to satisfy async-signature convention (plan explicitly calls this out); exact-token scoring missing "cancel"/"canceling" stem match is a known, called-out limitation, not a bug. Clean — Engineering may request push permission from the user.
- **2026-07-17 — Engineering → Review:** Implemented Document Search per `Planning/Plan-document-search.md` and `Documentation/Tickets/TICKET-1-document-search.md`: `Models/Document.cs`, `Models/SearchModels.cs`, `Services/DocumentLoader.cs`, `Services/DocumentSearchService.cs` + `IDocumentSearchService`, `Data/documents.json` (7-entry FAQ), `Program.cs` rewired for DI + `POST /api/search`, `wwwroot/index.html` frontend. Added `ASPNetCoreApp.Tests` (xUnit) with 4 scoring tests (`DocumentSearchServiceTests.cs`) — all pass (`dotnet test`: 4/4). Added `ASPNetCoreApp.slnx` referencing both projects; `dotnet build` on the slnx succeeds with 0 warnings/errors. Smoke tested a freshly built `dotnet run` instance: `/` returns 200, `POST /api/search` returns correct title/source for real queries (password reset, two-factor auth), 404 for no-match query, 400 for empty query. Known limitation: exact-token scoring means near-synonyms (e.g. "cancel" vs "canceling") can pick a less-intuitive best match — inherent to the no-ML design called out in the plan, not a bug. Review to check the diff against `.claude/coding-standards.md`.
- **2026-07-17 — Documentation → Engineering:** Wrote `Documentation/Tickets/TICKET-1-document-search.md` (summary, acceptance criteria, link to `Planning/Plan-document-search.md`). Engineering to implement per the plan's structure/build order, add unit tests, smoke test the running app, and hand off to Review.
- **2026-07-17 — Planning → Documentation:** Plan approved for Document Search interview ticket (search box → backend API → best-match result from small in-project JSON knowledge base). Plan written to `Planning/Plan-document-search.md`. Documentation to write `Documentation/Tickets/TICKET-1-document-search.md` (summary, acceptance criteria, link to plan) and hand off to Engineering.

## Open items

_(none yet)_
