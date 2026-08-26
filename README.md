# OptimizedFlange

OptimizedFlange is an engineering application for checking, optimizing, and later automatically sizing bolted gasketed flange joints.

## V1 architecture baseline

The V1 architecture is frozen after Decisions 1–306. Structural changes after this baseline require an explicit architectural decision/ADR.

The implementation order is:

1. F# engineering core;
2. automated testing and validation campaign;
3. C# / WPF / MVVM desktop UI.

## Current implementation step

**Project persistence hardening**

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
- schema-versioned project file envelope;
- explicit project-file migration boundary;
- explicit technical-data payload migration boundary;
- VS Code restore/build/test/run tasks;
- VS Code validation/package tasks;
- central NuGet package metadata with bundled README and license;
- NuGet publishing helper script and release documentation;
- standards, engineering rule, qualification, and normative-interpretation registries;
- pre-implementation formula inventory gate;
- reference workbook guide registry for non-authoritative calculation workbooks;
- candidate source-to-domain symbol mapping registry;
- reference workbook comparison cases for FAST R09;
- planned normative calculation procedure catalog;
- automated test campaign skeleton;
- calculation dispatcher and non-normative structural-validation engine;
- local technical database source registry for materials, gaskets, bolting, facings, standard flanges, ratings, pipes, and tube data.

The F# domain also contains the Step 2 technical skeleton for `FlangedJoint` and the Step 3 calculation/check contracts. A small number of traceable ASME VIII-2 and IOGP S-614 SI-only helper formulas are implemented as `PartiallyImplemented`; they are not validated, qualified, or full end-to-end procedures yet.

## Architecture

```text
src/
├── OptimizedFlange.Domain/
├── OptimizedFlange.Calculations/
├── OptimizedFlange.Configuration/
├── OptimizedFlange.DataSources/
└── OptimizedFlange.Persistence/

examples/
└── OptimizedFlange.TextDemo/
```

Future modules will follow the frozen modular architecture documented under `doc/architecture/`.

## Build

```bash
dotnet restore OptimizedFlange.sln
dotnet build OptimizedFlange.sln
dotnet test OptimizedFlange.sln
```

The repository targets .NET 10 LTS.

## License

OptimizedFlange is licensed under the PolyForm Noncommercial License 1.0.0. See `LICENSE` and `registry/policies/license-policy.json`.

## Credits

See `CREDITS.md` for project credits, technology acknowledgements, and notes about future standards references.

## Core rule

Software settings, calculation defaults, and project engineering data are different data domains. Global configuration changes must never silently alter an existing engineering project.

## Documentation

See:

- `doc/architecture/core-bootstrap.md`
- `doc/architecture/core-domain-step2.md`
- `doc/architecture/core-calculation-contracts-step3.md`
- `doc/architecture/calculation-engine-skeleton-step4.md`
- `doc/persistence/settings-and-project-data.md`
- `doc/persistence/project-file-envelope.md`
- `doc/release/nuget-publishing.md`
- `doc/validation/test-campaign-skeleton.md`
- `doc/calculations/normative-formula-intake.md`
- `registry/standards-support.json`
- `registry/formula-inventory.json`
- `registry/reference-guides.json`
- `registry/symbol-map.json`
- `registry/workbook-comparison-cases.json`
- `registry/validation-cases.json`
- `registry/database-sources.json`
- `registry/engineering-rules.json`
- `registry/qualification.json`
- `registry/normative-interpretations.json`
- `registry/policies/license-policy.json`
- `CREDITS.md`
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

The F# calculation package now includes a dispatcher, an implemented non-normative structural-validation procedure, unqualified SI-only ASME VIII Division 2 Part 4.16 basic bolt-load helper formulas, one unqualified IOGP S-614 paragraph 7.8.10 Equation (3) pure formula, and a catalog of planned normative procedures for ASME VIII Division 1, ASME VIII Division 2, ASME PCC-1 Appendix O, API 660 paragraph 7.8, and IOGP S-614 amendments to API 660 paragraph 7.8.

Normative procedure kinds still intentionally return a stable not-implemented error until complete sourced rules, formula mappings, integration logic, and validation cases are added. The implemented ASME and IOGP helpers are available as focused SI-only formulas and remain `PartiallyImplemented`, not qualified.

The dispatcher now routes the partially implemented ASME VIII Division 2 and IOGP S-614 procedure contracts to incomplete traceable results instead of generic not-implemented errors. These endpoints report the resolved inputs still needed before producing numeric procedure results.

### Standards and qualification registries

The repository now includes machine-readable registries for planned standards support, engineering rule placeholders, qualification status, and normative interpretations. These registries deliberately mark ASME VIII, PCC-1, API 660, IOGP S-614, TEMA, and EN 1591 support as planned only. No normative formula is implemented, validated, or qualified yet.

The selected source scope for the next formula-inventory phase is ASME VIII Division 1 2025, ASME VIII Division 2 2025, ASME PCC-1 Appendix O 2022, API 660 2015 paragraph 7.8, and IOGP S-614 v18-12 amendments to API 660 paragraph 7.8. See `doc/calculations/normative-formula-intake.md`.

`registry/formula-inventory.json` records the selected source documents and keeps each formula group at `NeedsManualClauseInventory` until exact clauses, formula/table references, SI symbol mappings, applicability limits, and validation cases are available.

`registry/reference-guides.json` records calculation workbooks that can guide symbol mapping and comparison-case discovery. `doc/Calcs/Flange Design - FAST - R09.xlsm` is registered as a macro-enabled reference guide, not as a normative source. Its formulas and macros must not be copied or executed as qualification evidence; every implemented formula still needs an approved standard, edition, clause/formula reference, and validation case.

The FAST R09 guide registry records worksheet metadata and defined-name clusters for bolting, gasket, flange, load, and minimum-distance concepts. These clusters are intended for later source-backed formula mapping only.

`registry/symbol-map.json` records the first candidate mapping from FAST R09 defined names to domain paths and SI canonical units. Entries remain candidates until the corresponding source standard clause/formula, sign convention, and validation case are confirmed.

`registry/normative-implementation-blockers.json` records why the remaining normative formula families are still blocked from implementation or qualification.

`registry/workbook-comparison-cases.json` records FAST R09 cached-value comparison cases. These tests compare implemented helpers against workbook values without executing macros and are not qualification evidence.

`registry/validation-cases.json` records the independent expected-result cases still needed before any partially implemented formula can be promoted to `Validated` or `Qualified`.

### Local Technical Databases

`registry/database-sources.json` records the current local database root:

```text
C:\Users\ganfossi\Documents\DataBase\data
```

The registry includes `MyLib.json`, gasket geometry and parameters, bolting, facings, standard flanges, ASME B16 ratings, pipes, and tube BWG data. `NozzleLoads` is recorded as a future/missing category until a source file is available.

The built-in `Defaults.databasePaths` value is intentionally portable and contains no workstation path. When the root folder is loaded from user settings or another defaults source, use `Defaults.databasePathsFromRootFolder` to create read-only `DatabasePathSettings`.

`OptimizedFlange.DataSources` can now load configured local XML/JSON data files into searchable imported records. Each imported record keeps source id, source path, category, source units, converted SI scalar values when units are recognized, and provenance. Search filters currently cover category, source id, free text, family/type, standard/specification, grade/class, and scalar presence.

Procedure-aware data resolution is available through `ProcedureDataResolver`: callers pass a `CalculationProcedureDefinition`, and the resolver returns candidate imported records for that procedure's expected data categories. Calculation modules still do not open database files directly.

The data-source layer also includes initial domain mapping helpers:

- material records to ambient `MaterialSnapshot` values;
- bolting records to `BoltingAssembly` values when project bolt count and bolt-circle diameter are supplied;
- ring-gasket records to `GasketAssembly` values using imported dimensions;
- standard-flange records from `Flanges.xml` to `JointSideGeometry` using `RingOD`, `RingWT`, `BoltCircDiam`, `HubSmallDiam`, `HubLargeDiam`, `HubLength`, and RF facing dimensions.

The ASME material SQLite databases are loaded through `Microsoft.Data.Sqlite` in read-only mode. The loader supports both the normalized `asme_sec2_partd_metric.sqlite3` schema and the PascalCase `asme_materials*.db` working schema.

`JointSelectionBuilder` composes a calculation-ready `FlangedJoint` from explicit selected flange, gasket, gasket parameter, bolting, and material records plus project-only inputs such as bolt count, bolt-circle diameter, pressure, and temperature. This provides a real data-selection-to-dispatcher path for structural validation and for the currently implemented ASME VIII-2 / IOGP helper outputs.

The ASME VIII-2 and IOGP dispatcher endpoints now resolve available inputs from the composed `FlangedJoint`: gasket reaction diameter, effective physical gasket width, selected gasket `m/y`, load-case pressure, gasket area, selected bolt root area, bolt count, and selected material allowable stress at the load-case temperature when that value is present in the `MaterialSnapshot`. These results remain `PartiallyImplemented` because they are helper-level outputs, not complete qualified code procedures.

### Text Demo

Run the no-UI assembly demo with:

```bash
dotnet run --project examples/OptimizedFlange.TextDemo/OptimizedFlange.TextDemo.fsproj --configuration Debug
```

The demo loads the local database root, selects a flange, ring gasket, gasket `m/y` parameter record, bolting record, and material record, builds a `FlangedJoint`, and runs structural, ASME VIII-2 helper, and IOGP S-614 helper dispatcher procedures. VS Code also has a `run-text-demo` task.

### VS Code tasks

The `run` task currently executes the core Debug test suite because the project has no UI or executable host yet. Dedicated tasks are available for restore, build, release build, test, validation tests, and package preparation.

### Packaging

NuGet packages include the repository README, credits, and PolyForm Noncommercial 1.0.0 license file through central package metadata in `Directory.Build.props`.

Use `scripts/publish-nuget.ps1` for a guarded NuGet publishing flow. It performs a dry run by default and requires `-Publish` plus a NuGet API key to push packages. See `doc/release/nuget-publishing.md` for where to retrieve the NuGet.org API key and source URL.

The repository also includes `.github/workflows/publish.yml` for NuGet.org Trusted Publishing through GitHub Actions.

### NuGet Trusted Publishing

NuGet.org Trusted Publishing publishes from GitHub Actions without storing a long-lived NuGet API key in the repository.

Create the Trusted Publishing policy on NuGet.org with these values:

```text
Package owner: Ganfoss
Publisher: GitHubActions
Repository Owner: GianFossi
Repository: OptimizedFlange
Workflow File: publish.yml
Environment: leave empty
Glob Patterns and Packages: OptimizedFlange.*
```

`Workflow File` must be only `publish.yml`, not `.github/workflows/publish.yml`.

The workflow login user must be the NuGet.org username that created the Trusted Publishing policy, not necessarily the package owner. The workflow reads it from this GitHub repository secret:

```text
NUGET_USER
```

Create it in GitHub under `Settings` -> `Secrets and variables` -> `Actions` -> `New repository secret`.

Set the value to the exact NuGet.org username of the account that created the Trusted Publishing policy. To find it, open `View Profile` from the NuGet.org account menu and copy the last part of the profile URL:

```text
Name: NUGET_USER
Value: value-from-https://www.nuget.org/profiles/<username>
```

This is not a NuGet API key. It is only the NuGet.org username used by `NuGet/login@v1` to find the Trusted Publishing policy. If GitHub Actions fails with `No matching trust policy owned by user 'Ganfoss' was found`, then `Ganfoss` is the package owner/display name but not the policy creator username. Update the `NUGET_USER` secret to the exact profile username and re-run the workflow.

`Glob Patterns and Packages` controls which package IDs the policy may publish. `OptimizedFlange.*` covers:

```text
OptimizedFlange.Domain
OptimizedFlange.Calculations
OptimizedFlange.Configuration
OptimizedFlange.Persistence
```

If NuGet.org shows `Use within 7 day(s) to keep it permanently active`, the policy is provisional. Run the GitHub workflow once within that window:

1. Push `.github/workflows/publish.yml` to GitHub.
2. Open the GitHub repository.
3. Select `Actions`.
4. Select `Publish NuGet`.
5. Select `Run workflow`.

A successful trusted publish activates the policy permanently.

### Project file envelope

Persistence now includes a versioned project file envelope for metadata and project-owned calculation configuration. The explicit technical-data DTO fragment currently covers project acceptance criteria, physical load cases, joint-side geometries, bolting assemblies, gasket assemblies, material snapshots, and reference-based flanged-joint composition.

The project file store can now embed and extract the versioned technical-data payload from the `.ofj` envelope. Project-file and technical-data schema versions both pass through explicit migration boundaries before being accepted. Technical-data mapping also rejects missing technical collections, missing side references, blank identifiers, blank scalar/array references, duplicate fragment identifiers, and duplicate joint reference IDs before resolving reference-based joints.

Codex users should read the repository-root `AGENTS.md` before editing. It points to the project memory and engineering standard and records the mandatory implementation sequence: Core first, testing second, WPF/MVVM UI last.

## Codex + VS Code

The repository includes a root `AGENTS.md` containing persistent instructions for Codex. Open the repository root in VS Code so Codex can discover these instructions together with `AI.md`, the architecture documents, and the registries. See `doc/codex-vscode.md` for the recommended handoff workflow.
