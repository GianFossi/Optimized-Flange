# OptimizedFlange

OptimizedFlange is an engineering application for checking, optimizing, and later automatically sizing bolted gasketed flange joints.

## V1 architecture baseline

The V1 architecture is frozen after Decisions 1–306. Structural changes after this baseline require an explicit architectural decision/ADR.

The implementation order is:

1. F# engineering core;
2. automated testing and validation campaign;
3. C# / WPF / MVVM desktop UI.

## Current implementation step

**Core Bootstrap — Step 1**

Implemented foundations:

- .NET 10 LTS single-target solution;
- F# `OptimizedFlange.Domain` project;
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
- VS Code restore/build/test tasks.

No ASME/PCC/API engineering formula has been implemented yet. Normative calculations will be introduced only together with source traceability and their corresponding validation plan.

## Architecture

```text
src/
├── OptimizedFlange.Domain/
├── OptimizedFlange.Configuration/
└── OptimizedFlange.Persistence/
```

Future modules will follow the frozen modular architecture documented under `doc/architecture/`.

## Build

```bash
dotnet restore OptimizedFlange.sln
dotnet build OptimizedFlange.sln
```

The repository targets .NET 10 LTS.

## Core rule

Software settings, calculation defaults, and project engineering data are different data domains. Global configuration changes must never silently alter an existing engineering project.

## Documentation

See:

- `doc/architecture/core-bootstrap.md`
- `doc/persistence/settings-and-project-data.md`
- `AI.md`
