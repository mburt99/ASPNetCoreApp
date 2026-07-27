# Postmortem — Role Boundary Violations (Document Search cycle)

**Date:** 2026-07-27
**Scope:** First full pass through the four-tab loop (Planning → Documentation → Engineering → Review) for the Document Search feature.

## What went wrong

1. **Planning implemented code.** After the plan was approved, Planning kept going and wrote the implementation instead of stopping at the plan. It never updated `Development-Portfolio.md` or created a handoff for Documentation/Engineering to pick up.
2. **Engineering skipped the Review handoff and reviewed its own work.** After coding, adding tests, building, and smoke testing, Engineering moved straight into reviewing the diff itself instead of stopping, updating the portfolio, and waiting for the Review tab.
3. **Review re-ran unit tests and smoke tests.** That's Engineering's responsibility (build/test/run) and already covered before handoff. Review's job is a static diff read against `coding-standards.md`, not re-execution.
4. **Documentation used an inconsistent ticket format.** `workflow.md` described the ticket's required contents in prose ("summary, acceptance criteria, linked plan file") but gave no concrete template, so the actual file didn't follow a fixed shape.

## Root cause

Each tab's `CLAUDE.md` stated what the tab should produce, but not a hard stop instruction to quit once that output was produced. Without an explicit "you are done, do not proceed" signal, momentum carried each tab into the next tab's job. Documentation's ticket format failure was a separate issue: no literal template existed to copy, only a description.

## Fixes applied

- `C:\Working\.claude\workflow.md` — added an explicit **STOP** line to each of the four loop steps, and corrected Review's step to state it does not execute anything (no `dotnet test`, no `dotnet run`) — diff-and-standards read only.
- `C:\Working\.claude\workflow.md` — added a literal ticket template under Documentation's step so the file shape isn't reinvented per ticket.
- `C:\Working\.claude\interaction-style.md` — added a "Role boundaries are hard stops" section: once your tab's deliverable is written and the portfolio is updated, stop responding with more work — even if the next step is obvious or already known.
