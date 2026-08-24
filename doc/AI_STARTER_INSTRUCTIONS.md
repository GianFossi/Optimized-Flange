# AI Starter Instructions — Engineering Software Projects

Load and follow `AI_ENGINEERING_PROJECT_STANDARD.md` as the architectural baseline for this project.

## Mandatory working rules

- Prefer F# for the engineering/calculation core.
- Use C# + WPF + MVVM for desktop UI unless the project explicitly decides otherwise.
- Keep calculation logic out of ViewModels, Views, persistence, and reporting.
- Use stable DTO boundaries between F# and C#.
- Target the current selected .NET LTS; for the originating baseline this is .NET 10 LTS.
- Use small cohesive modules rather than large generic files.
- Add XML documentation to public APIs and important code elements.
- Maintain a complete testing campaign: unit, rule/clause, reference, regression, integration, and benchmarks where useful.
- Research and verify external engineering references when implementing standards-based rules.
- Never invent normative formulas, clauses, limits, or interpretations.
- Always maintain `README.md`.
- Always maintain a dedicated `doc/` tree.
- Always maintain `AI.md` with project decisions, implementation state, pending work, and important changes.
- Prepare `.vscode` tasks for restore/build/run/debug/test/validation/package.
- Use Git feature branches and pull requests; keep `main` protected when repository capabilities allow it.
- Prepare packages/releases for NuGet and GitHub where appropriate.
- Separate Software Settings, Calculation Defaults, and Project Engineering Data.
- Persist structured data as schema-versioned JSON using `System.Text.Json` unless there is a documented reason not to.
- Use atomic save, backups, explicit migrations, provenance, canonical fingerprints, and reproducible release artifacts.
- Maintain recent files and configurable database folders in application settings rather than hard-coded paths.
- Changing application/global defaults must never silently modify an existing engineering project.
- After architecture freeze, structural changes require an explicit ADR/decision.

Before changing architecture, first inspect the existing repository documentation and decision history.
