# AI Engineering Project Standard

**Version:** 1.0  
**Purpose:** reusable architectural and development standard for future engineering/scientific software projects.  
**Origin:** distilled from the architecture defined for OptimizedFlange, but intentionally written to be project-independent.

---

## 1. Core philosophy

Build engineering software as a set of small, explicit, testable modules.

The software must separate:

1. engineering/domain logic;
2. application orchestration;
3. persistence and external infrastructure;
4. user interface;
5. reporting;
6. validation and qualification;
7. release and reproducibility infrastructure.

Do not mix calculation formulas with UI, persistence, reporting, or external services.

Prefer pure functions and immutable data in the calculation core whenever practical.

Avoid large generic files such as `Calculations.*`, `Utils.*`, or `Models.*`. Split code by responsibility and engineering concept. A practical target is usually about 150–300 lines per focused source file, but cohesion is more important than a strict line limit.

---

## 2. Preferred technology stack

### Calculation core
- F#
- .NET 10 LTS
- Units of Measure for physical quantities where useful
- strong domain types
- discriminated unions for technical states and alternatives
- `Result`-style typed error handling for expected failures
- deterministic calculation paths for qualified calculations

### Desktop UI
- C#
- WPF
- MVVM
- CommunityToolkit.Mvvm
- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.Logging`

### Interoperability
Do not expose complex F# implementation types directly to WPF/C#.

Use stable DTO boundaries:

```text
WPF / C#
    ↓
Application DTO
    ↓
Mapping
    ↓
F# Domain/Core
    ↓
Calculation Result
    ↓
Result DTO
    ↓
WPF / C#
```

DTOs used for UI/API boundaries and DTOs used for persistence may be different.

---

## 3. Recommended solution structure

```text
src/
├── <Project>.Domain/
├── <Project>.Geometry/
├── <Project>.Loads/
├── <Project>.Materials/
├── <Project>.Calculations/
├── <Project>.Validation/
├── <Project>.Optimization/
├── <Project>.Sizing/
├── <Project>.Results/
├── <Project>.Reporting/
├── <Project>.Configuration/
├── <Project>.Application/
├── <Project>.Infrastructure/
└── <Project>.Desktop/

tests/
├── <Project>.UnitTests/
├── <Project>.ClauseTests/
├── <Project>.ReferenceTests/
├── <Project>.RegressionTests/
├── <Project>.IntegrationTests/
└── <Project>.Benchmarks/

doc/
├── architecture/
├── calculations/
├── validation/
├── standards/
├── decisions/
└── generated/

.vscode/
.github/
registry/
```

Not every future project needs every module. Preserve the boundaries even when some modules are omitted.

---

## 4. Domain and engineering rules

Engineering formulas belong only in calculation/domain modules.

Every important technical rule should have a stable identity.

Recommended metadata:

```text
EngineeringRule
├── RuleId
├── Name
├── Module
├── Source
├── Standard
├── Edition
├── Clause
├── FormulaReference
├── QualificationStatus
└── ValidationEvidence[]
```

Do not hide important engineering interpretations inside code comments.

Maintain a versioned interpretation registry for non-trivial interpretations of standards.

A technical check should return structured information, not only `true/false`:

```text
CheckResult
├── CheckId
├── Status
├── Severity
├── Actual
├── Limit
├── Utilization
├── GoverningCase
├── MessageCode
├── Standard
├── Edition
├── Clause
├── Inputs
└── IntermediateValues
```

---

## 5. Separation of software settings and calculation data

This distinction is mandatory:

```text
Software Settings
        ≠
Calculation Defaults
        ≠
Project Engineering Data
```

### Software settings
Describe how the application behaves.

Examples:
- language;
- theme;
- UI state;
- default folders;
- report/output folders;
- logs;
- autosave;
- recent files;
- database folders;
- external repository locations.

They must not change an existing engineering result.

### Calculation defaults
Describe defaults for creating a new technical project.

Examples:
- standards;
- solver defaults;
- optimization defaults;
- acceptance defaults;
- manufacturing defaults;
- material policies;
- reporting defaults.

When a project is created, copy the relevant defaults into the project.

Changing global defaults later must never silently alter an existing project.

### Project engineering data
Contain the actual values used for a calculation:
- geometry;
- loads;
- materials;
- calculation options;
- selected standards and editions;
- solver settings;
- acceptance criteria;
- optimization settings;
- source snapshots;
- audit information.

---

## 6. Configuration hierarchy

Recommended configuration module:

```text
<Project>.Configuration/
├── Application/
├── UserPreferences/
├── Calculation/
├── Optimization/
├── Standards/
├── Paths/
├── RecentFiles/
├── Validation/
└── Serialization/
```

Recommended application settings hierarchy:

```text
ApplicationSettings
├── General
├── Window
├── Files
├── Databases
├── ExternalRepositories
├── RecentFiles
├── Logging
└── Updates
```

Maintain at most 20 normal recent files.

Each recent-file entry should support:

```text
RecentFileEntry
├── Path
├── DisplayName
├── LastOpenedAt
├── LastSavedAt?
├── FileExists
├── SchemaVersion?
└── Pinned
```

Pinned items may be kept outside the normal automatic eviction policy.

---

## 7. Paths and databases

Never hard-code external database locations into calculation modules.

Use configurable database locations:

```text
DatabaseLocation
├── Id
├── Name
├── Path
├── Enabled
├── ReadOnly
├── Priority
├── LastAccessedAt
└── Fingerprint?
```

Typical configurable locations:
- materials;
- fasteners;
- threads;
- gaskets;
- tools;
- standards-derived datasets;
- manufacturer data;
- reference cases;
- project-specific databases;
- custom database folders.

External data consumed by an official result should be snapshot-able and fingerprinted.

---

## 8. Persistence

Use `System.Text.Json` as the default JSON serializer.

Persistence DTOs must be separated from F# domain types.

```text
Domain Model
    ↓ mapping
Persistence DTO
    ↓
System.Text.Json
    ↓
JSON file
```

Reading follows:

```text
JSON file
    ↓
Persistence DTO
    ↓
Migration
    ↓
Current DTO
    ↓
Validation
    ↓
Domain Model
```

All persisted technical formats must include a schema version.

Prefer explicit, testable migrations:

```text
Schema N → Schema N+1
```

Never perform an undocumented best-effort conversion of unknown future schemas.

---

## 9. Atomic saving

Important files must use atomic-style persistence:

```text
write temporary file
        ↓
flush
        ↓
validate
        ↓
atomic replace
        ↓
final file
```

Maintain backups according to a retention policy.

Use the same principle for:
- project files;
- results;
- settings;
- registries;
- snapshots;
- manifests;
- audit data.

Autosave must use a separate file and must never silently overwrite the official saved project.

---

## 10. Canonical data and fingerprints

Normal JSON serialization and fingerprint serialization are separate concerns.

For fingerprints use a canonical representation:
- deterministic field ordering;
- deterministic collection ordering where semantics allow it;
- invariant numeric representation;
- canonical SI values;
- no presentation rounding;
- SHA-256 by default.

A change in a dependency or input node should invalidate only the affected calculation nodes where possible.

Use an explicit dependency graph rather than broad global invalidation.

---

## 11. Units and numeric values

Use SI as the canonical internal system unless the engineering domain requires otherwise.

UI may support SI and US Customary.

Preserve both:
- canonical value;
- original user input;
- original unit;
- data source/provenance.

Recommended concept:

```text
EngineeringValue
├── EngineeringValueId
├── CanonicalValue
├── OriginalInput
├── Unit
├── Source
└── Provenance
```

Presentation rounding must never alter the value used in calculations.

Normative rounding rules, when required, must be explicit engineering rules.

---

## 12. Numerical input controls

For WPF numeric entry use a dedicated numeric-input component rather than plain text boxes when available.

The control is responsible for UI-level numeric entry/validation only.

It must not contain:
- engineering equations;
- normative checks;
- unit-system business logic that belongs to the application/domain layer.

For projects using GianFossi components, prefer the `NumericInput` NuGet package when compatible with the selected .NET target.

---

## 13. Calculation execution

Separate:
- calculation definition;
- execution state;
- engineering assessment;
- UI progress.

Recommended status families:

```text
ExecutionStatus
AssessmentStatus
QualificationStatus
Severity
```

Do not collapse them into one status.

Expected technical failures should be returned as typed errors instead of exceptions.

Exceptions remain appropriate for unexpected/programming/infrastructure failures.

Calculation cancellation should be cooperative.

Partial calculation output must never be mistaken for a final accepted assessment.

---

## 14. Solver configuration

Numerical solver settings must be explicit and persisted when they can affect results.

Examples:
- relative tolerance;
- absolute tolerances;
- maximum iterations;
- damping;
- subdivision limits;
- deterministic math mode.

Solver defaults are numerical implementation defaults, not normative engineering limits.

For validation and qualified release, provide a deterministic/strict math mode where necessary.

---

## 15. Optimization

The optimizer must not duplicate engineering formulas.

Architecture:

```text
candidate generator
      ↓
engineering Check engine
      ↓
feasibility
      ↓
objective/ranking
      ↓
final full recheck
```

Optimization should support:
- fixed variables;
- bounded continuous variables;
- bounded discrete variables;
- standard-series variables.

Manufacturing rounding is applied before the final full verification.

Do not accept an optimized geometry merely because an unrounded mathematical candidate passed.

---

## 16. Reporting

Reporting is independent from the calculation engine.

The report layer consumes a structured result model and does not recalculate engineering values.

Support at least:
- summary report;
- detailed report.

Detailed engineering reports should show, when appropriate:
- formula;
- symbols;
- substitutions;
- result;
- allowable/limit;
- utilization;
- pass/fail;
- governing case;
- source standard;
- edition;
- clause.

---

## 17. Testing strategy

Every project must include a testing campaign from the beginning.

Research public references and independent examples where legally and technically appropriate.

Testing layers:

1. **Unit tests** — pure mathematical/domain behavior.
2. **Clause/rule tests** — tests mapped to individual normative or engineering rules.
3. **Reference tests** — independent published/reference examples.
4. **Regression tests** — protect previously accepted results.
5. **Integration tests** — persistence, repositories, application boundaries, UI services, external providers.
6. **Benchmarks** — critical calculation and optimization workloads.

A reference case should include:

```text
ReferenceCase
├── Id
├── Description
├── Standard
├── Edition
├── Clause
├── Source
├── Inputs
├── ExpectedResults
├── Tolerances
└── Notes
```

Never mark a module Qualified only because unit tests pass.

---

## 18. Validation and qualification

Keep these concepts separate:

```text
Implemented
PartiallyImplemented
Validated
Qualified
Deprecated
```

Qualification must be granular by:
- module;
- standard;
- edition;
- engine/software version.

A new standard edition never automatically inherits qualification from a previous edition.

Validation evidence should be immutable and versioned.

---

## 19. Auditability

Important project changes must be traceable.

Audit records should be append-oriented and contain:
- who/what changed;
- timestamp;
- previous value;
- new value;
- reason where relevant;
- source/provenance.

For high-assurance projects, use a hash-chained audit trail.

Official releases should freeze the audit state and reference its final hash.

---

## 20. Messages and localization

The calculation core should emit stable `MessageCode` identifiers and typed parameters.

Human-readable text belongs in a message catalog/rendering layer.

Recommended fallback:

```text
requested language
    ↓
English
    ↓
generic MessageCode fallback
```

A missing required message in a qualified build should be detected by CI.

---

## 21. Registries

Use JSON as the authoritative machine-readable source for registries.

Generate Markdown documentation from those registries.

Suggested registry files:

```text
registry/
├── engineering-values.json
├── engineering-rules.json
├── features.json
├── normative-interpretations.json
├── qualification.json
├── standards-support.json
└── policies/
    ├── license-policy.json
    └── dependency-policy.json
```

Generated documentation:

```text
doc/generated/
├── engineering-values.md
├── engineering-rules.md
├── features.md
├── standards-support-matrix.md
└── normative-interpretations.md
```

CI must detect inconsistencies between registries, code, tests, and generated documentation.

---

## 22. Feature lifecycle

Separate:

```text
Feature implementation
Feature activation
Project technical option
Qualification
```

A feature flag means that a capability is available.

A technical option means the user selected an engineering configuration.

Feature flags must never silently select a normative interpretation.

Use a versioned Feature Registry and explicit feature dependencies.

---

## 23. Documentation

Every repository must contain a maintained `README.md`.

Always create a `doc/` folder with dedicated technical documentation.

Recommended topics:
- architecture;
- engineering model;
- standards/rules;
- persistence;
- validation;
- optimization;
- release process;
- troubleshooting;
- developer guide;
- user workflow;
- decisions/ADRs.

Documentation is part of the implementation, not a task postponed until the end.

---

## 24. AI project memory

Maintain an `AI.md` file in the repository.

It should record:
- project purpose;
- architecture;
- important decisions;
- current implementation status;
- pending tasks;
- constraints;
- conventions;
- known technical risks;
- external repositories/data sources;
- recent significant changes.

Update `AI.md` whenever an implementation or architectural change materially affects future work.

Do not use `AI.md` as a replacement for formal engineering documentation or source-code history.

---

## 25. VS Code

Prepare `.vscode/` configuration from project start.

At minimum provide commands/tasks for:
- restore;
- build;
- clean;
- run;
- debug;
- unit tests;
- complete test suite;
- validation tests;
- benchmarks when relevant;
- formatting;
- packaging;
- release validation.

Prefer reproducible command-line tasks rather than IDE-only operations.

---

## 26. Source control

Use Git from the first implementation commit.

Recommended flow:

```text
protected main
    ↑
pull request
    ↑
feature branch
```

Avoid direct implementation changes on `main`.

Commit logical changes in focused commits.

Use ADRs or equivalent records for architectural changes after the architecture baseline has been frozen.

---

## 27. Build profiles

Recommended profiles:

```text
Development
Test
Validation
Release
QualifiedRelease
```

Promotion should be sequential.

Example gates:

```text
Development → Test
    build + unit tests

Test → Validation
    clause + regression + reference tests

Validation → Release
    packaging + SBOM + security + license checks

Release → QualifiedRelease
    qualification evidence + strict deterministic calculation + release verification
```

A qualified release must not contain unqualified functionality inside its declared qualified scope.

---

## 28. Dependencies

Centralize NuGet package versions using `Directory.Packages.props`.

Rules:
- no floating package versions;
- no wildcard versions;
- prerelease dependencies only for development unless explicitly approved;
- record resolved dependency graph in release SBOM;
- do not require `packages.lock.json` unless a future project explicitly decides otherwise.

Use a central dependency policy and license policy.

---

## 29. Release engineering

Use Semantic Versioning.

Separate:
- software version;
- release channel;
- technical qualification.

Suggested release channels:

```text
Development
Preview
Validated
Qualified
Deprecated
```

Normative changes that may affect engineering results must be explicitly documented.

Breaking engineering changes require an appropriate version increment and revalidation.

---

## 30. Official release bundle

For high-assurance engineering software, an official release should be reproducible and auditable.

Recommended bundle:

```text
release/
├── manifest.json
├── project/example input when applicable
├── results/example output when applicable
├── report.pdf when applicable
├── qualification.json
├── validation-evidence/
├── external-data/
├── sbom.cdx.json
├── release-notes.json
├── RELEASE_NOTES.md
└── checksums.sha256
```

Use CycloneDX JSON for the SBOM.

Use SHA-256 checksums for artifacts.

Architect for future package/release signing even if signing is not mandatory in the first version.

---

## 31. Release manifest

Recommended machine-readable release manifest:

```text
ReleaseManifest
├── ReleaseId
├── Version
├── EngineVersion
├── SchemaVersion
├── StandardEditions[]
├── QualificationStatus
├── ProjectFingerprint
├── ResultsFingerprint
├── AuditChainFinalHash
├── ExternalDataSnapshots[]
├── ValidationEvidenceRefs[]
├── FeatureSnapshot
├── SecurityScan
├── LicensePolicyVersion
├── SbomFingerprint
├── ReleaseSignature?
└── CreatedAt
```

The manifest is the authoritative index of an official release.

---

## 32. Reproducibility

Official results should be reproducible from:
- project input;
- software version;
- calculation configuration;
- standard editions;
- material/external-data snapshots;
- dependency graph;
- feature state;
- numerical mode.

Verification status may be:

```text
Reproducible
ReproducibleWithWarnings
NotReproducible
NotVerifiable
```

---

## 33. Change impact

Classify changes:

```text
DocumentationOnly
UIOnly
NonNormativeCode
NumericalImplementation
NormativeLogic
DataSourceChange
```

Map each class to required tests and validation activity.

Changes to numerical implementation, normative logic, or source data must trigger stronger CI gates.

---

## 34. Architecture freeze

Before major implementation, define an architecture baseline.

After freeze:
- implementation details may evolve normally;
- architectural changes require an explicit ADR/decision;
- normative changes require impact analysis;
- schema changes require migration design;
- public/API breaking changes require versioning review.

The architecture baseline is not intended to prevent improvement. It exists to prevent accidental structural drift.

---

## 35. General AI instructions for future projects

When an AI assistant works on a project following this standard:

1. Read `README.md`, `AI.md`, architecture documentation, registries, and relevant ADRs before making structural changes.
2. Preserve module boundaries.
3. Do not put engineering calculations in UI code.
4. Do not invent normative formulas, clauses, limits, or standard requirements.
5. Research and verify technical references when implementing standards-based logic.
6. Add or update tests with every calculation change.
7. Update `README.md` when capabilities or usage change.
8. Update the relevant files under `doc/`.
9. Update `AI.md` with material changes and pending work.
10. Keep code modules small and cohesive.
11. Add XML documentation to public APIs and important code elements.
12. Maintain VS Code build/run/debug/test tasks.
13. Keep persistence schema-versioned and migration-safe.
14. Preserve provenance of engineering data.
15. Make changes ready for GitHub PR/CI.
16. Prepare libraries for NuGet publication where appropriate.
17. Never silently change an existing project's calculation because a global default changed.
18. Treat official engineering results as reproducible artifacts, not ephemeral UI output.
19. Prefer explicit states, typed data, and deterministic behavior over implicit conventions.
20. If an architectural decision conflicts with this standard, document the exception explicitly.

---

## 36. Minimum project bootstrap checklist

A new project based on this standard should start with:

```text
README.md
AI.md
CHANGELOG.md
Directory.Packages.props
global.json
.editorconfig

src/
tests/
doc/
registry/
.vscode/
.github/
```

Before the first functional release verify:
- build from clean checkout;
- all tests pass;
- persistence round-trip tests pass;
- migration tests pass;
- reports can be generated;
- external-data provenance is preserved;
- SBOM is generated;
- security/license gates run;
- release manifest is generated;
- checksums are generated;
- release can be reproduced.

---

## 37. Guiding principle

> A calculation result must be explainable, testable, reproducible, traceable to its inputs and rules, and insulated from unrelated changes in the UI or application environment.

This principle should govern future engineering projects built from this standard.
