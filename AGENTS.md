# OptimizedFlange — Codex Working Instructions

## Required context before editing

Read these files before making architectural or engineering changes:

1. `AI.md`
2. `README.md`
3. `doc/AI_ENGINEERING_PROJECT_STANDARD.md`
4. `doc/AI_STARTER_INSTRUCTIONS.md`
5. relevant files under `doc/architecture/`
6. relevant registries under `registry/`

## Architecture baseline

The V1 architecture is frozen at Decision 306. Structural changes require an explicit ADR/decision and an update to `AI.md`.

The implementation sequence is:

1. F# Core and domain.
2. Test campaign and validation.
3. C# WPF/MVVM UI last.

## Mandatory engineering rules

- Keep engineering equations and normative checks out of WPF, ViewModels, persistence, and reporting.
- Prefer pure, deterministic F# functions in the calculation core.
- Use small cohesive files/modules; avoid generic `Calculations.fs`, `Utils.fs`, and `Models.fs` files.
- Public types, functions, members, and important code elements require XML documentation comments.
- Do not invent ASME/API/PCC/TEMA/IOGP/EN formulas, clauses, acceptance limits, or interpretations.
- When implementing a normative rule, identify source standard, edition, clause/formula reference, and validation evidence.
- SI is canonical internally; presentation units are boundary concerns.
- Preserve provenance for engineering data.
- Keep Software Settings, Calculation Defaults, and Project Engineering Data separate.
- Persistence uses schema-versioned DTOs, `System.Text.Json`, explicit migrations, and atomic saving.

## Mandatory project maintenance

For every material implementation change:

- update `README.md` when capabilities or workflow change;
- update relevant files under `doc/`;
- update `AI.md` with status, decisions, and pending work;
- add/update tests once the testing phase is active;
- keep `.vscode` tasks usable;
- prepare changes as a focused Git commit/PR;
- keep NuGet/GitHub release compatibility in mind.

## Current Step 2 scope

Implement the technical domain skeleton only. Do not add ASME calculation equations yet.

The domain must model:

- flange/joint geometry and geometry states;
- joint load cases and signed force/moment components;
- bolting assemblies and bolt patterns;
- gasket assemblies, sealing zones, and partition ribs;
- material references/snapshots;
- two-sided `FlangedJoint` composition;
- acceptance criteria and technical options required by the frozen architecture.

The UI is intentionally out of scope.
