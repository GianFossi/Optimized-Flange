# Core Domain — Step 2

## Purpose

Step 2 introduces the technical domain skeleton for a bolted and gasketed flange joint. It deliberately contains **no ASME/PCC/API calculation equations**.

## Domain composition

```text
FlangedJoint
├── PrimarySide
│   └── JointSideGeometry
├── MatingSideMode
├── MatingSide?
├── GasketAssembly
│   ├── GasketEnvelope
│   ├── SealingZones[]
│   └── PartitionLayout?
├── BoltingAssembly
│   ├── BoltPattern
│   ├── StudDefinition
│   └── PreloadDefinition
├── LoadCases[]
│   ├── PrimaryCondition
│   ├── MatingCondition
│   └── ExternalLoads(Fx,Fy,Fz,Mx,My,Mz)
├── AcceptanceCriteria[]
└── ComponentMaterials[]
```

## Important boundaries

- Geometry types describe physical geometry. Normative code-effective transformations will be implemented later in dedicated code modules.
- Loads preserve signed vector components. Resultants are derived later and never replace the original components.
- Gasket physical envelope, sealing zones, partition ribs, and future code-effective geometry remain distinct.
- Bolting preserves physical thread/stud data and multiple section-area definitions. A normative rule selects the area it requires; the model does not silently substitute another basis.
- Material snapshots contain resolved properties supplied by the external Materials provider. OptimizedFlange does not duplicate interpolation or thickness-range selection logic.
- `FlangedJoint.validateStructure` only checks non-normative structural consistency.

## Step 3 direction

Before implementing engineering formulas, create the calculation contracts and result/check structures, then start the testing layer. Normative equations must be introduced only with source/edition/clause traceability and tests.
