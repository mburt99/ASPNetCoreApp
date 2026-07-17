# Coding Standards — .NET 8 / ASP.NET Core

Starter conventions — extend as the actual interview task's specifics emerge (this file is a living doc, not a final spec).

## General

- Nullable reference types enabled, implicit usings enabled.
- `async`/`await` all the way down — no `.Result`/`.Wait()` blocking calls.
- Async methods end in `Async`.
- Prefer minimal API endpoints unless the task's scope clearly calls for full MVC controllers (more routes, shared filters/conventions).
- Dependency injection via constructor injection; no service locator patterns.
- Records for immutable DTOs/response shapes; classes for entities with identity/mutable state.

## Testing

- xUnit for unit tests.
- Test naming: `MethodName_Scenario_ExpectedBehavior`.
- Unit tests live in a `*.Tests` project nested under the main project folder (e.g. `ASPNetCoreApp/ASPNetCoreApp.Tests/`), referencing the main project.
- Smoke test the running app after implementation (see `workflow.md` — Engineering's responsibility before handoff to Review).

## Structure

- One `.sln`/`.slnx` inside the main project folder (e.g. `ASPNetCoreApp/ASPNetCoreApp.slnx`), sibling to the app's own `.csproj` and the `*.Tests` folder — not at the repo root or under `Engineering/`.
- Main project's `.csproj` must exclude the `*.Tests` subfolder (`Compile`/`Content`/`EmbeddedResource`/`None` `Remove`) since the SDK-style default glob would otherwise pull test files into the main project's compile items.
- Standard ASP.NET Core folder layout (`Controllers/` or `Endpoints/`, `Models/`, `Services/`) — exact shape depends on the task; don't over-scaffold before requirements are known.
