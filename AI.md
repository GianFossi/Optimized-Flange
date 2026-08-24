# AI Project Memory — OptimizedFlange

## Baseline

Architecture Decisions 1–306 are the frozen V1 baseline. Any structural change requires an explicit ADR/decision.

## Technology

- Core: F#.
- Target: .NET 10 LTS, single target.
- Future desktop UI: C# + WPF + MVVM + CommunityToolkit.Mvvm.
- Future DI: Microsoft.Extensions.DependencyInjection.
- Persistence: System.Text.Json.
- Numerical UI input: GianFossi NumericInput NuGet package when UI implementation begins.

## Implementation sequence

1. Core.
2. Testing and validation campaign.
3. UI.

## Current state

Core Bootstrap Step 1 created:

- Domain fundamentals;
- configuration hierarchy;
- calculation defaults copied into project-owned configuration;
- recent files (20 non-pinned maximum);
- database paths;
- JSON persistence;
- atomic-style file replacement;
- VS Code task skeleton.

No engineering/normative flange formulas are implemented in this step.

## Important rules

- No engineering equations in UI or persistence layers.
- Do not serialize F# domain internals directly as long-term project contracts; use persistence-specific DTO/mapping as the model grows.
- Every normative rule requires source/edition/clause metadata and tests.
- README.md, doc/, AI.md, .vscode and test campaign must evolve with implementation.
- Do not write directly to protected/main workflow when a feature branch can be used.

## GitHub status

Repository: GianFossi/OptimizedFlange.

Attempt to create `feature/core-bootstrap` through the available GitHub integration returned HTTP 403 `Resource not accessible by integration`. The prepared bootstrap must therefore be pushed from a branch through a normal Git client or after connector permissions allow branch creation.
