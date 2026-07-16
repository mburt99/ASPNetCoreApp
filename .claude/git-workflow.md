# Git Workflow — C:\Working

## Repo

GitHub: `ASPNetCoreApp` (covers the whole `C:\Working` tree — Planning/Documentation/Engineering/Review, not just code).

## Commit Messages

- Single-line message only. No body, no bullet points, no description.
- **No Co-Authored-By lines — ever.**
- Do NOT use HEREDOC format. Use: `git commit -m "short message"`
- One logical change per commit.

## Branching

- Feature branches off main: `feature/short-name`.
- **Never push to main without explicit direction from user** — each push authorization is scoped to that specific action, prior permission doesn't carry forward.
- Engineering asks for explicit permission to push only after Review has signed off clean (see `workflow.md`).

## Pull Requests

Not used for this solo interview-prep workflow — direct commits to main once Review is clean and push is authorized.
