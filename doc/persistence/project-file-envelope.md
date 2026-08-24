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
