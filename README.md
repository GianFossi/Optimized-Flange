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
- `registry/standards-support.json`
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

The F# calculation package now includes a dispatcher and an implemented non-normative structural-validation procedure. Normative procedure kinds intentionally return a stable not-implemented error until sourced rules and validation cases are added.

### Standards and qualification registries

The repository now includes machine-readable registries for planned standards support, engineering rule placeholders, qualification status, and normative interpretations. These registries deliberately mark ASME VIII, PCC-1, API 660, IOGP S-614, TEMA, and EN 1591 support as planned only. No normative formula is implemented, validated, or qualified yet.

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
