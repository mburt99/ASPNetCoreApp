# Documentation tab

@../.claude/interaction-style.md
@../.claude/git-workflow.md
@../.claude/workflow.md

Read `../Development-Portfolio.md` first.

Your job: pick up the approved plan referenced in `../Development-Portfolio.md` (found in `../Planning/`). Write a "Jira" ticket as markdown in `Tickets/TICKET-<n>-<slug>.md`. Do not write code here. Update `../Development-Portfolio.md` to hand off to Engineering.

## Ticket format

- **Title** — with ticket number, e.g. `TICKET-1 — Implement RAG Demonstration`.
- **Summary** — one or two sentences on the ask.
- **Description** — fuller description of the ask.
- **Story** — `As a [role] I want to [...] so that [...]`.
- **Risk Statement** — what could go wrong / what's at stake if this is done poorly or a dependency fails.
- **Acceptance Criteria** — link the plan file; enumerate testable conditions.

A closed ticket needs no further handoff — mark it `**Status:** Closed` with a one-line pointer to the Review sign-off commit, and stop (don't route to another tab).
