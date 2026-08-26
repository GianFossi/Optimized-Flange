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
- Project license: PolyForm Noncommercial License 1.0.0.

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

## 2026-08-24 — Test Campaign Skeleton

- Added F# xUnit test projects for unit tests, persistence tests, and validation-campaign placeholders.
- Added central NuGet package versions for xUnit, Microsoft.NET.Test.Sdk, xUnit runner, and coverlet collector.
- Added tests for recent-file capping/pinning behavior and case-insensitive refresh.
- Added tests for non-normative `FlangedJoint.validateStructure` behavior.
- Added calculation-default JSON persistence round-trip coverage.
- Added placeholder validation tests tagged for future clause, reference, and regression campaigns.
- Added `doc/validation/test-campaign-skeleton.md`.
- No normative engineering formulas or qualification claims were added.

## 2026-08-24 — Core Step 4

- Added `OptimizedFlange.Calculations` as the first calculation-engine package.
- Added `ProcedureCatalog` with an implemented non-normative structural-validation procedure.
- Added `StructuralValidationEngine.run`, which wraps `FlangedJoint.validateStructure` into the calculation result contract.
- Added `CalculationDispatcher.run`, which routes structural validation and returns `CALCULATION.PROCEDURE.NOT_IMPLEMENTED` for future normative procedure kinds.
- Added `OptimizedFlange.CalculationTests` for dispatcher behavior.
- Fixed `Defaults.calculation` with an explicit `CalculationDefaults` return type to avoid ambiguous record inference.
- Fixed `AtomicFile.writeText` so the temporary file handle is closed before replacement.
- No ASME VIII, PCC-1, API 660, IOGP S-614, TEMA, EN 1591, or project-policy formulas are implemented.

## 2026-08-24 — Project File Envelope

- Added `ProjectFileDto`, `ProjectMetadataDto`, and `ProjectCalculationConfigurationDto`.
- Added explicit mappers between `ProjectCalculationConfiguration` and its persistence DTO.
- Added `ProjectFileStore` with schema-version checking.
- Added persistence tests for project file envelope round-trip and project-owned calculation-configuration mapping.
- Added `doc/persistence/project-file-envelope.md`.
- Full `FlangedJoint` technical-data persistence remains pending and must use explicit DTOs/migrations.

## 2026-08-24 — Technical Data DTO Fragment

- Added `ProjectTechnicalDataDto` as the first explicit technical project data DTO.
- Added `AcceptanceCriterionDto` and mappers for requirement level/source identifiers and optional limits.
- Added `JointLoadCaseDto`, `ComponentConditionDto`, and `JointLoadVectorDto` for physical load case persistence.
- Added `JointSideGeometryDto`, `NominalSideGeometryDto`, `PhysicalHubGeometryDto`, and `SurfaceAllowanceDto` for physical geometry persistence.
- Added `BoltingAssemblyDto`, `BoltPatternDto`, `StudDefinitionDto`, `BoltSectionAreasDto`, and `PreloadDefinitionDto` for physical bolting persistence.
- Added `GasketAssemblyDto`, `GasketEnvelopeDto`, `SealingZoneDto`, `PartitionLayoutDto`, and `PartitionRibDto` for physical gasket persistence.
- Added `ComponentMaterialDto`, `MaterialSnapshotDto`, `MaterialIdentityDto`, and `ResolvedMaterialPropertiesDto` for material snapshot persistence.
- Added `JointSideReferenceDto` and `FlangedJointDto` for reference-based two-sided joint composition.
- Added tests for acceptance-criterion DTO round-trip, signed load-case DTO round-trip, joint-side geometry DTO round-trip, bolting assembly DTO round-trip, gasket assembly DTO round-trip, material snapshot DTO round-trip, flanged-joint DTO resolution, and technical-data collection mapping.
- Added a direct persistence reference to `OptimizedFlange.Domain` for explicit technical DTO mapping.
- Full `FlangedJoint` persistence is now represented by explicit reference DTOs; future work should add schema migrations and project-file payload integration.

## 2026-08-24 — Project Technical Payload Integration

- Added `ProjectFileStore.CurrentTechnicalDataSchemaVersion`.
- Added `ProjectFileStore.withTechnicalData` to embed `ProjectTechnicalDataDto` into the `.ofj` envelope.
- Added `ProjectFileStore.technicalData` to extract and validate technical-data payloads.
- Added tests for save/load/extract of a project file with technical-data payload.
- Missing, empty, unsupported, and schema-mismatched payloads are rejected with explicit errors.

## 2026-08-24 — License

- Switched the repository license to PolyForm Noncommercial License 1.0.0.
- Added root `LICENSE` with required project notice.
- Added `registry/policies/license-policy.json`.
- Updated `README.md` and this project memory with the active license.

## 2026-08-24 — Project File Migrations

- Added `ProjectFileMigrations.migrateToCurrent` as the explicit `.ofj` envelope migration boundary.
- Schema version 1 is currently accepted as-is.
- Legacy and future project-file schema versions are rejected until documented migrations exist.
- Added persistence tests for current, legacy, and future project-file schema handling.

## 2026-08-25 — VS Code Task Maintenance

- Updated the `run` task to execute the core Debug verification path while no executable UI/host exists.
- Added dedicated `test-validation` and `package` tasks.
- Documented the current task semantics in `README.md` and `doc/codex-vscode.md`.

## 2026-08-26 — Package Metadata

- Added central NuGet package metadata in `Directory.Build.props`.
- Configured packages to include the repository `README.md` and `LICENSE`.
- Kept the active package license file aligned with PolyForm Noncommercial License 1.0.0.

## 2026-08-26 — NuGet Publishing Help

- Added `doc/release/nuget-publishing.md` with manual NuGet.org publishing instructions.
- Added `scripts/publish-nuget.ps1` for restore/build/test/pack and optional publish.
- The publishing script is dry-run by default and requires `-Publish` plus a NuGet API key before pushing packages.
- Added a VS Code `publish-nuget-dry-run` task.
- Documented where to retrieve the NuGet.org API key and source URL, plus how to pass a custom/private feed source.
- Added `.github/workflows/publish.yml` for manual GitHub Actions publishing through NuGet.org Trusted Publishing.
- Expanded `README.md` with Trusted Publishing policy values, glob pattern guidance, and the 7-day activation workflow.
- Updated Trusted Publishing documentation and workflow to use the NuGet.org policy creator username for `NuGet/login@v1`.
- Changed the Trusted Publishing workflow to read the NuGet.org policy creator username from the `NUGET_USER` GitHub Actions secret.
- Clarified Trusted Publishing troubleshooting: `NUGET_USER` must match the NuGet.org profile username from `https://www.nuget.org/profiles/<username>`, not necessarily the package owner/display name.
- Updated the NuGet publishing workflow to Node 24-compatible `actions/checkout@v5` and `actions/setup-dotnet@v6`.
