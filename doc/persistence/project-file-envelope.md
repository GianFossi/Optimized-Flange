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

`ProjectFileStore.withTechnicalData` embeds a `ProjectTechnicalDataDto` payload in the envelope and records its schema version.

`ProjectFileStore.technicalData` extracts the payload and rejects missing, empty, unsupported, or mismatched technical-data schema versions.

`ProjectFileMigrations.migrateToCurrent` is the explicit migration boundary for `.ofj` envelopes. Schema version 1 is currently accepted as-is. Legacy and future schema versions are rejected until a documented migration exists.

`ProjectTechnicalDataMigrations.migrateToCurrent` is the explicit migration boundary for embedded technical-data payloads. Technical-data schema version 1 is currently accepted as-is. Legacy and future schema versions are rejected until a documented migration exists.

The first explicit technical-data DTO fragment is:

```text
ProjectTechnicalDataDto
├── SchemaVersion
├── AcceptanceCriteria[]
├── LoadCases[]
├── JointSideGeometries[]
├── BoltingAssemblies[]
├── GasketAssemblies[]
├── ComponentMaterials[]
└── FlangedJoints[]
```

## Boundary

The envelope does not serialize F# domain internals such as `FlangedJoint` directly.

`FlangedJointDto` is reference-based: it stores side geometry IDs, assembly IDs, load-case IDs, acceptance-criterion IDs, and component-material roles that resolve against the explicit technical-data fragments above.

Technical-data mapping rejects duplicate fragment identifiers before resolving references so a project file cannot silently bind to the first of multiple matching records. Reference arrays inside a `FlangedJointDto` also reject duplicate load-case, acceptance-criterion, and component-material references.

Required technical-data collections must be present as arrays, even when empty. Null or missing collections are rejected with explicit field names.

Fragment identifiers and joint references must be present and non-empty. Missing side references, blank IDs, blank reference arrays, blank side references, and blank assembly references are rejected before reference resolution.
