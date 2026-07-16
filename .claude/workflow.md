# Workflow — Four-Tab Process

Adapted from the `claude-starter-kit` working-style pattern, modified for solo interview-prep work with no Jira/team involved. Four Claude Code sessions ("tabs"), each scoped to one folder and one job. `Development-Portfolio.md` (repo root) is the single shared handoff/status file — read it first in any tab to see what's in flight and what this tab is expected to pick up.

## The loop

1. **Planning** (`Planning/`) — Discuss what to build in the ASP.NET Core project. After approval, write the plan to `Planning/Plan-<slug>.md`. Update `Development-Portfolio.md`: mark the plan approved, hand off to Documentation.

2. **Documentation** (`Documentation/`) — Pick up the approved plan from `Development-Portfolio.md`. Write a "Jira" ticket as markdown in `Documentation/Tickets/TICKET-<n>-<slug>.md` (field-order doesn't matter here — just: summary, acceptance criteria, linked plan file). Update `Development-Portfolio.md`: hand off to Engineering.

3. **Engineering** (`Engineering/`) — Pick up the plan + ticket. Implement it, including unit tests. Before handing off, smoke test the running app (start it, hit the real endpoints, confirm actual behavior — not just a clean build). Update `Development-Portfolio.md`: note what was implemented, unit/smoke test results, hand off to Review.

4. **Review** (`Review/`) — Deep review of the Engineering diff for bugs and coding-standard violations (see `coding-standards.md`). If clean: update `Development-Portfolio.md` to reflect sign-off, hand back to Engineering to request push permission. If issues found: write `Review/Handoffs/Handoff-<n>.md` listing every issue, update `Development-Portfolio.md` to route back to Engineering for another iteration — loop to step 3.

5. **Push** — Once Review signs off clean, Engineering asks the user directly for explicit permission to push to `main` (per `git-workflow.md` — this is never assumed, every push is its own authorization).

## Rules

- Each tab reads and updates `Development-Portfolio.md` — that's the only place state is tracked between tabs. Don't rely on chat history from another tab's session; it isn't visible here.
- Stay in your tab's lane: Planning doesn't write code, Engineering doesn't invent scope beyond the ticket, Review doesn't fix things itself — it documents findings and hands back.
- Iteration count on the Review loop isn't capped — repeat until clean.
