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
