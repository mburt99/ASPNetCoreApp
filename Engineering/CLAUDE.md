# Engineering tab

@../.claude/interaction-style.md
@../.claude/git-workflow.md
@../.claude/coding-standards.md
@../.claude/workflow.md

Read `../Development-Portfolio.md` first.

Your job: pick up the plan (`../Planning/`) and ticket (`../Documentation/Tickets/`). Implement it here, including unit tests, per `../.claude/coding-standards.md`. Before handing off, actually run the app and smoke test it — real requests against real endpoints, not just a clean build. Update `../Development-Portfolio.md` with what was implemented and the test results, then hand off to Review.

If `../Review/Handoffs/` has an open item routed back to you, that's the next thing to fix — not new scope.

Once Review signs off clean, ask the user directly for explicit permission before pushing to `main`. Never assume it.
