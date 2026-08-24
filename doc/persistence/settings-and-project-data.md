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

## Future work

Before `.ofp` is production-ready, persistence DTOs and explicit schema migrations will be introduced separately from the domain model.

## DTO boundary

F# unions and options are not persisted directly. `OptimizedFlange.Persistence` maps configuration/domain-oriented records to simple JSON DTOs with stable string identifiers and nullable fields. This deliberately decouples the long-lived file contract from internal F# representation.
