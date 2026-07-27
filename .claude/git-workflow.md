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
- **Pushing the current branch to origin is pre-authorized** once a build, unit tests, and smoke tests all succeed — commit and push without asking (see `workflow.md` step 3).
- **Never push to `main`/`master` without explicit direction from user** — each push authorization is scoped to that specific action, prior permission doesn't carry forward, and no amount of prior branch pushes implies it.
- Engineering asks for explicit permission to push to `main`/`master` only after Review has signed off clean (see `workflow.md`).

## Pull Requests

Not used for this solo interview-prep workflow — direct commits to main once Review is clean and push is authorized.
