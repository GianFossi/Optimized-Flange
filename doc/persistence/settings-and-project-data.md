# Settings and Project Data Persistence

## Separation

```text
Software Settings
        !=
Calculation Defaults
        !=
Project Engineering Data
```

Software settings describe application behavior. Calculation defaults are templates for new projects. Project engineering data contains the actual technical configuration used by a calculation.

## Persistence

JSON is the default structured persistence format and is serialized with `System.Text.Json`.

Important writes use this flow:

```text
write temporary file
→ flush to disk
→ validate JSON
→ replace destination
→ retain previous backup
```

## Recent files

The application retains up to 20 non-pinned recent projects. Pinned projects are preserved independently of normal eviction.

## Database paths

External data locations are configurable. Calculation modules must never hard-code material, gasket, thread, bolting, tool, or validation database paths.

The built-in database-path default is portable and empty. A local installation should resolve the root folder from user settings, a defaults file, or a registry/configuration source, then call `Defaults.databasePathsFromRootFolder`.

For the current workstation, `registry/database-sources.json` records:

```text
C:\Users\ganfossi\Documents\DataBase\data
```

That folder currently includes `MyLib.json`, gasket databases, bolting, facings, standard flanges, ASME B16 ratings, pipes, and tube BWG data. These files are read-only technical data sources. Imported values must retain provenance and pass unit conversion at the application or infrastructure boundary before reaching calculation modules.

`OptimizedFlange.DataSources` is the first infrastructure boundary for these local files. It loads configured XML/JSON files into searchable `ImportedDataRecord` values with provenance and recognized SI scalar conversions. Search filters are deterministic and cover category, source id, free text, family/type, standard/specification, grade/class, and scalar presence.

Procedure data resolution is intentionally one step before calculation execution: a caller can use `ProcedureDataResolver` with a `CalculationProcedureDefinition` to obtain candidate records for the procedure, then explicitly map selected records into project engineering data. This avoids hidden database reads inside formula modules.

Initial mapping helpers convert selected imported records into domain objects only when required source fields are present. `Flanges.xml` records are mapped using the source fields `RingOD`, `RingWT`, `BoltCircDiam`, `HubSmallDiam`, `HubLargeDiam`, `HubLength`, and RF facing dimensions. Missing dimensions or properties return explicit errors; no calculation defaults or geometry values are inferred from a standard name alone.

`JointSelectionBuilder` composes a `FlangedJoint` from explicit selected records and explicit project inputs. It creates the first selection-to-dispatcher path for structural validation without introducing hidden global state or database reads inside calculation modules.

Selected gasket `m/y` parameters are now carried by `GasketAssembly` and its DTO as optional project technical data. Dispatcher endpoints may use those values when they are present, while preserving incomplete diagnostics when they are absent.

## Future work

Before `.ofp` is production-ready, persistence DTOs and explicit schema migrations will be introduced separately from the domain model.

## DTO boundary

F# unions and options are not persisted directly. `OptimizedFlange.Persistence` maps configuration/domain-oriented records to simple JSON DTOs with stable string identifiers and nullable fields. This deliberately decouples the long-lived file contract from internal F# representation.
