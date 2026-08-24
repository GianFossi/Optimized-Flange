# Calculation Engine Skeleton — Step 4

## Purpose

Step 4 introduces the first calculation-engine package without adding normative engineering formulas.

## Module

```text
src/OptimizedFlange.Calculations/
├── ProcedureCatalog.fs
├── StructuralValidationEngine.fs
└── CalculationDispatcher.fs
```

## Implemented behavior

- `ProcedureCatalog.structuralValidation` defines the implemented non-normative structural validation procedure.
- `StructuralValidationEngine.run` wraps `FlangedJoint.validateStructure` in the calculation-result contract.
- `CalculationDispatcher.run` routes implemented structural validation requests and returns a stable `CALCULATION.PROCEDURE.NOT_IMPLEMENTED` error for future normative procedure kinds.

## Boundaries

The calculation package depends on `OptimizedFlange.Domain` only.

It does not implement ASME VIII, PCC-1, API 660, IOGP S-614, TEMA, EN 1591, or project-policy engineering equations.

Future normative engines must be added as focused modules with source standard, edition, clause/formula references, validation evidence, and tests.
