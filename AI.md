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

## 2026-08-24 — Core Step 2

- Added the technical domain skeleton for `FlangedJoint`.
- Added focused domain files for geometry, loads, acceptance criteria, bolting, gaskets, materials, and joint composition.
- Added `AGENTS.md` so Codex/IDE sessions inherit the project architecture and working rules.
- Preserved signed load components `Fx,Fy,Fz,Mx,My,Mz` and side-specific pressure/temperature conditions.
- Kept physical geometry separate from future ASME code-effective geometry.
- Kept gasket assembly/sealing zones/partition ribs explicit.
- Kept multiple bolt area definitions explicit and rule-selectable.
- Added material snapshots designed for data resolved by the external Materials provider.
- Added only non-normative structural validation; no ASME/PCC/API equations are implemented yet.
- Next: calculation/check contracts and result traceability, then testing campaign before UI.

## 2026-08-24 — Core Step 3

- Added calculation/check contracts without implementing normative formulas.
- Added focused domain files for engineering rule metadata, result trace quantities/dependencies, structured check results, and calculation request/engine contracts.
- Added rule source metadata fields for source kind, name, edition, clause, and formula reference so future rules can be tied to standards and validation evidence.
- Added `CalculationResult` and `CheckResult` structures with execution state, assessment state, qualification state, severity, governing cases, stable message codes, comparisons, and trace data.
- Kept human-readable messages, reporting, persistence contracts, and UI out of the calculation core.
- Updated architecture documentation and feature registry for Step 3.
- No ASME VIII, PCC-1, API 660, IOGP S-614, TEMA, or EN 1591 equations are implemented.
- Next: create the testing and validation campaign skeleton before adding normative calculation engines.
