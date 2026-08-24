# OptimizedFlange

OptimizedFlange is an engineering application for checking, optimizing, and later automatically sizing bolted gasketed flange joints.

## V1 architecture baseline

The V1 architecture is frozen after Decisions 1–306. Structural changes after this baseline require an explicit architectural decision/ADR.

The implementation order is:

1. F# engineering core;
2. automated testing and validation campaign;
3. C# / WPF / MVVM desktop UI.

## Current implementation step

**Core Step 3 — calculation contracts and result traceability**

Implemented foundations:

- .NET 10 LTS single-target solution;
- F# `OptimizedFlange.Domain` project;
- F# `OptimizedFlange.Calculations` project;
- F# `OptimizedFlange.Configuration` project;
- F# `OptimizedFlange.Persistence` project;
- SI unit declarations;
- engineering-value provenance model;
- separate execution, assessment, and qualification states;
- strict separation between software settings, calculation defaults, and project calculation configuration;
- recent-file model with a maximum of 20 non-pinned entries;
- configurable technical database paths;
- persisted solver/calculation defaults;
- `System.Text.Json` persistence;
- atomic-style write/flush/validate/replace workflow;
- VS Code restore/build/test/run tasks;
- automated test campaign skeleton.
- calculation dispatcher and non-normative structural-validation engine.

The F# domain also contains the Step 2 technical skeleton for `FlangedJoint` and the Step 3 calculation/check contracts. No ASME/PCC/API engineering formula has been implemented yet. Normative calculations will be introduced only together with source traceability and their corresponding validation plan.

## Architecture

```text
src/
├── OptimizedFlange.Domain/
├── OptimizedFlange.Calculations/
├── OptimizedFlange.Configuration/
└── OptimizedFlange.Persistence/
```

Future modules will follow the frozen modular architecture documented under `doc/architecture/`.

## Build

```bash
dotnet restore OptimizedFlange.sln
dotnet build OptimizedFlange.sln
dotnet test OptimizedFlange.sln
```

The repository targets .NET 10 LTS.

## Core rule

Software settings, calculation defaults, and project engineering data are different data domains. Global configuration changes must never silently alter an existing engineering project.

## Documentation

See:

- `doc/architecture/core-bootstrap.md`
- `doc/architecture/core-domain-step2.md`
- `doc/architecture/core-calculation-contracts-step3.md`
- `doc/architecture/calculation-engine-skeleton-step4.md`
- `doc/persistence/settings-and-project-data.md`
- `doc/validation/test-campaign-skeleton.md`
- `AI.md`

## Core implementation status

### Step 2 — technical domain skeleton

The F# core now contains the first technical domain model for a two-sided bolted and gasketed joint. The model covers physical geometry and geometry states, signed load vectors, bolting assemblies, gasket assemblies and sealing zones, partition ribs, material snapshots, acceptance criteria, and the aggregate `FlangedJoint`.

No ASME VIII, PCC-1, API 660, IOGP S-614, TEMA, or EN 1591 equations have been implemented in this step. Normative calculations will be added only after the calculation contracts and test/validation structure are in place.

### Step 3 — calculation contracts and result traceability

The F# core now defines calculation procedure contracts, engineering-rule metadata, structured check results, result traces, governing-case references, calculation dependencies, and the calculation-engine function shape.

These types prepare the project for traceable calculations and validation evidence without adding normative equations. Future ASME/PCC/API/TEMA/EN rules must be implemented only with source, edition, clause/formula references, and corresponding tests.

### Test campaign skeleton

The solution now includes F# xUnit projects for unit tests, persistence tests, and validation-campaign placeholders. Current tests cover non-normative structural behavior and JSON persistence only; they do not qualify any normative engineering calculation.

### Step 4 — calculation engine skeleton

The F# calculation package now includes a dispatcher and an implemented non-normative structural-validation procedure. Normative procedure kinds intentionally return a stable not-implemented error until sourced rules and validation cases are added.

Codex users should read the repository-root `AGENTS.md` before editing. It points to the project memory and engineering standard and records the mandatory implementation sequence: Core first, testing second, WPF/MVVM UI last.

## Codex + VS Code

The repository includes a root `AGENTS.md` containing persistent instructions for Codex. Open the repository root in VS Code so Codex can discover these instructions together with `AI.md`, the architecture documents, and the registries. See `doc/codex-vscode.md` for the recommended handoff workflow.
