# Project File Envelope

## Purpose

The project file envelope establishes the first schema-versioned `.ofj` persistence boundary.

It stores metadata and project-owned calculation configuration while reserving an explicit slot for future technical project data DTOs.

## Current contents

```text
ProjectFileDto
├── SchemaVersion
├── Metadata
├── CalculationConfiguration
├── TechnicalDataSchemaVersion?
└── TechnicalDataJson?
```

The first explicit technical-data DTO fragment is:

```text
ProjectTechnicalDataDto
├── SchemaVersion
└── AcceptanceCriteria[]
```

## Boundary

The envelope does not serialize F# domain internals such as `FlangedJoint` directly.

Future geometry, load, bolting, gasket, material, and full `FlangedJoint` data must be added through explicit persistence DTOs and migrations before full project round-trip persistence is considered implemented.
