# Workflow — Four-Tab Process

Adapted from the `claude-starter-kit` working-style pattern, modified for solo interview-prep work with no Jira/team involved. Four Claude Code sessions ("tabs"), each scoped to one folder and one job. `Development-Portfolio.md` (repo root) is the single shared handoff/status file — read it first in any tab to see what's in flight and what this tab is expected to pick up.

## The loop

1. **Planning** (`Planning/`) — Discuss what to build in the ASP.NET Core project. After approval, write the plan to `Planning/Plan-<slug>.md`. Update `Development-Portfolio.md`: mark the plan approved, hand off to Documentation. **STOP HERE.** Do not start implementing, do not write a ticket, do not touch `Engineering/`.

2. **Documentation** (`Documentation/`) — Pick up the approved plan from `Development-Portfolio.md`. Write a "Jira" ticket as markdown in `Documentation/Tickets/TICKET-<n>-<slug>.md`, using this exact template:

   ```markdown
   # TICKET-<n>: <short title>

   ## Summary
   <1-3 sentences — what this ticket delivers>

   ## Acceptance Criteria
   - [ ] <criterion>
   - [ ] <criterion>

   ## Linked Plan
   `Planning/Plan-<slug>.md`
   ```

   Update `Development-Portfolio.md`: hand off to Engineering. **STOP HERE.** Do not implement, do not touch `Engineering/`.

3. **Engineering** (`Engineering/`) — Pick up the plan + ticket. Implement it, including unit tests. Before handing off, smoke test the running app (start it, hit the real endpoints, confirm actual behavior — not just a clean build). Once build, unit tests, and smoke tests all pass: commit and push the current branch to origin (per `git-workflow.md` — this step is pre-authorized, no need to ask). Then update `Development-Portfolio.md`: note what was implemented, unit/smoke test results, confirm the push, hand off to Review. **STOP HERE.** Do not review your own diff, do not write `Review/Handoffs/` entries, do not sign off your own work.

4. **Review** (`Review/`) — Deep review of the Engineering diff for bugs and coding-standard violations (see `coding-standards.md`). This is a **read-only diff review** — do not run `dotnet build`, `dotnet test`, or `dotnet run`; do not re-verify behavior by executing anything. Engineering already built, tested, and smoke tested before handoff — re-running that is Engineering's job, not Review's. Read the diff and the standards doc, form a judgment from that alone. If clean: update `Development-Portfolio.md` to reflect sign-off, hand back to Engineering to request push permission. If issues found: write `Review/Handoffs/Handoff-<n>.md` listing every issue, update `Development-Portfolio.md` to route back to Engineering for another iteration — loop to step 3. **STOP HERE.** Do not fix issues yourself, do not touch `Engineering/`.

5. **Push to main** — Once Review signs off clean, Engineering asks the user directly for explicit permission to push to `main`/`master` (per `git-workflow.md` — this is never assumed, every push to `main`/`master` is its own authorization, regardless of how many branch pushes preceded it). After that push, update `Development-Portfolio.md` to record it.

## Rules

- Each tab reads and updates `Development-Portfolio.md` — that's the only place state is tracked between tabs. Don't rely on chat history from another tab's session; it isn't visible here.
- Stay in your tab's lane: Planning doesn't write code, Engineering doesn't invent scope beyond the ticket, Review doesn't fix things itself — it documents findings and hands back.
- Iteration count on the Review loop isn't capped — repeat until clean.
- **Always update the `Development-Portfolio.md` handoff log after pushing a commit (branch or `main`/`master`) — never before.** The handoff entry should describe what actually landed on origin, not what's only staged locally.
