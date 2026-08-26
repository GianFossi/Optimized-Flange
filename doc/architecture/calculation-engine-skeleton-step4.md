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

## Registry Boundary Before Normative Formulas

Before normative formulas are implemented, the repository maintains machine-readable registries for:

- planned standards support in `registry/standards-support.json`;
- engineering rule placeholders in `registry/engineering-rules.json`;
- qualification scope in `registry/qualification.json`;
- normative interpretations in `registry/normative-interpretations.json`;
- non-authoritative calculation workbook guides in `registry/reference-guides.json`.

Normative rule placeholders remain `Planned` and unqualified until source editions, clauses, formulas, validation cases, and implementation evidence are added.

The first selected source scope for future inventory is:

- ASME VIII Division 1, 2025 edition;
- ASME VIII Division 2, 2025 edition;
- ASME PCC-1 Appendix O, 2022 edition.
- API 660 paragraph 7.8, 2015 edition;
- IOGP S-614 v18-12 amendments to API 660 paragraph 7.8.

The formula inventory gate is tracked in `registry/formula-inventory.json`. Formula code should not begin until a group is moved from `NeedsManualClauseInventory` to an implementation-ready state with exact references and validation cases.

Reference workbooks may help identify symbols, units, workbook comparison cases, and implementation sequencing. They do not replace source standards and must not be used as normative authority or qualification evidence.
